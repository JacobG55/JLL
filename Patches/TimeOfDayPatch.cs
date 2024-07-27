using HarmonyLib;
using JLL.Behaviors;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(TimeOfDay))]
    internal class TimeOfDayPatch
    {
        [HarmonyPatch("SetWeatherBasedOnVariables")]
        [HarmonyPrefix]
        public static void patchSetWeatherBasedOnVariables(TimeOfDay __instance)
        {
            System.Random random = new System.Random(StartOfRound.Instance.randomMapSeed + 101);
            JLLBase.Instance.mls.LogInfo("logging");
            foreach (JLevelWeatherEffect weatherEffect in JBehavior.GetWeatherEffects())
            {
                if (__instance.currentLevel.sceneName == weatherEffect.getSceneName())
                {
                    weatherEffect.applyEffects(__instance, random);
                }
            }
        }
    }
}
