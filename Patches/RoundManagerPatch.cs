using HarmonyLib;
using JLL.API.LevelProperties;
using JLL.Components;
using Unity.Netcode;
using UnityEngine;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManagerPatch
    {
        [HarmonyPatch("FinishGeneratingLevel")]
        [HarmonyPostfix]
        public static void spawnFinishGeneratingLevel()
        {
            JMaterialReplacer[] matReplacers = GameObject.FindObjectsOfType<JMaterialReplacer>();
            for (int i = 0; i < matReplacers.Length; i++)
            {
                if (matReplacers[i].triggerPostDunGen)
                {
                    matReplacers[i].SearchAndReplace();
                }
            }
        }

        [HarmonyPatch("FinishGeneratingNewLevelClientRpc")]
        [HarmonyPrefix]
        public static void patchPostLevelGeneration()
        {
            JLevelPropertyRegistry.ApplyLevelOverrides();
        }
    }
}
