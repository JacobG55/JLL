using GameNetcodeStuff;
using JLL.API;
using JLL.Components.Filters;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class JDestructableObject : NetworkBehaviour, IHittable
    {
        public float maxHealth = 1;
        [HideInInspector]
        public float currentHealth = 1;

        [Tooltip("Damage multiplier when damage is not caused by a player.")]
        public float nonPlayerDamageMultiplier = 1;

        public WeaponEventFilter[] weaponFilters = new WeaponEventFilter[1] { new WeaponEventFilter() { WeaponFilter = new ItemFilter() { shouldCheck = true } } };

        [Serializable]
        public class WeaponEventFilter
        {
            public ItemFilter WeaponFilter = new ItemFilter();
            public float filteredDamageMultiplier = 1;
            public InteractEvent FilteredDamageEvent = new InteractEvent();

            public float GetMultiplier(GrabbableObject item, PlayerControllerB player)
            {
                if (WeaponFilter.Check(item))
                {
                    FilteredDamageEvent.Invoke(player);
                    return filteredDamageMultiplier;
                }
                return 1;
            }

            public void FilterEvent(GrabbableObject item, PlayerControllerB player)
            {
                if (WeaponFilter.shouldCheck && WeaponFilter.CheckValue(item))
                {
                    FilteredDamageEvent.Invoke(player);
                }
            }
        }

        public GameObject DisableObject;
        public UnityEvent DamageEvent = new UnityEvent();
        public UnityEvent DestroyEvent = new UnityEvent();

        [Header("FX")]
        public ParticleSystem? destroyParticles;
        public AudioClip[] damageClips = new AudioClip[0];
        public AudioClip[] destroyClips = new AudioClip[0];
        public float minVolume = 0.8f;
        public float maxVolume = 1.0f;
        public float minPitch = 0.6f;
        public float maxPitch = 1.0f;
        public AudioSource? audioSource;

        public void Start()
        {
            currentHealth = maxHealth;
            JLogHelper.LogInfo($"{name} Set Health: {currentHealth}");
        }

        public void Hit(int force)
        {
            Hit(force, Vector3.zero);
        }

        public bool Hit(int force, Vector3 hitDirection, PlayerControllerB? playerWhoHit = null, bool playHitSFX = false, int hitID = -1)
        {
            JLogHelper.LogInfo($"{name} Hit By Shovel:");
            DamageObjectServerRpc(playerWhoHit == null ? -1 : (int)playerWhoHit.actualClientId, force);
            return true;
        }

        [ServerRpc(RequireOwnership = false)]
        private void DamageObjectServerRpc(int playerWhoSent, int force)
        {
            JLogHelper.LogInfo($"Server RPC | {currentHealth} | {playerWhoSent} {force}");
            if (currentHealth <= 0)
            {
                return;
            }

            float damage = force;
            if (playerWhoSent >= 0)
            {
                PlayerControllerB playerWhoHit = RoundManager.Instance.playersManager.allPlayerScripts[playerWhoSent];
                JLogHelper.LogInfo($"Player: {playerWhoHit.playerUsername} {playerWhoHit.currentlyHeldObjectServer == null}");
                if (playerWhoHit.currentlyHeldObjectServer != null)
                {
                    for (int i = 0; i < weaponFilters.Length; i++)
                    {
                        damage *= weaponFilters[i].GetMultiplier(playerWhoHit.currentlyHeldObjectServer, playerWhoHit);
                        JLogHelper.LogInfo($"New Damage: {damage}");
                    }
                }
            }
            else
            {
                damage *= nonPlayerDamageMultiplier;
                JLogHelper.LogInfo($"New Damage: {damage}");
            }
            if (damage != 0)
            {
                currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
                bool destroyed = currentHealth <= 0;
                PlayDamageFX(destroyed);
                JLogHelper.LogInfo($"Sending Client RPC | Destroyed: {destroyed}");
                DamageObjectClientRpc(playerWhoSent, destroyed);
            }
        }

        [ClientRpc]
        private void DamageObjectClientRpc(int playerWhoSent, bool destroyed)
        {
            if (IsHost || IsServer)
            { 
                return;
            }

            JLogHelper.LogInfo($"Received Client RPC");

            if (playerWhoSent >= 0)
            {
                PlayerControllerB playerWhoHit = RoundManager.Instance.playersManager.allPlayerScripts[playerWhoSent];
                for (int i = 0; i < weaponFilters.Length; i++)
                {
                    weaponFilters[i].FilterEvent(playerWhoHit.currentlyHeldObjectServer, playerWhoHit);
                }
            }

            PlayDamageFX(destroyed);
        }

        private void PlayDamageFX(bool destroyed)
        {
            DamageEvent.Invoke();
            if (destroyed)
            {
                if (destroyClips.Length > 0)
                {
                    PlayRandomSound(destroyClips);
                }
                else if (damageClips.Length > 0)
                {
                    PlayRandomSound(damageClips);
                }

                Destroy();
            }
            else if (damageClips.Length > 0)
            {
                PlayRandomSound(damageClips);
            }
        }

        private void PlayRandomSound(AudioClip[] clips)
        {
            PlaySound(clips[UnityEngine.Random.Range(0, clips.Length)]);
        }

        private void PlaySound(AudioClip clip)
        {
            JLogHelper.LogInfo($"{name} playing sound {clip.name}.");
            if (audioSource == null) return;
            float volume = UnityEngine.Random.Range(minVolume, maxVolume);
            audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(clip, volume);
            WalkieTalkie.TransmitOneShotAudio(audioSource, clip, volume * 0.85f);
            RoundManager.Instance.PlayAudibleNoise(audioSource.transform.position, 4 * volume, volume * 0.5f, 0);
        }

        private void Destroy()
        {
            foreach (Collider collider in GetComponents<Collider>())
            {
                collider.enabled = false;
            }
            DisableObject?.SetActive(false);
            destroyParticles?.Play();
            DestroyEvent.Invoke();
        }
    }
}
