using HarmonyLib;
using JLL.API;
using System.Collections;
using UnityEngine;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    internal class HudManagerPatch
    {
        [HarmonyPatch("DisplayTip")]
        [HarmonyPrefix]
        public static void patchDisplayTip(HUDManager __instance)
        {
            JHudHelper.isTipActive = true;
            __instance.StartCoroutine(TipQueueCheck());
        }

        private static IEnumerator TipQueueCheck()
        {
            yield return new WaitForSeconds(5f);
            JHudHelper.isTipActive = false;
            JHudHelper.DisplayNextTip();
        }
    }
}
