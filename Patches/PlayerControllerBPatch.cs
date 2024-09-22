using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    public class PlayerControllerBPatch
    {
        [HarmonyPatch("SpawnDeadBody")]
        [HarmonyPrefix]
        public static bool patchSpawnDeadBody(int playerId, Vector3 bodyVelocity, int causeOfDeath, PlayerControllerB deadPlayerController, int deathAnimation = 0)
        {
            if (deathAnimation == -404)
            {
                return false;
            }
            return true;
        }
    }
}
