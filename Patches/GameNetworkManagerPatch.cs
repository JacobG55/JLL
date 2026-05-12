using HarmonyLib;
using JLL.API;
using JLL.ScriptableObjects;
using System.Collections;
using UnityEngine;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    internal static class GameNetworkManagerPatch
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
            //yield return new WaitUntil(() => JFileHelper.HaveJLLBundlesLoaded);
            while (!JFileHelper.HaveJLLBundlesLoaded)
            {
                JLogHelper.LogInfo($"{JFileHelper.JLLBundlesLoaded} {JFileHelper.LLLBundlesLoaded}");
                yield return null;
            }
            JNetworkPrefabSet.RegisterPrefabs();
        }
    }
}
