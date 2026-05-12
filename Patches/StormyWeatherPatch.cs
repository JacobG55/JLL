using HarmonyLib;
using JLL.Components;
using UnityEngine;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(StormyWeather))]
    internal static class StormyWeatherPatch
    {
        [HarmonyPatch("LightningStrike")]
        [HarmonyPostfix]
        public static void patchLightningStrike(Vector3 strikePosition)
        {
            foreach (JLightningRod rod in JLightningRod.All)
            {
                if (Vector3.Distance(strikePosition, rod.transform.position) < rod.detectDist)
                {
                    rod.onStrike.Invoke();
                }
            }
        }
    }
}
