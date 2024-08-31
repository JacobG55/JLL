using System.Collections.Generic;
using UnityEngine;

namespace JLL.API.LevelProperties
{
    public class JLevelPropertyRegistry
    {
        private static readonly Dictionary<string, JLevelProperties> Registry = new Dictionary<string, JLevelProperties>();

        public static readonly List<EnemyType> AllSortedEnemies = new List<EnemyType>();
        public static readonly List<EntranceTeleport> EntranceTeleports = new List<EntranceTeleport>();

        private static JLevelProperties original = new JLevelProperties();
        public static JLevelProperties GetLevelProperties(SelectableLevel level)
        {
            return GetLevelProperties(level.name);
        }

        public static JLevelProperties GetLevelProperties(string name)
        {
            if (Registry.TryGetValue(name, out JLevelProperties properties))
            {
                return properties;
            }
            return new JLevelProperties();
        }

        public static void RegisterLevelProperties(SelectableLevel level, JLevelProperties properties)
        {
            RegisterLevelProperties(level.name, properties);
        }

        public static void RegisterLevelProperties(string name, JLevelProperties properties)
        {
            if (Registry.TryGetValue(name, out JLevelProperties old))
            {
                old.MergeWith(properties);
            }
            else
            {
                Registry.Add(name, properties);
            }
        }

        public static bool HasEntranceTeleports()
        {
            return EntranceTeleports.Count > 0;
        }

        public static Vector3? GetEntranceTeleportLocation(int id = 0, bool getOutsideEntrance = false, bool getTeleportPosition = false)
        {
            for (int i = 0; i < EntranceTeleports.Count; i++)
            {
                if (EntranceTeleports[i].entranceId == id && getOutsideEntrance == EntranceTeleports[i].isEntranceToBuilding)
                {
                    if (getTeleportPosition)
                    {
                        return EntranceTeleports[i].entrancePoint.position;
                    }
                    else
                    {
                        return EntranceTeleports[i].transform.position;
                    }
                }
            }

            return null;
        }

        internal static void ApplyLevelOverrides()
        {
            EntranceTeleport[] entrances = Object.FindObjectsOfType<EntranceTeleport>(includeInactive: false);
            EntranceTeleports.AddRange(entrances);

            SelectableLevel currentLevel = RoundManager.Instance.currentLevel;
            JLevelProperties currentProperties = GetLevelProperties(currentLevel);

            foreach (LevelPrefab levelPrefab in currentProperties.levelPrefabs)
            {
                GameObject spawned = GameObject.Instantiate(levelPrefab.prefab);
                spawned.transform.position = levelPrefab.position;
                spawned.transform.rotation = levelPrefab.rotation;
            }

            List<EnemyPropertyOverride> originalValues = new List<EnemyPropertyOverride>();
            foreach (EnemyPropertyOverride property in currentProperties.enemyPropertyOverrides)
            {
                EnemyPropertyOverride og = new EnemyPropertyOverride();
                EnemyType target = GetRegisteredEnemy(property.enemyType);

                if (property.MaxCount >= 0)
                {
                    og.MaxCount = target.MaxCount;
                    target.MaxCount = property.MaxCount;
                }
                if (property.PowerLevel >= 0)
                {
                    og.PowerLevel = target.PowerLevel;
                    target.PowerLevel = property.PowerLevel;
                }

                originalValues.Add(og);
            }

            original.enemyPropertyOverrides = originalValues.ToArray();
        }

        internal static void RemoveLevelOverrides()
        {
            EntranceTeleports.Clear();

            foreach (EnemyPropertyOverride property in original.enemyPropertyOverrides)
            {
                EnemyType target = GetRegisteredEnemy(property.enemyType);

                if (property.MaxCount >= 0)
                {
                    target.MaxCount = property.MaxCount;
                }
                if (property.PowerLevel >= 0)
                {
                    target.PowerLevel = property.PowerLevel;
                }
            }

            original = new JLevelProperties();
        }

        public static EnemyType GetRegisteredEnemy(EnemyType enemyType)
        {
            for (int i = 0; i < AllSortedEnemies.Count; i++)
            {
                if (AllSortedEnemies[i].name == enemyType.name)
                {
                    return AllSortedEnemies[i];
                }
            }
            return enemyType;
        }
    }
}
