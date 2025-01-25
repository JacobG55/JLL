using HarmonyLib;
using System.Collections.Generic;
using WesleyMoonScripts.ScriptableObjects;
using System;
using System.Reflection;

namespace WesleyMoonScripts.Patches.ReLocker
{
    [HarmonyPatch]
    public class LethalMoonUnlocksPatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(LethalMoonUnlocks.UnlockManager), "InitializeUnlocks");
        }

        static void Postfix()
        {
            try
            {
                List<LethalMoonUnlocks.LMUnlockable> removeLockables = new List<LethalMoonUnlocks.LMUnlockable>();
                foreach (LethalMoonUnlocks.LMUnlockable unlock in LethalMoonUnlocks.UnlockManager.Instance.Unlocks)
                {
                    foreach (var level in WesleyProgressionIntegration.AllLevels)
                    {
                        if (level.moonUnlockerProtection && unlock.ExtendedLevel.NumberlessPlanetName.Equals(level.NumberlessName))
                        {
                            removeLockables.Add(unlock);
                            break;
                        }
                    }
                }
                for (int i = 0; i < removeLockables.Count; i++)
                {
                    LethalMoonUnlocks.UnlockManager.Instance.Unlocks.Remove(removeLockables[i]);
                }
            }
            catch (Exception e)
            {
                WesleyScripts.Instance.mls.LogWarning($"Caught Error patching LethalMoonUnlocks: {e}");
            }
        }
    }
}
