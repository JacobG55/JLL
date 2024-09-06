using HarmonyLib;
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
                for (int i = 0; i < JLevelEventTriggers.EventTriggers.Count; i++)
                {
                    JLevelEventTriggers.EventTriggers[i].InvokeApparatus();
                }
            }
        }
    }
}
