using GameNetcodeStuff;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using System.Collections;
using JLL.Components;
using UnityEngine.Events;
using JLL.Patches;
using JLL.API;

namespace JLLItemsModule.Components
{
    public class JMeleeWeapon : JGrabbableObject
    {
        [Header("Melee Weapon")]
        public int HitForce = 1;

        [Tooltip("Shovel Default: 1.5\nKnife Default: 0.3")]
        public float weaponRange = 1.5f;
        [Tooltip("Shovel Default: 0.3\nKnife Default: 0.43")]
        public float weaponCooldown = 0.3f;

        [Tooltip("Leave blank for no blood particle on hit")]
        public ParticleSystem bloodParticle;

        private float timeAtLastDamageDealt;

        [Tooltip("Player is the player who swung the weapon")]
        public UnityEvent<PlayerControllerB> OnHitSuccess = new UnityEvent<PlayerControllerB>();
        public LayerMask hitMask = 1084754248;

        [Header("Heavy Weapons")]
        [Tooltip("Heavy Weapons are similar to the shovel or signs.\nNon Heavy Weapons are similar to the Kitchen Knife")]
        public bool isHeavyWeapon = true;
        [Tooltip("Shovel Default: 0.35")]
        public float reelingTime = 0.35f;
        private float reelingAnimSpeed = 1f;
        [Tooltip("Shovel Default: 0.13")]
        public float swingTime = 0.13f;

        private bool reelingUp;
        private bool isHoldingButton;
        private Coroutine? reelingUpCoroutine;

        [Header("Damage Targets")]
        public bool damagePlayers = true;
        [Tooltip("Passes a Player that has been damaged by the weapon")]
        public UnityEvent<PlayerControllerB> OnPlayerHit = new UnityEvent<PlayerControllerB>();
        public bool damageEnemies = true;
        [Tooltip("Passes an Enemy that has been damaged by the weapon")]
        public UnityEvent<EnemyAI> OnEnemyHit = new UnityEvent<EnemyAI>();
        public bool damageVehicles = false;
        [Tooltip("Passes a Vehicle that has been damaged by the weapon")]
        public UnityEvent<VehicleController> OnVehicleHit = new UnityEvent<VehicleController>();
        public bool damageObjects = true;
        [Tooltip("Passes an Object that has been damaged by the weapon")]
        public UnityEvent<IHittable> OnObjectHit = new UnityEvent<IHittable>();

        [Header("Audio")]
        public AudioClip[] hitSFX;
        [Tooltip("Only used for Heavy Weapons")]
        public AudioClip[] reelUpSFX;
        public AudioClip[] swingSFX;
        public AudioSource weaponAudio;

        private PlayerControllerB previousPlayerHeldBy;

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            if (playerHeldBy == null)
            {
                return;
            }

            previousPlayerHeldBy = playerHeldBy;

            if (itemProperties.requiresBattery && insertedBattery.empty)
            {
                return;
            }

            if (isHeavyWeapon)
            {
                isHoldingButton = buttonDown;
                if (!reelingUp && buttonDown)
                {
                    reelingUp = true;
                    previousPlayerHeldBy = playerHeldBy;
                    if (reelingUpCoroutine != null)
                    {
                        if (playerHeldBy.IsOwner) playerHeldBy.playerBodyAnimator.speed = 1f;
                        StopCoroutine(reelingUpCoroutine);
                    }

                    reelingUpCoroutine = StartCoroutine(ReelBackWeapon());
                }
            }
            else
            {
                PlayRandomSFX(swingSFX);

                if (playerHeldBy != null)
                {
                    previousPlayerHeldBy = playerHeldBy;
                    if (playerHeldBy.IsOwner)
                    {
                        playerHeldBy.playerBodyAnimator.SetTrigger("UseHeldItem1");
                    }
                }

                if (IsOwner && Time.realtimeSinceStartup - timeAtLastDamageDealt > weaponCooldown)
                {
                    HitWeapon();
                }
            }
        }

