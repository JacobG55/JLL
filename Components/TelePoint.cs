using GameNetcodeStuff;
using JLL.API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JLL.Components
{
    public enum RandomTeleportRegion
    {
        Indoor,
        Outdoor,
        Moon,
        Nearby,
        RandomPlayer,
    }

    public class TelePoint : MonoBehaviour
    {
        public bool rotateObjects = true;
        
        [Header("Players")]
        public bool rotatePlayer = false;
        [Tooltip("Ship teleporters have a delay of 1.\nParticle effects are predetermined to last this amount of time.\nTeleport delay is not applied to teleporting game objects currently.")]
        public float teleportDelay = 0;
        public TeleportEffect teleportEffect = TeleportEffect.None;
        public AudioClip teleportSound;
        public AudioSource audioSource;

        private readonly List<int> TeleportPlayerIds = new List<int>();

        [Header("Random Teleport")]
        public float randomRange = 10f;
        public RandomTeleportRegion randomTeleportRegion = RandomTeleportRegion.Indoor;

        public enum TeleportEffect
        {
            None,
            ShipTeleport,
            InverseTeleport,
        }

        public void Teleport(GameObject obj)
        {
            PlayEffects();
            obj.transform.position = transform.position;
            if (rotateObjects)
            {
                obj.transform.rotation = transform.rotation;
            }
        }

        public void TeleportPlayer(PlayerControllerB player)
        {
            PlayEffects(player);
            TeleportPlayerIds.Add((int)player.actualClientId);
            StartCoroutine(TeleportPlayer());
        }

        private IEnumerator TeleportPlayer()
        {
            yield return new WaitForSeconds(teleportDelay);
            if (TeleportPlayerIds.Count > 0)
            {
                RoundManager.Instance.playersManager.allPlayerScripts[TeleportPlayerIds[0]].TeleportPlayer(transform.position, rotatePlayer, transform.eulerAngles.y);
                TeleportPlayerIds.RemoveAt(0);
            }
        }

        public void RandomTeleport(PlayerControllerB player)
        {
            PlayEffects(player);
            TeleportPlayerIds.Add((int)player.actualClientId);
            StartCoroutine(TeleportPlayerRandomly());
        }

        private IEnumerator TeleportPlayerRandomly()
        {
            yield return new WaitForSeconds(teleportDelay);
            if (TeleportPlayerIds.Count > 0)
            {
                JLLNetworkManager.Instance.RandomTeleportServerRpc(TeleportPlayerIds[0], (int)randomTeleportRegion, rotatePlayer, transform.rotation.y, randomRange);
                TeleportPlayerIds.RemoveAt(0);
            }
        }

        private void PlayEffects(PlayerControllerB? player = null)
        {
            if (player != null)
            {
                switch(teleportEffect)
                {
                    case TeleportEffect.ShipTeleport:
                        player.beamUpParticle.Play();
                        break;
                    case TeleportEffect.InverseTeleport:
                        player.beamOutParticle.Play();
                        break;
                    default: break;
                }
                if (teleportSound != null)
                {
                    player.movementAudio.PlayOneShot(teleportSound);
                }
            }

            if (teleportSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(teleportSound);
            }
        }
    }
}
