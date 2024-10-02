using System;
using UnityEngine;

namespace JLL.API.LevelProperties
{
    [Serializable]
    public class EnemyPropertyOverride
    {
        [Header("Identifier")]
        public string enemyName = "";

        [Header("Properties")]
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
