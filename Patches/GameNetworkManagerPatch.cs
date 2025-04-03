using HarmonyLib;
using JLL.API;
using JLL.ScriptableObjects;
using System.Collections;
using UnityEngine;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    public class GameNetworkManagerPatch
    {
        private static bool registeredPrefabs = false;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void patchStart(GameNetworkManager __instance)
        {
            if (!registeredPrefabs)
            {
                registeredPrefabs = true;
                __instance.StartCoroutine(RegisterNetworkPrefabs());
            }
        }

        private static IEnumerator RegisterNetworkPrefabs()
        {
            yield return new WaitUntil(() => JFileHelper.HaveJLLBundlesLoaded);
            JNetworkPrefabSet.RegisterPrefabs();
        }
    }
}
