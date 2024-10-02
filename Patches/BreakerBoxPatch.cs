using HarmonyLib;
using JLL.Components;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(BreakerBox))]
    internal class BreakerBoxPatch
    {
        [HarmonyPatch("SwitchBreaker")]
        [HarmonyPostfix]
        public static void patchSwitchBreaker(bool on)
        {
            for (int i = 0; i < JLevelEventTriggers.EventTriggers.Count; i++)
            {
                JLevelEventTriggers.EventTriggers[i].ToggleBreakerBox(on);
            }
        }

        [HarmonyPatch("SetSwitchesOff")]
        [HarmonyPostfix]
        public static void patchSetSwitchesOff()
        {
            for (int i = 0; i < JLevelEventTriggers.EventTriggers.Count; i++)
            {
                JLevelEventTriggers.EventTriggers[i].ToggleBreakerBox(false);
            }
        }
    }
}
