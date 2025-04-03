using HarmonyLib;
using JLL.API;
using JLL.API.LevelProperties;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(MenuManager))]
    internal class MenuManagerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static void patchStart()
        {
            JLevelPropertyRegistry.RemoveLevelOverrides();
        }
    }
}
