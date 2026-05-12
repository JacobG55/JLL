using HarmonyLib;
using UnityEngine;
using WesleyMoonScripts.Components;

namespace WesleyMoonScripts.Patches
{
    [HarmonyPatch(typeof(EntranceTeleport))]
    internal class EntranceTeleportPatch
    {
        [HarmonyPatch("TeleportPlayerClientRpc")]
        [HarmonyPostfix]
        public static void patchTeleportPlayerClient(EntranceTeleport __instance)
        {
            if (__instance.exitScript != null)
            {
                if (__instance.exitScript.TryGetComponent(out ExitDoorEffects exitFX))
                {
                    exitFX.PlayExitFX();
                }
            }
        }
    }
}
