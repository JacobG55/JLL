using HarmonyLib;
using JLL.Components;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using JLL.API.LevelProperties;
using JLL.API;

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

                JLogHelper.LogInfo("JLL Network Manager Initialized.", JLogLevel.User);
            }
        }

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void FindAllEnemyTypes()
        {
            List<EnemyType> enemyTypes = new List<EnemyType>();

            SelectableLevel allEnemiesLevel = Object.FindObjectOfType<QuickMenuManager>().testAllEnemiesLevel;

            FindEnemyTypes(allEnemiesLevel.Enemies, ref enemyTypes);
            FindEnemyTypes(allEnemiesLevel.OutsideEnemies, ref enemyTypes);
            FindEnemyTypes(allEnemiesLevel.DaytimeEnemies, ref enemyTypes);

            JLevelPropertyRegistry.AllSortedEnemies.Clear();
            JLevelPropertyRegistry.AllSortedEnemies.AddRange(enemyTypes);
        }

        private static void FindEnemyTypes(List<SpawnableEnemyWithRarity> spawnableEnemies, ref List<EnemyType> enemyTypes)
        {
            for (int i = 0; i < spawnableEnemies.Count; i++)
            {
                if (spawnableEnemies[i].enemyType != null && !enemyTypes.Contains(spawnableEnemies[i].enemyType))
                {
                    enemyTypes.Add(spawnableEnemies[i].enemyType);
                }
            }
        }

        [HarmonyPatch("EndOfGame")]
        [HarmonyPrefix]
        public static void patchEndOfGame()
        {
            JWeatherOverride.Instance = null;

            if (HudManagerPatch.customFilter != null)
            {
                HudManagerPatch.customFilter.underwaterFilter.weight = 0f;
                HudManagerPatch.customFilter = null;
            }

            JLevelPropertyRegistry.RemoveLevelOverrides();
        }

        [HarmonyPatch("OnShipLandedMiscEvents")]
        [HarmonyPrefix]
        public static void patchOnShipLandedMiscEvents()
        {
            for (int i = 0; i < JLevelEventTriggers.EventTriggers.Count; i++)
            {
                JLevelEventTriggers.EventTriggers[i].ShipLanded.Invoke();
            }
        }

        [HarmonyPatch("ShipLeave")]
        [HarmonyPrefix]
        public static void patchShipLeave()
        {
            for (int i = 0; i < JLevelEventTriggers.EventTriggers.Count; i++)
            {
                JLevelEventTriggers.EventTriggers[i].ShipLeaving.Invoke();
            }
        }
    }
}
