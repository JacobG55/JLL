using HarmonyLib;
using JLL.API;
using JLL.API.LevelProperties;
using JLL.Components;
using UnityEngine;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManagerPatch
    {
        [HarmonyPatch("FinishGeneratingLevel")]
        [HarmonyPrefix]
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
            if (!JLevelPropertyRegistry.IsLevelGenerated)
            {
                JLevelPropertyRegistry.ApplyLevelOverrides();

                JRandomPropPlacer[] randomPropPlacer = GameObject.FindObjectsOfType<JRandomPropPlacer>();
                if (randomPropPlacer.Length > 0)
                {
                    JLogHelper.LogInfo($"Found {randomPropPlacer.Length} JRandomPropPlacer(s).");
                    for (int i = 0; i < randomPropPlacer.Length; i++)
                    {
                        randomPropPlacer[i].PostDungeonGeneration();
                    }
                }
            }
        }
    }
}
