using System;
using UnityEngine;

namespace JLL.Components
{
    public class RandomClipPlayer : MonoBehaviour
    {
        public AudioSource audioSource;

        public WeightedAudioClip[] weightedClips = new WeightedAudioClip[1] { new WeightedAudioClip() { Weight = 20 } };

        public float minVolume = 0.8f;
        public float maxVolume = 1.0f;

        public bool changePitch = false;
        public float minPitch = 0.6f;
        public float maxPitch = 1.0f;

        [Header("Environment")]
        public bool walkieCanHear = true;
        public float walkieVolumeMultiplier = 0.85f;
        public bool creaturesCanHear = true;
        public float creatureVolumeMultiplier = 0.5f;
        public float creatureRangeMultiplier = 4f;

        [Serializable]
        public class WeightedAudioClip : IWeightedItem
        {
            public AudioClip clip;
            public float volumeMultiplier = 1.0f;

            [Range(0f, 100f)]
            public int Weight = 20;

            public int GetWeight()
            {
                return Weight;
            }
        }

        public void PlayRandomClip()
        {
            if (audioSource && weightedClips.Length > 0)
            {
                int random = IWeightedItem.GetRandomIndex(weightedClips);
                if (weightedClips[random].clip != null)
                {
                    if (changePitch)
                    {
                        audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
                    }
                    float volume = UnityEngine.Random.Range(minVolume, maxVolume) * weightedClips[random].volumeMultiplier;

                    audioSource.PlayOneShot(weightedClips[random].clip, volume);

                    if (walkieCanHear)
                    {
                        WalkieTalkie.TransmitOneShotAudio(audioSource, weightedClips[random].clip, volume * walkieVolumeMultiplier);
                    }
                    if (creaturesCanHear)
                    {
                        RoundManager.Instance.PlayAudibleNoise(audioSource.transform.position, creatureRangeMultiplier * volume, volume * creatureVolumeMultiplier, 0);
                    }
                }
            }
        }
    }
}
