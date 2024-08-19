using HarmonyLib;
using JLL.Components;
using System;
using System.Collections;
using UnityEngine;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(TimeOfDay))]
    internal class TimeOfDayPatch
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(TimeOfDay), "DisableWeatherEffect")]
        public static void DisableWeatherEffect(object instance, WeatherEffect effect) =>
            throw new NotImplementedException("It's a stub");

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(TimeOfDay), "fadeOutEffect")]
        public static IEnumerator fadeOutEffect(object instance, WeatherEffect effect, Vector3 moveFromPosition) =>
            throw new NotImplementedException("It's a stub");


        [HarmonyPatch("DisableWeatherEffect")]
        [HarmonyPrefix]
        public static void DisableWeatherEffect(WeatherEffect effect)
        {
            JWeatherOverride? overrideWeather = JWeatherOverride.Instance;
            if (overrideWeather != null)
            {
                WeatherEffect? overriden = overrideWeather.getOverrideEffect(effect.name);

                if (overriden != null)
                {
                    if (overriden.effectObject != null)
                    {
                        overriden.effectObject.SetActive(false);
                    }
                }
            }
        }

        [HarmonyPatch("DisableAllWeather")]
        [HarmonyPrefix]
        public static void DisableAllWeather(bool deactivateObjects)
        {
            JWeatherOverride? overrideWeather = JWeatherOverride.Instance;
            if (overrideWeather != null)
            {
                for (int i = 0; i < overrideWeather.overrideEffects.Length; i++)
                {
                    WeatherEffect effect = overrideWeather.overrideEffects[i];

                    effect.effectEnabled = false;
                    if (deactivateObjects)
                    {
                        if (effect.effectObject != null)
                        {
                            effect.effectObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }
}
