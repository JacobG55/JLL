using GameNetcodeStuff;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using static JLL.Components.ItemSpawner;

namespace JLLItemsModule.Components
{
    public class JThrowableItem : JGrabbableObject
    {
        [Header("Throwable Item")]
        public bool throwable = true;
        public string throwString = "Throw grenade: [RMB]";

        private LayerMask projectileMask = 268437761;
        public AnimationCurve projectileFallCurve;
        public AnimationCurve projectileVerticalFallCurve;
        public AnimationCurve projectileVerticalFallCurveNoBounce;

        [Header("Explosion Settings")]
        public bool damageOnExplode = false;
        public bool stunOnExplode = false;
        public bool spawnItemsOnExplode = false;

        public GameObject? explosionPrefab;

        public bool hasExploded = false;
        public float chanceToExplodeOnThrow = 100f;
        public float chanceToExplodeOnDropped = 100f;
        public bool destroyOnExplode = false;

        [Header("Interact Before Throw")]
        public bool hasInteraction = false;
        public string interactString = "Pull pin: [RMB]";
        public string playerAnimation = "PullGrenadePin";

        public bool interactedWith = false;
        private bool inInteractAnimation = false;
        private Coroutine? interactCoroutine;

        public bool interactionStartsTimer = false;
        [Tooltip("Waits timeToExplode before exploding. If interactionStartsTimer is checked the timer will have to run twice for the explosion to occur.")]
        public bool explodeOnTimer = false;
        private bool markedToExplode = false;
        public float timeToExplode = 2.25f;
        private float explodeTimer = 0;

        [Header("Explosion Properties")]
        public float killDistance = 0.5f;
        public float damageRange = 3f;
        public int nonLethalDamage = 40;
        public float pushForce = 45f;
        public bool goThroughCar = false;

        [Header("Stun Explosion Properties")]
        public bool affectAudio = true;
        public float flashSeverityMultiplier = 1f;
        public float enemyStunTime = 7.5f;
        public float flashSeverityDistanceRolloff = 1f;

        [Header("Item Spawner")]
        [Tooltip("How many items get spawned in")]
        public int numberToSpawn = 1;
        public SpawnPoolSource SourcePool = SpawnPoolSource.CustomList;
        public WeightedItemRefrence[] CustomList = new WeightedItemRefrence[1] { new WeightedItemRefrence() };
        [Tooltip("Spawn offsets. Index is the number spawned")]
        public Vector3[] SpawnOffsets = new Vector3[0];

        [Header("Events")]
        public UnityEvent ExplodeEvent = new UnityEvent();
        public InteractEvent InteractionEvent = new InteractEvent();
        public InteractEvent CollisionEvent = new InteractEvent();

        [Header("FX")]
        [Tooltip("Trigger: \"pullPin\"\nTrigger: \"explode\"")]
        public Animator? itemAnimator;
        public AudioSource itemAudio;
        public AudioClip? interactSFX;
        public AudioClip? explodeSFX;

        private bool explodeOnThrow = false;
        private bool explodeOnDrop = false;
        private bool wasThrown = false;

        private PlayerControllerB? playerThrownBy;

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (inInteractAnimation)
            {
                return;
            }

            if (hasInteraction && !interactedWith)
            {
                if (interactCoroutine == null)
                {
                    playerHeldBy.activatingItem = true;
                    interactCoroutine = StartCoroutine(interactAnimation());
                }
            }
            else if (IsOwner)
            {
                if (throwable)
                {
                    wasThrown = true;
                    playerHeldBy.DiscardHeldObject(placeObject: true, null, GetThrowDestination());
                }
            }
        }

        public override void DiscardItem()
        {
            if (playerHeldBy != null)
            {
                playerHeldBy.activatingItem = false;
            }

            base.DiscardItem();
        }

