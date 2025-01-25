using JLLItemsModule.Components;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace WesleyMoonScripts.Components
{
    public class MicBombItem : JGrabbableObject
    {
        [Header("Mic Bomb Item")]
        public bool isBombActive = true;
        public bool hasExploded = false;
        public bool destroyOnExplode = true;
        public UnityEvent ExplodeEvent = new UnityEvent();
        public float minLoudness = 0.25f;

        public float coolOffRate = 1.0f;
        public float maxAgression = 16f;
        private float currentAgression = 0;

        [Header("Beep Sounds")]
        public float beepRate = 2.0f;
        private float beepTimer = 0;
        public AudioClip[] audioClips = new AudioClip[0];
        public float minVolume = 0.8f;
        public float maxVolume = 1.0f;
        public float minPitch = 0.85f;
        public float maxPitch = 1.2f;
        public AudioSource audioSource;

        public override void Update()
        {
            base.Update();

            if (isBombActive && !hasExploded && currentAgression > 0)
            {
                if (currentAgression > maxAgression)
                {
                    TriggerExplosionServerRpc();
                }

                beepTimer -= Time.deltaTime;
                if (beepTimer < 0)
                {
                    PlayBeepAudio();
                    beepTimer = beepRate / Mathf.Max(1, currentAgression);
                }

                currentAgression -= Time.deltaTime * coolOffRate;
                currentAgression = Mathf.Max(0, currentAgression);
            }
        }

        private void PlayBeepAudio()
        {
            if (audioSource != null && audioClips.Length > 0)
            {
                audioSource.pitch = Random.Range(minPitch, maxPitch);
                audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)], Random.Range(minVolume, maxVolume));
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void TriggerExplosionServerRpc()
        {
            TriggerExplosionClientRpc();
        }

        [ClientRpc]
        private void TriggerExplosionClientRpc()
        {
            if (!hasExploded)
            {
                hasExploded = true;
                ExplodeEvent.Invoke();
                if (destroyOnExplode)
                {
                    DestroyObjectInHand(playerHeldBy);
                }
            }
        }

        public override void OnSetInsideShip(bool isInside)
        {
            base.OnSetInsideShip(isInside);
            if (isInside)
            {
                isBombActive = false;
            }
        }

        public void IncreaseAgression()
        {
            currentAgression++;
        }

        public override void LoadItemSaveData(int saveData)
        {
            base.LoadItemSaveData(saveData);
            isBombActive = (saveData & 1) != 0;
            hasExploded = (saveData & 2) != 0;
        }

        public override int GetItemDataToSave()
        {
            base.GetItemDataToSave();
            int data = 0;
            if (isBombActive) data |= 1;
            if (hasExploded) data |= 2;
            return data;
        }
    }
}
