using HarmonyLib;
using JLL.Behaviors;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        [HarmonyPatch("EndOfGame")]
        [HarmonyPrefix]
        public static void patchEndOfGame()
        {
            if (JWeatherOverride.Instance != null)
            {
                JWeatherOverride.Instance = null;
            }
        }
    }
}
