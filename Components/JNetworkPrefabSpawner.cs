using JLL.ScriptableObjects;
using Unity.Netcode;
using UnityEngine;

namespace JLL.Components
{
    public class JNetworkPrefabSpawner : MonoBehaviour
    {
        public JNetworkPrefabSet PrefabSet;

        [Header("Spawn Triggers")]
        public bool spawnOnEnable = false;

        [Header("Prefab Spawn Settings")]
        public bool spawnRandom = false;
        public string spawnPrefabName;
        [Tooltip("Spawns the prefab as a child of the spawner. When disabled will spawn it inside the level's map props container.")]
        public bool spawnAsChild = true;

        void OnEnable()
        {
            if (spawnOnEnable) SpawnPrefab();
        }

        public void SpawnPrefab()
        {
            if (RoundManager.Instance.IsServer || RoundManager.Instance.IsHost)
            {
                GameObject? prefabToSpawn = null;
                if (spawnRandom)
                {
                    prefabToSpawn = PrefabSet.GetRandomPrefab();
                }
                else
                {
                    if (PrefabSet.GetPrefab(spawnPrefabName, out GameObject prefab))
                    {
                        prefabToSpawn = prefab;
                    }
                }

                if (prefabToSpawn != null)
                {
                    GameObject spawned = Instantiate(prefabToSpawn, transform.position, transform.rotation, spawnAsChild ? transform : RoundManager.Instance.mapPropsContainer.transform);

                    if (spawned.TryGetComponent(out NetworkObject netObj))
                    {
                        netObj.Spawn();
                    }
                }
            }
        }
    }
}
