using JLL.API.Events;
using JLL.API.LevelProperties;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace JLL.Components
{
    public class EnemySpawner : MonoBehaviour
    {
        public EnemyType type;
        [Tooltip("Ran after an enemy spawns. Enemy provided is the one that was just spawned.")]
        public EnemyEvent SpawnedEvent = new EnemyEvent();

        [Header("Randomized")]
        [Tooltip("Determines weather to spawn a random enemy from the pool or to spawn the specified type")]
        public bool spawnRandom = false;
        public SpawnableEnemyWithRarity[] randomPool = new SpawnableEnemyWithRarity[0];
        private readonly List<SpawnableEnemyWithRarity> spawnList = new List<SpawnableEnemyWithRarity>();

        [Header("Object Enabled")]
        [FormerlySerializedAs("spawnOnAwake")]
        public bool spawnOnEnable = false;

        private bool checkRegistry = true;

        public void Awake()
        {
            if (checkRegistry)
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
                checkRegistry = false;
            }
        }

        public void OnEnable()
        {
            if (spawnOnEnable)
            {
                StartCoroutine(SpawnNextFixedUpdate());

                SpawnEnemy();
            }
        }

        private IEnumerator SpawnNextFixedUpdate()
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            SpawnEnemy();
        }

        public void SpawnEnemy()
        {
            if (RoundManager.Instance.IsHost || RoundManager.Instance.IsServer)
            {
                EnemyType? spawn;

                if (spawnRandom)
                {
                    spawn = spawnList[GetWeightedIndex()].enemyType;
                }
                else
                {
                    spawn = type;
                }

                if (spawn != null)
                {
                    if (spawn.enemyPrefab != null)
                    {
                        GameObject obj = RoundManager.Instance.SpawnEnemyGameObject(transform.position, transform.rotation.y, 0, spawn);
                        if (obj.TryGetComponent(out EnemyAI enemy))
                        {
                            SpawnedEvent.Invoke(enemy);
                        }
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
