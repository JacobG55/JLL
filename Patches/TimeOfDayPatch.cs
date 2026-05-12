using HarmonyLib;
using JLL.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using UnityEngine;
using JLL.API;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(TimeOfDay))]
    internal static class TimeOfDayPatch
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
            JWeatherOverride overrideWeather = JWeatherOverride.Instance;
            if (overrideWeather != null)
            {
                WeatherEffect overriden = overrideWeather.getOverrideEffect(effect.name);

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
            JWeatherOverride overrideWeather = JWeatherOverride.Instance;
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

        [HarmonyPatch("MoveTimeOfDay")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> MoveTimeOfDay_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo HourField = AccessTools.Field(typeof(TimeOfDay), nameof(TimeOfDay.hour));
            FieldInfo PrevHourField = AccessTools.Field(typeof(TimeOfDay), "previousHour");

            MethodInfo Method = AccessTools.Method(typeof(TimeOfDayPatch), nameof(ProgressHour));

            return JTranspilerHelper.AddAfter(instructions, "MoveTimeOfDay", 
                [
                    new CodeTest(OpCodes.Ldarg_0), 
                    new CodeTest(OpCodes.Ldarg_0), 
                    new CodeTest(OpCodes.Ldfld, (code) => code.LoadsField(HourField)), 
                    new CodeTest(OpCodes.Stfld, (code) => code.StoresField(PrevHourField))
                ], 
                Method, MethodParams.Self);
        }

        private static void ProgressHour(TimeOfDay timeOfDay)
        {
            try
            {
                foreach (JLevelEventTriggers trigger in JLevelEventTriggers.EventTriggers)
                {
                    foreach (var hourEvent in trigger.hourlyEvents)
                    {
                        if (hourEvent.hour == timeOfDay.hour)
                        {
                            hourEvent.hourEvent.Invoke();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                JLogHelper.LogError(ex.ToString());
            }
        }
    }
}
