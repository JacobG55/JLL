using JLL.Patches;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace JLL.Behaviors
{
    internal class JWeatherOverride : MonoBehaviour
    {
        [Header("Overrides")]
        public WeatherEffect[] overrideEffects;

        [Header("PropertyOverrides")]
        public LocalVolumetricFog[] foggyWeatherVolumes;

        [Header("Debug")]
        public float overrideDelay = 4f;

        public void Start()
        {
            JLLBase.Instance.mls.LogInfo("Weather Override Start");

            TimeOfDay timeOfDay = TimeOfDay.Instance;

            System.Random random = new System.Random(StartOfRound.Instance.randomMapSeed + 101);

            if (timeOfDay.currentLevelWeather == LevelWeatherType.Foggy)
            {
                foreach (LocalVolumetricFog fog in foggyWeatherVolumes)
                {
                    fog.parameters.meanFreePath = random.Next((int)timeOfDay.currentWeatherVariable, (int)timeOfDay.currentWeatherVariable2);
                }
            }
            foreach (WeatherEffect effect in  overrideEffects)
            {
                effect.effectEnabled = false;
                effect.effectPermanentObject.SetActive(false);
            }
            StartCoroutine(OverrideWeather());
        }

        public WeatherEffect getOverrideEffect(WeatherEffect original)
        {
            foreach (WeatherEffect effect in overrideEffects)
            {
                if (original.name ==  effect.name)
                {
                    return effect;
                }
            }
            return null;
        }

        private IEnumerator OverrideWeather()
        {
            yield return new WaitForSeconds(overrideDelay);

            StartOfRound round = StartOfRound.Instance;
            if (round.currentLevel.currentWeather != LevelWeatherType.None)
            {
                WeatherEffect original = TimeOfDay.Instance.effects[(int)round.currentLevel.currentWeather];
                WeatherEffect weatherEffect = getOverrideEffect(original);

                if (weatherEffect != null)
                {
                    original.effectEnabled = false;
                    weatherEffect.effectEnabled = true;
                    if (weatherEffect.effectPermanentObject != null)
                    {
                        if (original.effectPermanentObject != null)
                        {
                            original.effectPermanentObject.SetActive(value: false);
                        }
                        weatherEffect.effectPermanentObject.SetActive(value: true);
                    }

                    if (!round.currentLevel.overrideWeather)
                    {
                        original.effectEnabled = false;
                        weatherEffect.effectEnabled = true;
                    }
                }
            }
        }
        public void Update()
        {
            Vector3 vector = ((!GameNetworkManager.Instance.localPlayerController.isPlayerDead) ? StartOfRound.Instance.localPlayerController.transform.position : StartOfRound.Instance.spectateCamera.transform.position);
            TimeOfDay timeOfDay = TimeOfDay.Instance;

            for (int i = 0; i < overrideEffects.Length; i++)
            {
                if (overrideEffects[i].effectEnabled)
                {
                    if (!string.IsNullOrEmpty(overrideEffects[i].sunAnimatorBool) && timeOfDay.sunAnimator != null)
                    {
                        timeOfDay.sunAnimator.SetBool(overrideEffects[i].sunAnimatorBool, value: true);
                    }
                    overrideEffects[i].transitioning = false;
                    if (overrideEffects[i].effectObject != null)
                    {
                        overrideEffects[i].effectObject.SetActive(value: true);
                        if (overrideEffects[i].lerpPosition)
                        {
                            overrideEffects[i].effectObject.transform.position = Vector3.Lerp(overrideEffects[i].effectObject.transform.position, vector, Time.deltaTime);
                        }
                        else
                        {
                            overrideEffects[i].effectObject.transform.position = vector;
                        }
                    }
                }
                else if (!overrideEffects[i].transitioning)
                {
                    overrideEffects[i].transitioning = true;
                    if (overrideEffects[i].lerpPosition)
                    {
                        StartCoroutine(TimeOfDayPatch.fadeOutEffect(timeOfDay, overrideEffects[i], vector));
                    }
                    else
                    {
                        TimeOfDayPatch.DisableWeatherEffect(timeOfDay, overrideEffects[i]);
                    }
                }
            }
        }
    }
}
