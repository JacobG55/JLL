using HarmonyLib;
using JLL.API;
using JLL.API.LevelProperties;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(MenuManager))]
    internal static class MenuManagerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static void patchStart()
        {
            JLevelPropertyRegistry.RemoveLevelOverrides();
        }
    }
}
