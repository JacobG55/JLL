using HarmonyLib;
using JLL.API.Compatability;
using LethalLevelLoader;
using static WesleyMoonScripts.ScriptableObjects.WesleyProgressionIntegration;

namespace WesleyMoonScripts.Patches
{
    internal class LLLContentPatch
    {
        [HarmonyPatch(typeof(PatchedContent), "PopulateContentDictionaries")]
        [HarmonyPostfix]
        static void PopulateContentPatch()
        {
            foreach(WesleyIntegratedLevel level in AllLevels)
            {
                ExtendedLevel? extendedLevel = LLLHelper.GetLevel(level.sceneName);
                if (extendedLevel != null && level.forceLock)
                {
                    extendedLevel.IsRouteLocked = true;
                    extendedLevel.IsRouteHidden = true;
                }
            }
        }
    }
}