        private IEnumerator ReelBackWeapon()
        {
            playerHeldBy.activatingItem = true;
            playerHeldBy.twoHanded = true;
            playerHeldBy.playerBodyAnimator.ResetTrigger("shovelHit");
            playerHeldBy.playerBodyAnimator.SetBool("reelingUp", value: true);
            if (playerHeldBy.IsOwner)
            {
                reelingAnimSpeed = 0.35f / reelingTime;
                playerHeldBy.playerBodyAnimator.speed = reelingAnimSpeed;
            }
            PlayRandomSFX(reelUpSFX);
            ReelUpSFXServerRpc();
            yield return new WaitForSeconds(reelingTime);
            yield return new WaitUntil(() => !isHoldingButton || !isHeld);
            if (playerHeldBy.IsOwner) playerHeldBy.playerBodyAnimator.speed = 1f;
            SwingHeavyWeapon(!isHeld);
            yield return new WaitForSeconds(swingTime);
            yield return new WaitForEndOfFrame();
            HitWeapon(!isHeld);
            yield return new WaitForSeconds(weaponCooldown);
            reelingUp = false;
            reelingUpCoroutine = null;
        }

        [ServerRpc]
        public void ReelUpSFXServerRpc()
        {
            ReelUpSFXClientRpc();
        }

        [ClientRpc]
        public void ReelUpSFXClientRpc()
        {
            PlayRandomSFX(reelUpSFX);
        }

        public override void DiscardItem()
        {
            if (playerHeldBy != null)
            {
                playerHeldBy.activatingItem = false;
                if (playerHeldBy.IsOwner) playerHeldBy.playerBodyAnimator.speed = 1f;
            }

            base.DiscardItem();
        }

        public virtual void SwingHeavyWeapon(bool cancel = false)
        {
            previousPlayerHeldBy.playerBodyAnimator.SetBool("reelingUp", value: false);
            if (!cancel)
            {
                PlayRandomSFX(swingSFX);
                previousPlayerHeldBy.UpdateSpecialAnimationValue(specialAnimation: true, (short)previousPlayerHeldBy.transform.localEulerAngles.y, 0.4f);
            }
        }

