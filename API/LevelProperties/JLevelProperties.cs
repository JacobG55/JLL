using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JLL.API.LevelProperties
{
    [Serializable]
    public class JLevelProperties
    {
        public EnemyPropertyOverride[] enemyPropertyOverrides = new EnemyPropertyOverride[0];
        public LevelPrefab[] levelPrefabs = new LevelPrefab[0];

        public void MergeWith(JLevelProperties other)
        {
            List<EnemyPropertyOverride> enemyOverrides = enemyPropertyOverrides.ToList();
            enemyOverrides.AddRange(other.enemyPropertyOverrides);
            enemyPropertyOverrides = enemyOverrides.ToArray();

            List<LevelPrefab> prefabs = levelPrefabs.ToList();
            prefabs.AddRange(other.levelPrefabs);
            levelPrefabs = prefabs.ToArray();
        }
    }

    [Serializable]
    public class EnemyPropertyOverride
    {
        public EnemyType enemyType;

        public float PowerLevel = -1;
        public int MaxCount = -1;
    }

    [Serializable]
    public class LevelPrefab
    {
        public GameObject prefab;
        public Vector3 position;
        public Quaternion rotation;
    }

}
