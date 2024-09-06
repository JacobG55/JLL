using HarmonyLib;
using JLL.Components;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using JLL.API.LevelProperties;

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

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void FindAllEnemyTypes()
        {
            List<EnemyType> enemyTypes = new List<EnemyType>();

            SelectableLevel allEnemiesLevel = Object.FindObjectOfType<QuickMenuManager>().testAllEnemiesLevel;

            for (int i = 0; i < allEnemiesLevel.Enemies.Count; i++)
            {
                if (!enemyTypes.Contains(allEnemiesLevel.Enemies[i].enemyType))
                {
                    enemyTypes.Add(allEnemiesLevel.Enemies[i].enemyType);
                }
            }
            for (int i = 0; i < allEnemiesLevel.OutsideEnemies.Count; i++)
            {
                if (!enemyTypes.Contains(allEnemiesLevel.OutsideEnemies[i].enemyType))
                {
                    enemyTypes.Add(allEnemiesLevel.OutsideEnemies[i].enemyType);
                }
            }
            for (int i = 0; i < allEnemiesLevel.DaytimeEnemies.Count; i++)
            {
                if (!enemyTypes.Contains(allEnemiesLevel.DaytimeEnemies[i].enemyType))
                {
                    enemyTypes.Add(allEnemiesLevel.DaytimeEnemies[i].enemyType);
                }
            }

            JLevelPropertyRegistry.AllSortedEnemies.AddRange(enemyTypes);
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
            JLevelEventTriggers.EventTriggers = new List<JLevelEventTriggers>();

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
    }
}
