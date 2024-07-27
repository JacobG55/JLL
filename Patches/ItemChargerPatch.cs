using HarmonyLib;
using JLL.Components;
using UnityEngine;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(ItemCharger))]
    internal class ItemChargerPatch
    {
        [HarmonyPatch("chargeItemDelayed")]
        [HarmonyPrefix]
        public static void patchChargeItemDelayed(GrabbableObject itemToCharge, ItemCharger __instance)
        {
            Transform child = __instance.transform.Find("limited");
            if (child != null)
            {
                if (child.TryGetComponent<ChargeLimiter>(out ChargeLimiter limiter))
                {
                    JLLBase.Instance.mls.LogInfo("Found Limiter");
                    limiter.Charge();
                } 
                else
                {
                /*
                    JLLBase.Instance.mls.LogInfo("Creating Limiter");
                    ChargeLimiter limiter2 = child.gameObject.AddComponent<ChargeLimiter>();
                    limiter2.charges = 1;
                    if (__instance.TryGetComponent<InteractTrigger>(out InteractTrigger trigger))
                    {
                        limiter2.trigger = trigger;
                    }
                    limiter2.Charge();
                */
                }
            }
        }
    }
}
