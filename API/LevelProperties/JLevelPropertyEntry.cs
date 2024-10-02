using System;
using System.Collections.Generic;
using System.Linq;

namespace JLL.API.LevelProperties
{
    [Serializable]
    public class JLevelPropertyEntry
    {
        public string sceneName = "";
        public LevelPrefab[] levelPrefabs = new LevelPrefab[0];
        public EnemyPropertyOverride[] enemyPropertyOverrides = new EnemyPropertyOverride[0];

        public void MergeWith(JLevelPropertyEntry other)
        {
            List<EnemyPropertyOverride> enemyOverrides = enemyPropertyOverrides.ToList();
            enemyOverrides.AddRange(other.enemyPropertyOverrides);
            enemyPropertyOverrides = enemyOverrides.ToArray();

            List<LevelPrefab> prefabs = levelPrefabs.ToList();
            prefabs.AddRange(other.levelPrefabs);
            levelPrefabs = prefabs.ToArray();
        }
    }
}
