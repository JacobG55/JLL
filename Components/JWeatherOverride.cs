using JLL.API;
using JLL.API.Compatability;
using JLL.Patches;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace JLL.Components
{
    public class JWeatherOverride : MonoBehaviour
    {
        public static JWeatherOverride? Instance;

        [Header("Overrides")]
        public WeatherEffect[] overrideEffects = new WeatherEffect[0];

        [Header("Foggy Weather")]
        public LocalVolumetricFog[] foggyWeatherVolumes = new LocalVolumetricFog[0];
        public Volume[] foggyVolumes = new Volume[0];

        [Header("Debug")]
        public float overrideDelay = 4f;

        public void Start()
        {
            JLogHelper.LogInfo("Weather Override Start", JLogLevel.User);
            System.Random random = new System.Random(StartOfRound.Instance.randomMapSeed + 101);

            foreach (WeatherEffect effect in overrideEffects)
            {
                effect.effectEnabled = false;
                if (effect.effectPermanentObject != null)
                {
                    effect.effectPermanentObject.SetActive(false);
                }
            }

            if (TimeOfDay.Instance.currentLevelWeather == LevelWeatherType.Foggy)
            {
                float fogMeanPath = random.Next((int)TimeOfDay.Instance.currentWeatherVariable, (int)TimeOfDay.Instance.currentWeatherVariable2);

                foreach (LocalVolumetricFog fog in foggyWeatherVolumes)
                {
                    fog.parameters.meanFreePath = fogMeanPath;
                }
                foreach (Volume volume in foggyVolumes)
                {
                    VolumeProfile prof = volume.profile;
                    if (prof.TryGet(out Fog fog))
                    {
                        fog.meanFreePath.value = fogMeanPath;
                    }
                }
            }
            StartCoroutine(OverrideWeather());
        }

        public WeatherEffect? getOverrideEffect(string original)
        {
            foreach (WeatherEffect effect in overrideEffects)
            {
                if (original.Equals(effect.name))
                {
                    return effect;
                }
            }
            return null;
        }

        private IEnumerator OverrideWeather()
        {
            yield return new WaitForSeconds(overrideDelay);

            OverrideWeatherObjects();
            Instance = this;
        }

        private void OverrideWeatherObjects()
        {
            if (JCompatabilityHelper.IsModLoaded.WeatherRegistry)
            {
                JWeatherRegistryHelper.OverrideWeatherObjects(this);
            }

            if (StartOfRound.Instance.currentLevel.currentWeather != LevelWeatherType.None)
            {
                WeatherEffect original = TimeOfDay.Instance.effects[(int)StartOfRound.Instance.currentLevel.currentWeather];
                WeatherEffect? weatherEffect = getOverrideEffect(original.name);

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

                    if (!StartOfRound.Instance.currentLevel.overrideWeather)
                    {
                        original.effectEnabled = false;
                        weatherEffect.effectEnabled = true;
                    }
                }
            }
        }

        public void Update()
        {
            CheckWeatherEffectChanges();

            Vector3 vector = ((!GameNetworkManager.Instance.localPlayerController.isPlayerDead) ? StartOfRound.Instance.localPlayerController.transform.position : StartOfRound.Instance.spectateCamera.transform.position);

            for (int i = 0; i < overrideEffects.Length; i++)
            {
                if (overrideEffects[i].effectEnabled)
                {
                    if (!string.IsNullOrEmpty(overrideEffects[i].sunAnimatorBool) && TimeOfDay.Instance.sunAnimator != null)
                    {
                        TimeOfDay.Instance.sunAnimator.SetBool(overrideEffects[i].sunAnimatorBool, value: true);
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
                        StartCoroutine(TimeOfDayPatch.fadeOutEffect(TimeOfDay.Instance, overrideEffects[i], vector));
                    }
                    else
                    {
                        TimeOfDayPatch.DisableWeatherEffect(TimeOfDay.Instance, overrideEffects[i]);
                    }
                }
            }
        }

        private void CheckWeatherEffectChanges()
        {
            if (JCompatabilityHelper.IsModLoaded.WeatherRegistry)
            {
                JWeatherRegistryHelper.CheckWeatherChanges(this);
            }

            if (StartOfRound.Instance.currentLevel.currentWeather != LevelWeatherType.None)
            {
                WeatherEffect original = TimeOfDay.Instance.effects[(int)StartOfRound.Instance.currentLevel.currentWeather];
                WeatherEffect? weatherEffect = getOverrideEffect(original.name);

                if (weatherEffect != null)
                {
                    if (original.effectEnabled)
                    {
                        original.effectEnabled = false;
                        weatherEffect.effectEnabled = true;
                    }
                    if (original.effectPermanentObject != null && weatherEffect.effectPermanentObject != null)
                    {
                        if (original.effectPermanentObject.activeSelf)
                        {
                            original.effectPermanentObject.SetActive(false);
                            weatherEffect.effectPermanentObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }
}
