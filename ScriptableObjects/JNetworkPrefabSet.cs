using JLL.API;
using JLL.Components;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace JLL.ScriptableObjects
{
    [CreateAssetMenu(menuName = "JLL/JNetworkPrefabSet")]
    public class JNetworkPrefabSet : ScriptableObject
    {
        public static readonly List<JNetworkPrefabSet> NetworkPrefabSets = new List<JNetworkPrefabSet>();
        public static GameObject EmptyNetworkObject { get; internal set; }

        public string SetName;
        public JIdentifiablePrefab[] prefabList = new JIdentifiablePrefab[1] { new JIdentifiablePrefab() };

        [Serializable]
        public class JIdentifiablePrefab : IWeightedItem
        {
            public string name;
            public GameObject prefab;

            [Range(0f, 100f)]
            public int Weight = 20;

            public int GetWeight()
            {
                return Weight;
            }
        }

        internal static void RegisterPrefabs()
        {
            NetworkManager NetworkManager = GameNetworkManager.Instance.GetComponent<NetworkManager>();
            List<GameObject> RegisteredNetworkPrefabs = new List<GameObject>();

            foreach (NetworkPrefab netPrefab in NetworkManager.NetworkConfig.Prefabs.Prefabs)
            {
                RegisteredNetworkPrefabs.Add(netPrefab.Prefab);
            }

            int registeredPrefabs = 0;
            
            foreach (var NetworkPrefabSet in NetworkPrefabSets)
            {
                for (int i = 0; i < NetworkPrefabSet.prefabList.Length; i++)
                {
                    if (NetworkPrefabSet.prefabList[i].prefab == null) continue;

                    if (!RegisteredNetworkPrefabs.Contains(NetworkPrefabSet.prefabList[i].prefab))
                    {
                        NetworkManager.AddNetworkPrefab(NetworkPrefabSet.prefabList[i].prefab);
                        registeredPrefabs++;
                        JLogHelper.LogInfo($"Registered Network Object: {NetworkPrefabSet.prefabList[i].name} ({NetworkPrefabSet.prefabList[i].prefab.name})", JLogLevel.Wesley);
                    }
                }
            }
            JLogHelper.LogInfo($"Registered {registeredPrefabs} Network Objects");
        }

        public GameObject GetPrefab(string name)
        {
            for (int i = 0; i < prefabList.Length; i++)
            {
                if (prefabList[i].name == name)
                {
                    return prefabList[i].prefab;
                }
            }
            return EmptyNetworkObject;
        }

        public bool GetPrefab(string name, out GameObject prefab)
        {
            for (int i = 0; i < prefabList.Length; i++)
            {
                if (prefabList[i].name == name)
                {
                    prefab = prefabList[i].prefab;
                    return true;
                }
            }
            prefab = EmptyNetworkObject;
            return false;
        }

        public GameObject GetRandomPrefab()
        {
            return prefabList[IWeightedItem.GetRandomIndex(prefabList)].prefab;
        }
    }
}
