using HarmonyLib;
using System;
using UnityEngine;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(VehicleController))]
    public class VehicleControllerPatch
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(VehicleController), "DealPermanentDamage")]
        public static void DealPermanentDamage(object instance, int damageAmount, Vector3 damagePosition = default(Vector3)) =>
            throw new NotImplementedException("It's a stub");
    }
}
