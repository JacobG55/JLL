using GameNetcodeStuff;
using JLL.API.Events;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class JActionEvents : NetworkBehaviour, IHittable, IShockableWithGun, INoiseListener
    {
        [Header("Hittable")]
        public bool canBeHit = true;
        public IntEvent OnHit = new IntEvent();
        public InteractEvent OnPlayerHit = new InteractEvent();

        [Header("Shockable")]
        public bool canBeShocked = false;
        public float shockDifficulty = 1.0f;
        public Vector3 shockPositionOffset = Vector3.zero;
        public InteractEvent OnShock = new InteractEvent();
        public UnityEvent OnStopShocking = new UnityEvent();

        [Header("NoiseListener")]
        public bool isListening = false;
        public float minLoudness = 0.25f;
        public int minTimesInOneSpot = 0;
        public UnityEvent SoundListenedEvent = new UnityEvent();

        public bool Hit(int force, Vector3 hitDirection, PlayerControllerB? playerWhoHit = null, bool playHitSFX = false, int hitID = -1)
        {
            if (!canBeHit)
            {
                return false;
            }
            OnHit.Invoke(force);
            if (playerWhoHit != null)
            {
                OnPlayerHit.Invoke(playerWhoHit);
            }
            return true;
        }

        public bool CanBeShocked()
        {
            return canBeShocked;
        }

        public float GetDifficultyMultiplier()
        {
            return shockDifficulty;
        }

        public NetworkObject GetNetworkObject()
        {
            return NetworkObject;
        }

        public Vector3 GetShockablePosition()
        {
            return transform.position + shockPositionOffset;
        }

        public Transform GetShockableTransform()
        {
            return transform;
        }

        public void ShockWithGun(PlayerControllerB shockedByPlayer)
        {
            OnShock.Invoke(shockedByPlayer);
        }

        public void StopShockingWithGun()
        {
            OnStopShocking.Invoke();
        }

        public void DetectNoise(Vector3 noisePosition, float noiseLoudness, int timesPlayedInOneSpot, int noiseID)
        {
            if (!isListening)
            {
                return;
            }

            if (Physics.Linecast(transform.position, noisePosition, 256))
            {
                noiseLoudness /= 2f;
            }

            if (noiseLoudness < minLoudness || timesPlayedInOneSpot < minTimesInOneSpot)
            {
                return;
            }

            SoundListenedEvent.Invoke();
        }
    }
}
