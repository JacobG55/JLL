using System.Collections.Generic;
using UnityEngine;

namespace JLL.API.LevelProperties
{
    public class JLevelPropertyRegistry
    {
        private static readonly Dictionary<string, JLevelPropertyEntry> Registry = new Dictionary<string, JLevelPropertyEntry>();
        public static bool IsLevelGenerated { private set; get; } = false;

        public static readonly List<EnemyType> AllSortedEnemies = new List<EnemyType>();
        public static readonly List<EntranceTeleport> EntranceTeleports = new List<EntranceTeleport>();
        private static Terminal Terminal;

        private static JLevelPropertyEntry original = new JLevelPropertyEntry();
        public static JLevelPropertyEntry GetLevelProperties(SelectableLevel level)
        {
            return GetLevelProperties(level.sceneName);
        }

        public static JLevelPropertyEntry GetLevelProperties(string name)
        {
            if (Registry.TryGetValue(name, out JLevelPropertyEntry properties))
            {
                return properties;
            }
            return new JLevelPropertyEntry();
        }

        public static void RegisterLevelProperties(JLevelPropertyEntry properties)
        {
            RegisterLevelProperties(properties.sceneName, properties);
        }

        public static void RegisterLevelProperties(string name, JLevelPropertyEntry properties)
        {
            if (Registry.TryGetValue(name, out JLevelPropertyEntry old))
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

        public static Terminal GetTerminal()
        {
            if (Terminal == null)
            {
                Terminal = GameObject.FindObjectOfType<Terminal>();
            }
            return Terminal;
        }

        internal static void ApplyLevelOverrides()
        {
            EntranceTeleport[] entrances = Object.FindObjectsOfType<EntranceTeleport>(includeInactive: false);
            EntranceTeleports.AddRange(entrances);
            IsLevelGenerated = true;

            SelectableLevel currentLevel = RoundManager.Instance.currentLevel;
            JLevelPropertyEntry currentProperties = GetLevelProperties(currentLevel);

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
                EnemyType? target = GetRegisteredEnemy(property.enemyName);

                if (target != null)
                {
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
                }

                originalValues.Add(og);
            }

            original.enemyPropertyOverrides = originalValues.ToArray();
        }

        internal static void RemoveLevelOverrides()
        {
            IsLevelGenerated = false;
            EntranceTeleports.Clear();

            foreach (EnemyPropertyOverride property in original.enemyPropertyOverrides)
            {
                EnemyType? target = GetRegisteredEnemy(property.enemyName);

                if (target != null)
                {
                    if (property.MaxCount >= 0)
                    {
                        target.MaxCount = property.MaxCount;
                    }
                    if (property.PowerLevel >= 0)
                    {
                        target.PowerLevel = property.PowerLevel;
                    }
                }
            }

            original = new JLevelPropertyEntry();
        }

        public static EnemyType? GetRegisteredEnemy(string enemyName)
        {
            for (int i = 0; i < AllSortedEnemies.Count; i++)
            {
                if (AllSortedEnemies[i].enemyName == enemyName)
                {
                    return AllSortedEnemies[i];
                }
            }
            return null;
        }

        public static EnemyType GetRegisteredEnemy(EnemyType enemyType)
        {
            if (GetRegisteredEnemy(enemyType.enemyName, out EnemyType match))
            {
                return match;
            }
            return enemyType;
        }

        public static bool GetRegisteredEnemy(string enemyName, out EnemyType match)
        {
            JLogHelper.LogInfo($"Searching registry for {enemyName}", JLogLevel.Wesley);
            for (int i = 0; i < AllSortedEnemies.Count; i++)
            {
                if (AllSortedEnemies[i].enemyName == enemyName)
                {
                    match = AllSortedEnemies[i];
                    return true;
                }
            }
            match = AllSortedEnemies[0];
            return false;
        }
    }
}
