using JLL.API.LevelProperties;
using System.Collections.Generic;
using UnityEngine;

namespace JLL.Components
{
    public class EnemySpawner : MonoBehaviour
    {
        public EnemyType type;

        [Header("Randomized")]
        public bool spawnRandom = false;
        public SpawnableEnemyWithRarity[] randomPool = new SpawnableEnemyWithRarity[0];
        private List<SpawnableEnemyWithRarity> spawnList = new List<SpawnableEnemyWithRarity>();

        [Header("Awake")]
        public bool spawnOnAwake = false;

        public void Start()
        {
            if (type != null)
            {
                type = JLevelPropertyRegistry.GetRegisteredEnemy(type);
            }

            if (randomPool.Length > 0)
            {
                for (int i = 0; i < randomPool.Length; i++)
                {
                    SpawnableEnemyWithRarity spawnableRarity = new SpawnableEnemyWithRarity
                    {
                        enemyType = JLevelPropertyRegistry.GetRegisteredEnemy(randomPool[i].enemyType),
                        rarity = randomPool[i].rarity
                    };
                    spawnList.Add(spawnableRarity);
                }
            }
            else
            {
                for (int i = 0; i < JLevelPropertyRegistry.AllSortedEnemies.Count; i++)
                {
                    SpawnableEnemyWithRarity spawnableRarity = new SpawnableEnemyWithRarity
                    {
                        enemyType = JLevelPropertyRegistry.AllSortedEnemies[i],
                        rarity = 10
                    };
                    spawnList.Add(spawnableRarity);
                }
            }
        }

        public void Awake()
        {
            if (spawnOnAwake)
            {
                SpawnEnemy();
            }
        }

        public void SpawnEnemy()
        {
            if (RoundManager.Instance.IsHost || RoundManager.Instance.IsServer)
            {
                if (spawnRandom)
                {
                    RoundManager.Instance.SpawnEnemyGameObject(transform.position, transform.rotation.y, 0, spawnList[GetWeightedIndex()].enemyType);
                }
                else
                {
                    if (type != null)
                    {
                        RoundManager.Instance.SpawnEnemyGameObject(transform.position, transform.rotation.y, 0, type);
                    }
                }
            }
        }

        private int GetWeightedIndex()
        {
            int combinedWeights = 0;
            for (int i = 0; i < spawnList.Count; i++)
            {
                combinedWeights += spawnList[i].rarity;
            }
            int random = Random.Range(0, combinedWeights);
            for (int i = 0; i < spawnList.Count; i++)
            {
                random -= spawnList[i].rarity;
                if (random <= 0)
                {
                    return i;
                }
            }
            return Random.Range(0, spawnList.Count);
        }
    }
}
