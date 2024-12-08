using HarmonyLib;
using JLL.API.LevelProperties;
using JLL.Components;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(LungProp))]
    internal class LungPropPatch
    {
        [HarmonyPatch("EquipItem")]
        [HarmonyPrefix]
        public static void patchEquipItem(LungProp __instance)
        {
            if (__instance.isLungDocked)
            {
                JLevelPropertyRegistry.ApparatusPulled = true;
                for (int i = 0; i < JLevelEventTriggers.EventTriggers.Count; i++)
                {
                    JLevelEventTriggers.EventTriggers[i].InvokeApparatus();
                }
            }
        }
    }
}
