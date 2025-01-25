using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WesleyMoonScripts.ScriptableObjects;

namespace WesleyMoonScripts.Patches.ReLocker
{
    [HarmonyPatch]
    internal class DisplayAllMoonsPatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(DisplayAllMoons.ConfigManager), "Init");
        }

        static void Postfix()
        {
            List<string> blackListText = new List<string>();
            blackListText.AddRange(DisplayAllMoons.ConfigManager.BlacklistList.ConfigEntry.Value.Split(';').Where((x) => !x.IsNullOrWhiteSpace()));

            foreach (var level in WesleyProgressionIntegration.AllLevels)
            {
                string name = level.NumberlessName;
                if (level.moonUnlockerProtection && !blackListText.Contains(name))
                {
                    blackListText.Add(name);
                }
            }

            DisplayAllMoons.ConfigManager.BlacklistList.ConfigEntry.Value = string.Join(';', blackListText) + ';';
        }
    }
}
