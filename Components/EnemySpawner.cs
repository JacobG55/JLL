using JLL.API;
using JLL.API.Events;
using JLL.API.LevelProperties;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace JLL.Components
{
    public class EnemySpawner : MonoBehaviour
    {
        [Tooltip("Determines weather to spawn a random enemy from the pool or to spawn the specified type")]
        public bool spawnRandom = false;
        public RotationType spawnRotation = RotationType.ObjectRotation;
        public List<SpawnableEnemyWithRarity> randomPool = new List<SpawnableEnemyWithRarity>();

        public EnemyType type;
        [Tooltip("Ran after an enemy spawns. Enemy provided is the one that was just spawned.")]
        public EnemyEvent SpawnedEvent = new EnemyEvent();

        [Header("NavMesh")]
        [Tooltip("The max distance from this transform that a navmesh will be found")]
        public float navMeshRange = 10f;

        [Header("Misc")]
        [FormerlySerializedAs("spawnOnAwake")]
        public bool spawnOnEnable = false;
        public bool checkRegistry = true;

        [Serializable]
        public enum RotationType
        {
            ObjectRotation,
            RandomRotation,
            NoRotation,
        }

        public static float GetRot(RotationType rotation, Transform refrence)
        {
            return rotation switch
            {
                RotationType.ObjectRotation => refrence.rotation.y,
                RotationType.RandomRotation => UnityEngine.Random.Range(0f, 360f),
                _ => 0,
            };
        }

        public void Awake()
        {
            if (checkRegistry)
            {
                if (type != null)
                {
                    type = JLevelPropertyRegistry.GetRegisteredEnemy(type);
                }

                if (randomPool.Count > 0)
                {
                    for (int i = 0; i < randomPool.Count; i++)
                    {
                        if (randomPool[i].enemyType != null)
                        {
                            randomPool[i].enemyType = JLevelPropertyRegistry.GetRegisteredEnemy(randomPool[i].enemyType);
                        }
                        else
                        {
                            JLogHelper.LogWarning($"({name}) Enemy spawner is missing enemy at {i}", JLogLevel.Debuging);
                        }
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
                        randomPool.Add(spawnableRarity);
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
            }
        }

        private IEnumerator SpawnNextFixedUpdate()
        {
            yield return new WaitForFixedUpdate();
            SpawnEnemy();
        }

        public void SpawnEnemy()
        {
            SpawnEnemy(transform.position);
        }

        public void SpawnEnemy(GameObject target)
        {
            SpawnEnemy(target.transform.position);
        }

        public void SpawnEnemy(MonoBehaviour target)
        {
            SpawnEnemy(target.transform.position);
        }

        public void SpawnEnemy(Vector3 pos)
        {
            if (RoundManager.Instance.IsHost || RoundManager.Instance.IsServer)
            {
                EnemyType? spawn;

                if (spawnRandom)
                {
                    int i = GetWeightedIndex();
                    JLogHelper.LogInfo($"({name}) Attempting to spawn enemy at index {i}");
                    spawn = randomPool[i].enemyType;
                }
                else
                {
                    spawn = type;
                }

                if (spawn != null)
                {
                    if (spawn.enemyPrefab != null)
                    {
                        bool flag;
                        if ((flag = NavMesh.SamplePosition(pos, out NavMeshHit hit, navMeshRange, NavMesh.AllAreas)) || navMeshRange < 0)
                        {
                            JLogHelper.LogInfo($"({name}) Spawning: {spawn.enemyName} at {pos}");
                            GameObject obj = RoundManager.Instance.SpawnEnemyGameObject(flag ? hit.position : pos, GetRot(spawnRotation, transform), 0, spawn);
                            if (obj.TryGetComponent(out EnemyAI enemy))
                            {
                                SpawnedEvent.Invoke(enemy);
                            }
                        }
                        else
                        {
                            JLogHelper.LogInfo($"({name}) Failed to spawn. (Couldn't Find NavMesh. If try increasing the EnemySpawner's navMeshDistance if you're having an issue.)");
                        }
                    }
                }
                else
                {
                    JLogHelper.LogWarning($"({name}) Enemy Spawner tried to spawn a null EnemyType!");
                }
            }
        }

        private int GetWeightedIndex()
        {
            int combinedWeights = 0;
            for (int i = 0; i < randomPool.Count; i++)
            {
                combinedWeights += randomPool[i].rarity;
            }
            int random = UnityEngine.Random.Range(0, combinedWeights);
            for (int i = 0; i < randomPool.Count; i++)
            {
                random -= randomPool[i].rarity;
                if (random <= 0)
                {
                    return i;
                }
            }
            return UnityEngine.Random.Range(0, randomPool.Count);
        }
    }
}
