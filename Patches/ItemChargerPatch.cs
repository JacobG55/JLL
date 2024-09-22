using HarmonyLib;
using JLL.API;
using JLL.Components;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(ItemCharger))]
    internal class ItemChargerPatch
    {
        [HarmonyPatch("chargeItemDelayed")]
        [HarmonyPrefix]
        public static void patchChargeItemDelayed(ItemCharger __instance)
        {
            if (__instance.TryGetComponent(out ChargeLimiter limiter))
            {
                JLogHelper.LogInfo("Found Limiter");
                limiter.Charge();
            }
        }
    }
}
