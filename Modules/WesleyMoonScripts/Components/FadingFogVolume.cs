using JLL.Components;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace WesleyMoonScripts.Components
{
    [RequireComponent(typeof(LocalVolumetricFog))]

    public class FadingFogVolume : MonoBehaviour, IDungeonLoadListener
    {
        public Vector2 ActiveDistanceMinMax = new Vector2(5, 10);
        public Vector2 ActiveDurationMinMax = new Vector2(8, 16);

        public Vector2 InActiveDistanceMinMax = new Vector2(15, 25);
        public Vector2 InActiveDurationMinMax = new Vector2(4, 12);

        public Vector2 transitionDurationMinMax = new Vector2(0.5f, 4);

        private bool activePhase = false;
        public bool transitioning = false;
        private float timer = 0;
        private float timerStart = 0;
        private float targetDensity;

        private LocalVolumetricFog volumetricFog;
        private System.Random Random;
        private bool initialized = false;
        public int relativeSeed = 21;

        [Header("SFX")]
        public AudioSource source;
        public AudioClip fadeIn;
        public AudioClip fadeOut;

        public void PostDungeonGeneration()
        {
            volumetricFog = GetComponent<LocalVolumetricFog>();
            Random = new System.Random(StartOfRound.Instance.randomMapSeed + relativeSeed);
            initialized = volumetricFog != null;
        }

        public void Update()
        {
            if (!initialized) return;
            timer -= Time.deltaTime;

            if (transitioning)
            {
                if (timer < 0)
                {
                    transitioning = false;
                    volumetricFog.parameters.meanFreePath = targetDensity;
                    timer = timerStart = RandomBetween(activePhase ? ActiveDurationMinMax : InActiveDurationMinMax);
                }
                else
                {
                    volumetricFog.parameters.meanFreePath = Mathf.Lerp(targetDensity, volumetricFog.parameters.meanFreePath, timer / timerStart);
                }
            }
            else if (timer < 0)
            {
                transitioning = true;
                activePhase = !activePhase;
                targetDensity = RandomBetween(activePhase ? ActiveDistanceMinMax : InActiveDistanceMinMax);
                timer = timerStart = RandomBetween(transitionDurationMinMax);
                
                AudioClip clip = activePhase ? fadeIn : fadeOut;
                if (source != null && clip != null) source.PlayOneShot(clip);
            }
        }

        private float RandomBetween(Vector2 minMax) => Mathf.Lerp(minMax.x, minMax.y, (float)Random.NextDouble());
    }
}
