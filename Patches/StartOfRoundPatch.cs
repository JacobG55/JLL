using HarmonyLib;
using JLL.Components;
using Unity.Netcode;
using UnityEngine;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void spawnNetworkManager(StartOfRound __instance)
        {
            if (__instance.IsHost || __instance.IsServer)
            {
                GameObject obj = GameObject.Instantiate(JLL.Instance.networkObject);
                obj.GetComponent<NetworkObject>().Spawn();
                JLL.Instance.mls.LogInfo("JLL Network Manager Initialized.");
            }
        }

        [HarmonyPatch("EndOfGame")]
        [HarmonyPrefix]
        public static void patchEndOfGame()
        {
            JWeatherOverride.Instance = null;
        }
    }
}