        public override void EquipItem()
        {
            base.EquipItem();
            playerThrownBy = playerHeldBy;

            explodeOnThrow = false;
            explodeOnDrop = false;
            wasThrown = false;

            SetExplodeOnThrowServerRpc();

            //SetControlTipForItem();
            //EnableItemMeshes(enable: true);
            //isPocketed = false;

            /*
            if (!hasBeenHeld)
            {
                hasBeenHeld = true;
                if (!isInShipRoom && !StartOfRound.Instance.inShipPhase && StartOfRound.Instance.currentLevel.spawnEnemiesAndScrap)
                {
                    RoundManager.Instance.valueOfFoundScrapItems += scrapValue;
                }
            }
            */
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetExplodeOnThrowServerRpc()
        {
            SetExplodeOnThrowClientRpc(new System.Random(StartOfRound.Instance.randomMapSeed + 10 + (int)transform.position.x + (int)transform.position.z).Next(0, 100));
        }

        [ClientRpc]
        public void SetExplodeOnThrowClientRpc(int random)
        {
            explodeOnThrow = random <= chanceToExplodeOnThrow;
            explodeOnDrop = random <= chanceToExplodeOnDropped;
        }

        public override void SetControlTipsForItem()
        {
            List<string> tips = new List<string>();
            tips.AddRange(itemProperties.toolTips);
            if (interactedWith)
            {
                tips.Add(interactString);
            }
            else if (throwable)
            {
                tips.Add(throwString);
            }
            HUDManager.Instance.ChangeControlTipMultiple(tips.ToArray(), holdingItem: true, itemProperties);
        }

        public override void FallWithCurve()
        {
            float magnitude = (startFallingPosition - targetFloorPosition).magnitude;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(itemProperties.restingRotation.x, transform.eulerAngles.y, itemProperties.restingRotation.z), 14f * Time.deltaTime / magnitude);
            transform.localPosition = Vector3.Lerp(startFallingPosition, targetFloorPosition, projectileFallCurve.Evaluate(fallTime));
            if (magnitude > 5f)
            {
                transform.localPosition = Vector3.Lerp(new Vector3(transform.localPosition.x, startFallingPosition.y, transform.localPosition.z), new Vector3(transform.localPosition.x, targetFloorPosition.y, transform.localPosition.z), projectileVerticalFallCurveNoBounce.Evaluate(fallTime));
            }
            else
            {
                transform.localPosition = Vector3.Lerp(new Vector3(transform.localPosition.x, startFallingPosition.y, transform.localPosition.z), new Vector3(transform.localPosition.x, targetFloorPosition.y, transform.localPosition.z), projectileVerticalFallCurve.Evaluate(fallTime));
            }

            fallTime += Mathf.Abs(Time.deltaTime * 12f / magnitude);
        }

        private IEnumerator interactAnimation()
        {
            inInteractAnimation = true;
            playerHeldBy.activatingItem = true;
            playerHeldBy.doingUpperBodyEmote = 1.16f;
            playerHeldBy.playerBodyAnimator.SetTrigger(playerAnimation);

            itemAnimator?.SetTrigger("pullPin");
            if (itemAudio && interactSFX != null)
            {
                itemAudio.PlayOneShot(interactSFX);
                WalkieTalkie.TransmitOneShotAudio(itemAudio, interactSFX, 0.8f);
            }
            InteractionEvent.Invoke(playerHeldBy);

            yield return new WaitForSeconds(1f);

            if (playerHeldBy != null)
            {
                if (!destroyOnExplode)
                {
                    playerHeldBy.activatingItem = false;
                }

                playerThrownBy = playerHeldBy;
            }

            inInteractAnimation = false;
            interactedWith = true;
            itemUsedUp = true;
            if (IsOwner && playerHeldBy != null)
            {
                SetControlTipsForItem();
            }
        }

        public override void Update()
        {
            base.Update();
            if (((interactionStartsTimer && interactedWith) || (explodeOnTimer && markedToExplode)) && !hasExploded)
            {
                explodeTimer += Time.deltaTime;
                if (explodeTimer > timeToExplode)
                {
                    if (interactionStartsTimer && explodeOnTimer && !markedToExplode)
                    {
                        markedToExplode = true;
                        explodeTimer = 0;
                    }
                    else
                    {
                        ExplodeProjectile(destroyOnExplode);
                    }
                }
            }
        }

        public override void Start()
        {
            base.Start();
            if (!hasInteraction)
            {
                interactedWith = true;
            }
        }

