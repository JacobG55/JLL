using HarmonyLib;
using JLL.API;
using JLL.ScriptableObjects;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    public class GameNetworkManagerPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void patchStart(GameNetworkManager __instance)
        {
            __instance.StartCoroutine(RegisterNetworkPrefabs());
        }

        private static IEnumerator RegisterNetworkPrefabs()
        {
            yield return new WaitUntil(() => JFileHelper.HaveJLLBundlesLoaded);
            JNetworkPrefabSet.RegisterPrefabs();
        }
    }
}
