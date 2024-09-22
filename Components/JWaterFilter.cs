using UnityEngine;
using UnityEngine.Rendering;

namespace JLL.Components
{
    public class JWaterFilter : MonoBehaviour
    {
        [Tooltip("If you have multiple of the same type of water in your level you can point all of your JWaterFilter scripts to the same Volume")]
        public Volume underwaterFilter;

        [Header("Audio")]
        public bool playBreathingSounds = true;
        public AudioSource customUnderwaterSounds;
        public bool enableUnderwaterLowPassFilter = true;
        public float underwaterLowPassCutoff = 700f;

        public void Start ()
        {
            underwaterFilter.weight = 0f;
        }

        public void UnderwaterFilters(HUDManager manager, bool isSpectating)
        {
            manager.audioListenerLowPass.enabled = enableUnderwaterLowPassFilter;
            manager.audioListenerLowPass.cutoffFrequency = Mathf.Lerp(manager.audioListenerLowPass.cutoffFrequency, underwaterLowPassCutoff, 10f * Time.deltaTime);
            underwaterFilter.weight = 1f;

            if (playBreathingSounds && !isSpectating && !manager.breathingUnderwaterAudio.isPlaying)
            {
                manager.breathingUnderwaterAudio.Play();
            }

            if (!isSpectating && customUnderwaterSounds != null)
            {
                if (!customUnderwaterSounds.isPlaying)
                {
                    customUnderwaterSounds.Play();
                }
            }
        }
    }
}
