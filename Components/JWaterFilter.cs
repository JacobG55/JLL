using UnityEngine;
using UnityEngine.Rendering;

namespace JLL.Components
{
    public class JWaterFilter : MonoBehaviour
    {
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
