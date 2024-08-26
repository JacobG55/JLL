using System.Collections.Generic;

namespace JLL.API.LevelProperties
{
    public class JLevelPropertyRegistry
    {
        private static readonly Dictionary<string, JLevelProperties> Registry = new Dictionary<string, JLevelProperties>();
        public static List<EnemyType> AllSortedEnemies = new List<EnemyType>();

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
            Registry.Add(name, properties);
        }

        internal static void ApplyLevelOverrides()
        {
            SelectableLevel currentLevel = RoundManager.Instance.currentLevel;
            JLevelProperties currentProperties = GetLevelProperties(currentLevel);

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
