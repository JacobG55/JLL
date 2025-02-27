using HarmonyLib;
using JLL.API.LevelProperties;
using JLL.Components;
using System.Linq;
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
        [HarmonyPostfix]
        public static void patchPostLevelGeneration()
        {
            if (!JLevelPropertyRegistry.IsLevelGenerated)
            {
                JLevelPropertyRegistry.ApplyLevelOverrides();

                IDungeonLoadListener[] dungeonLoadListener = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IDungeonLoadListener>().ToArray();
                if (dungeonLoadListener.Length > 0)
                {
                    for (int i = 0; i < dungeonLoadListener.Length; i++)
                    {
                        dungeonLoadListener[i].PostDungeonGeneration();
                    }
                }
            }
        }
    }
}