        public override void OnHitGround()
        {
            base.OnHitGround();
            if (playerThrownBy != null && (!hasInteraction || interactedWith))
            {
                CollisionEvent.Invoke(playerThrownBy);
                OnItemCollision(wasThrown);
            }
        }

        public virtual void OnItemCollision(bool thrown)
        {
            if ((thrown && explodeOnThrow) || (!thrown && explodeOnDrop))
            {
                ExplodeProjectile(destroyOnExplode);
            }
        }

        public void ExplodeProjectile(bool destroy = false)
        {
            if (hasExploded)
            {
                return;
            }

            if (!StartOfRound.Instance.currentLevel.spawnEnemiesAndScrap && parentObject == FindObjectOfType<DepositItemsDesk>().deskObjectsContainer)
            {
                if (playerThrownBy != null)
                {
                    playerThrownBy.activatingItem = false;
                }

                return;
            }

            itemAnimator?.SetTrigger("explode");

            if (explodeOnTimer && !markedToExplode)
            {
                markedToExplode = true;
                return;
            }

            hasExploded = true;
            ExplodeEvent.Invoke();

            Transform parent = (!isInElevator) ? RoundManager.Instance.mapPropsContainer.transform : StartOfRound.Instance.elevatorTransform;
            CreateExplosion(parent);

            if (destroy)
            {
                DestroyObjectInHand(playerThrownBy);
            }
        }

        public virtual void CreateExplosion(Transform parent)
        {
            if (damageOnExplode)
            {
                Landmine.SpawnExplosion(transform.position, false, killDistance, damageRange, nonLethalDamage, pushForce, goThroughCar: goThroughCar);
            }
            if (stunOnExplode)
            {
                StunGrenadeItem.StunExplosion(transform.position, affectAudio, flashSeverityMultiplier, enemyStunTime, flashSeverityDistanceRolloff, isHeld, playerHeldBy, playerThrownBy);
            }

            if (spawnItemsOnExplode)
            {
                SpawnItemsOnServer(numberToSpawn);
            }

            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity, parent);
            }

            if (itemAudio && explodeSFX != null)
            {
                itemAudio.PlayOneShot(explodeSFX);
                WalkieTalkie.TransmitOneShotAudio(itemAudio, explodeSFX);
            }
        }

        public void ResetExplosionTimer()
        {
            explodeTimer = 0;
        }

        public void SpawnItemsOnServer(int amount)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.25f;
            Transform parent = ((!isInElevator) ? RoundManager.Instance.mapPropsContainer.transform : StartOfRound.Instance.elevatorTransform);
            SpawnRandomItems(SourcePool, spawnPos, parent, CustomList, SpawnOffsets, amount);
        }

        public Vector3 GetThrowDestination()
        {
            Vector3 position = transform.position;
            Debug.DrawRay(playerHeldBy.gameplayCamera.transform.position, playerHeldBy.gameplayCamera.transform.forward, Color.yellow, 15f);
            Ray grenadeThrowRay = new Ray(playerHeldBy.gameplayCamera.transform.position, playerHeldBy.gameplayCamera.transform.forward);
            position = ((!Physics.Raycast(grenadeThrowRay, out RaycastHit grenadeHit, 12f, projectileMask, QueryTriggerInteraction.Ignore)) ? grenadeThrowRay.GetPoint(10f) : grenadeThrowRay.GetPoint(grenadeHit.distance - 0.05f));
            Debug.DrawRay(position, Vector3.down, Color.blue, 15f);
            grenadeThrowRay = new Ray(position, Vector3.down);
            if (Physics.Raycast(grenadeThrowRay, out grenadeHit, 30f, projectileMask, QueryTriggerInteraction.Ignore))
            {
                return grenadeHit.point + Vector3.up * 0.05f;
            }

            return grenadeThrowRay.GetPoint(30f);
        }

        void OnDrawGizmosSelected()
        {
            if (damageOnExplode)
            {
                if (killDistance > 0)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(transform.position, killDistance);
                }
                if (damageRange > 0)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(transform.position, damageRange);
                }
            }
        }
    }
}