        public bool HitWeapon(bool cancel = false)
        {
            if (previousPlayerHeldBy == null)
            {
                Debug.LogError("Previousplayerheldby is null on this client when HitShovel is called.");
                return false;
            }

            previousPlayerHeldBy.activatingItem = false;
            int surfaceSound = -1;
            bool hitSomething = false;
            bool hitEntity = false;

            if (!cancel)
            {
                previousPlayerHeldBy.twoHanded = false;
                var objectsHitByShovel = Physics.SphereCastAll(previousPlayerHeldBy.gameplayCamera.transform.position + previousPlayerHeldBy.gameplayCamera.transform.right * -0.35f, weaponRange / 1.5f, previousPlayerHeldBy.gameplayCamera.transform.forward, weaponRange, hitMask, QueryTriggerInteraction.Collide);
                var objectsHitByShovelList = objectsHitByShovel.OrderBy((RaycastHit x) => x.distance).ToList();

                List<GameObject> alreadyHit = new List<GameObject>();
                Vector3 hitDir = previousPlayerHeldBy.gameplayCamera.transform.forward;

                foreach (RaycastHit hit in objectsHitByShovelList)
                {
                    if (hit.collider.gameObject.layer == 8 || hit.collider.gameObject.layer == 11)
                    {
                        if (hit.collider.isTrigger) continue;

                        for (int i = 0; i < StartOfRound.Instance.footstepSurfaces.Length; i++)
                        {
                            if (hit.collider.gameObject.tag == StartOfRound.Instance.footstepSurfaces[i].surfaceTag)
                            {
                                surfaceSound = i;
                                hitSomething = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (hit.collider.TryGetComponent(out IHittable hittable))
                        {
                            GameObject? obj = null;
                            DamageTrigger.ColliderType type = DamageTrigger.ColliderType.Object;
                            bool shouldDamage = true;

                            if (hittable is EnemyAICollisionDetect detect)
                            {
                                shouldDamage = damageEnemies;
                                obj = detect.mainScript.gameObject;
                                type = DamageTrigger.ColliderType.Enemy;
                            }
                            else if (hittable is PlayerControllerB player)
                            {
                                if (player.actualClientId == previousPlayerHeldBy.actualClientId)
                                {
                                    continue;
                                }
                                shouldDamage = damagePlayers;
                                obj = player.gameObject;
                                type = DamageTrigger.ColliderType.Player;
                            }
                            else 
                            {
                                shouldDamage = damageObjects;
                                obj = hit.collider.gameObject;
                            }

                            JLogHelper.LogInfo($"{hit.transform.name} {shouldDamage}");

                            if (shouldDamage && obj != null)
                            {
                                if (!alreadyHit.Contains(obj) && OnWeaponHit(hittable, hitDir))
                                {
                                    alreadyHit.Add(obj);

                                    if (!hitSomething) hitSomething = true;
                                    if (!hitEntity) hitEntity = true;

                                    switch (type)
                                    {
                                        case DamageTrigger.ColliderType.Player:
                                            OnPlayerHit.Invoke(hit.collider.GetComponent<PlayerControllerB>());
                                            bloodParticle?.Play(withChildren: true);
                                            break;
                                        case DamageTrigger.ColliderType.Enemy:
                                            OnEnemyHit.Invoke(hit.collider.GetComponent<EnemyAICollisionDetect>().mainScript);
                                            bloodParticle?.Play(withChildren: true);
                                            break;
                                        case DamageTrigger.ColliderType.Object:
                                            OnObjectHit.Invoke(hittable);
                                            break;
                                    }
                                }
                            }
                        }
                        if (damageVehicles && hit.collider.TryGetComponent(out VehicleController vehicle))
                        {
                            if (!alreadyHit.Contains(vehicle.gameObject))
                            {
                                alreadyHit.Add(vehicle.gameObject);

                                hitSomething = true;

                                vehicle.PushTruckServerRpc(previousPlayerHeldBy.transform.position, hitDir);
                                VehicleControllerPatch.DealPermanentDamage(vehicle, HitForce, previousPlayerHeldBy.transform.position);

                                OnVehicleHit.Invoke(vehicle);
                            }
                        }
                    }
                }
            }

            if (hitSomething)
            {
                timeAtLastDamageDealt = Time.realtimeSinceStartup;

                PlayRandomSFX(hitSFX);
                RoundManager.Instance.PlayAudibleNoise(transform.position, 17f, 0.8f);
                if (!hitEntity && surfaceSound != -1)
                {
                    weaponAudio.PlayOneShot(StartOfRound.Instance.footstepSurfaces[surfaceSound].hitSurfaceSFX);
                    WalkieTalkie.TransmitOneShotAudio(weaponAudio, StartOfRound.Instance.footstepSurfaces[surfaceSound].hitSurfaceSFX);
                }

                if (isHeavyWeapon) playerHeldBy.playerBodyAnimator.SetTrigger("shovelHit");
                HitWeaponServerRpc(surfaceSound);
            }

            return hitSomething;
        }

        public virtual bool OnWeaponHit(IHittable target, Vector3 hitDir)
        {
            return target.Hit(HitForce, hitDir, previousPlayerHeldBy, playHitSFX: true, 1);
        }

        [ServerRpc]
        public void HitWeaponServerRpc(int hitSurfaceID)
        {
            HitWeaponClientRpc(hitSurfaceID);
        }

        [ClientRpc]
        public void HitWeaponClientRpc(int hitSurfaceID)
        {
            PlayRandomSFX(hitSFX);
            OnHitSuccess.Invoke(previousPlayerHeldBy);

            if (hitSurfaceID != -1)
            {
                HitSurface(hitSurfaceID);
            }
        }

        private void PlayRandomSFX(AudioClip[] clips)
        {
            if (clips.Length > 1)
            {
                RoundManager.PlayRandomClip(weaponAudio, clips);
            }
            else if (clips.Length == 1)
            {
                weaponAudio.PlayOneShot(clips[0], UnityEngine.Random.Range(0.82f, 1));
            }
        }

        private void HitSurface(int hitSurfaceID)
        {
            weaponAudio.PlayOneShot(StartOfRound.Instance.footstepSurfaces[hitSurfaceID].hitSurfaceSFX);
            WalkieTalkie.TransmitOneShotAudio(weaponAudio, StartOfRound.Instance.footstepSurfaces[hitSurfaceID].hitSurfaceSFX);
        }
    }
}
