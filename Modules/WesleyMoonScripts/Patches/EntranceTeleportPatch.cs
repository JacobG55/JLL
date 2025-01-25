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
        public static void patchTeleportPlayerClient(AudioSource ___exitPointAudio)
        {
            if (___exitPointAudio != null)
            {
                if (___exitPointAudio.TryGetComponent(out ExitDoorEffects exitFX))
                {
                    exitFX.PlayExitFX();
                }
            }
        }
    }
}
