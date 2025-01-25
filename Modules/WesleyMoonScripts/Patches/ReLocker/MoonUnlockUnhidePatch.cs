using HarmonyLib;
using System;
using System.Reflection;
using WesleyMoonScripts.ScriptableObjects;

namespace WesleyMoonScripts.Patches.ReLocker
{
    [HarmonyPatch]
    public class MoonUnlockUnhidePatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(MoonUnlockUnhide.UnlockUnhidePatch), "genIgnoreMoonsList");
        }

        static void Postfix()
        {
            try
            {
                WesleyScripts.Instance.mls.LogInfo($"Adding {WesleyProgressionIntegration.AllLevels.Count} ingore levels.");
                foreach (var level in WesleyProgressionIntegration.AllLevels)
                {
                    if (level.moonUnlockerProtection)
                    {
                        string entryName = level.NumberlessName.ToLower();
                        MoonUnlockUnhide.UnlockUnhidePatch.ignoreMoons.Add(entryName);
                        WesleyScripts.Instance.mls.LogInfo("Ignoring " + entryName);
                    }
                }
            }
            catch (Exception e)
            {
                WesleyScripts.Instance.mls.LogWarning($"Caught Error patching MoonUnlockUnhide: {e}");
            }
        }
    }
}
