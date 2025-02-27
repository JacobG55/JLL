using GameNetcodeStuff;
using JLL.API;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JLL.Components
{
    public class TelePoint : MonoBehaviour
    {
        public bool rotateObjects = true;
        
        [Header("Players")]
        [Tooltip("Used to set the player as inside or outside the facility.\n'None' will leave the player in whatever state they were before teleporting.")]
        public Region region = Region.None;
        public bool rotatePlayer = false;
        [Tooltip("Ship teleporters have a delay of 1.\nParticle effects are predetermined to last this amount of time.\nTeleport delay is not applied to teleporting game objects currently.\nThis gets ignored when triggering a random teleport in favor of what the random teleport region is.")]
        public float teleportDelay = 0;
        public TeleportEffect teleportEffect = TeleportEffect.None;
        public AudioClip teleportSound;
        public AudioSource audioSource;

        [Header("Random Teleport")]
        public float randomRange = 10f;
        public RandomTeleportRegion randomTeleportRegion = RandomTeleportRegion.Indoor;

        private readonly List<TeleportEntry> TeleportPlayers = new List<TeleportEntry>();
        private readonly List<TeleportEntry> Expired = new List<TeleportEntry>();

        private class TeleportEntry
        {
            public PlayerControllerB player;
            public float startTime;
            public float delay;

            public bool random;
            public Region region;

            public TeleportEntry(PlayerControllerB player, float delay, bool random, Region region = Region.None)
            {
                this.player = player;
                startTime = Time.realtimeSinceStartup;
                this.delay = delay;
                this.random = random;
                this.region = region;
            }

            public bool Update(TelePoint telePoint)
            {
                if (Time.realtimeSinceStartup - startTime > delay)
                {
                    if (random)
                    {
                        JLLNetworkManager.Instance.RandomTeleportServerRpc((int)player.actualClientId, (int)telePoint.randomTeleportRegion, telePoint.rotatePlayer, telePoint.transform.rotation.y, telePoint.randomRange);
                    }
                    else
                    {
                        JLogHelper.LogInfo($"{telePoint.name} Teleporting {player.actualClientId}", JLogLevel.Debuging);
                        player.TeleportPlayer(telePoint.transform.position, telePoint.rotatePlayer, telePoint.transform.eulerAngles.y);
                        switch (region)
                        {
                            case Region.Outdoor:
                                player.isInsideFactory = false;
                                break;
                            case Region.Indoor:
                                player.isInsideFactory = true;
                                break;
                            default: break;
                        }
                    }
                    return true;
                }
                return false;
            }
        }

        public void Update()
        {
            foreach (TeleportEntry entry in TeleportPlayers)
            {
                if (entry.Update(this))
                {
                    Expired.Add(entry);
                }
            }
            if (Expired.Count > 0)
            {
                foreach (TeleportEntry entry in Expired)
                {
                    TeleportPlayers.Remove(entry);
                }
                Expired.Clear();
            }
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
            TeleportPlayers.Add(new TeleportEntry(player, teleportDelay, false, region));
        }

        public void RandomTeleport(PlayerControllerB player)
        {
            PlayEffects(player);
            TeleportPlayers.Add(new TeleportEntry(player, teleportDelay, true));
        }

        private void PlayEffects(PlayerControllerB? player = null)
        {
            int[] ids = RoundManager.Instance.playersManager.allPlayerScripts.Select((x) => (int)x.actualClientId).ToArray();
            JLogHelper.LogInfo(string.Join(", ", ids), JLogLevel.User);

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
