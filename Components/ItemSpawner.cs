using JLL.API;
using JLL.API.LevelProperties;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static JLL.Components.EnemySpawner;

namespace JLL.Components
{
    public class ItemSpawner : MonoBehaviour
    {
        [FormerlySerializedAs("spawnOnAwake")]
        public bool spawnOnEnabled = true;
        public RotationType spawnRotation = RotationType.NoRotation;
        public SpawnPoolSource SourcePool = SpawnPoolSource.CustomList;
        public WeightedItemRefrence[] CustomList = new WeightedItemRefrence[1] { new WeightedItemRefrence() };

        [Serializable]
        public class WeightedItemRefrence : IWeightedItem
        {
            [Range(0,100)]
            public int Weight = 20;
            public string ItemName = "";
            public Item Item;
            public bool FindRegisteredItem = true;
            public bool OverrideValue = false;
            public int ScrapValue = 0;
            public Vector3 SpawnOffset = Vector3.zero;

            public int GetWeight()
            {
                return Weight;
            }

            public void FindRegistered()
            {
                if (ItemName != "")
                {
                    foreach (Item registeredItem in RoundManager.Instance.playersManager.allItemsList.itemsList)
                    {
                        if (ItemName == registeredItem.itemName)
                        {
                            Item = registeredItem;
                            break;
                        }
                    }
                }
                else if (FindRegisteredItem && Item != null)
                {
                    foreach (Item registeredItem in RoundManager.Instance.playersManager.allItemsList.itemsList)
                    {
                        if (Item.itemName == registeredItem.itemName)
                        {
                            Item = registeredItem;
                            break;
                        }
                    }
                }
            }
        }

        private bool checkRegistry = true;

        public void Awake()
        {
            if (checkRegistry)
            {
                foreach (WeightedItemRefrence refrence in CustomList)
                {
                    refrence.FindRegistered();
                }
                checkRegistry = false;
            }
        }

        public void OnEnable()
        {
            if (spawnOnEnabled)
            {
                SpawnRandom();
            }
        }

        public static Item? GetRandomItem(SpawnPoolSource source, out int overrideValue, out Vector3 offset, WeightedItemRefrence[]? weightedItems = null)
        {
            Item? randomItem = null;
            overrideValue = -1;
            offset = Vector3.zero;
            switch (source)
            {
                case SpawnPoolSource.CustomList:
                    if (weightedItems != null)
                    {
                        if (weightedItems.Length == 0) break;
                        WeightedItemRefrence weightedItem = weightedItems.GetWeightedRandom();
                        randomItem = weightedItem.Item;
                        if (weightedItem.OverrideValue)
                        {
                            overrideValue = weightedItem.ScrapValue;
                        }
                        offset = weightedItem.SpawnOffset;
                    }
                    break;
                case SpawnPoolSource.AllItems:
                    randomItem = StartOfRound.Instance.allItemsList.itemsList[UnityEngine.Random.Range(0, StartOfRound.Instance.allItemsList.itemsList.Count)];
                    break;
                case SpawnPoolSource.LevelItems:
                    List<WeightedItemRefrence> levelItems = new List<WeightedItemRefrence>();
                    foreach (var itemRarity in RoundManager.Instance.currentLevel.spawnableScrap)
                    {
                        levelItems.Add(new WeightedItemRefrence() { Item = itemRarity.spawnableItem, Weight = itemRarity.rarity });
                    }
                    randomItem = levelItems.GetWeightedRandom().Item;
                    break;
                case SpawnPoolSource.StoreItems:
                    Terminal terminal = JLevelPropertyRegistry.GetTerminal();
                    if (terminal != null)
                    {
                        randomItem = terminal.buyableItemsList[UnityEngine.Random.Range(0, terminal.buyableItemsList.Length)];
                    }
                    break;
            }
            return randomItem;
        }

        public void SpawnRandom(int count = 1)
        {
            SpawnRandomItems(SourcePool, transform.position, RoundManager.Instance.spawnedScrapContainer, transform.rotation, CustomList, count: count, rotation: spawnRotation);
        }

        public void SpawnRandom(MonoBehaviour target)
        {
            SpawnRandom(target.transform.position);
        }

        public void SpawnRandom(GameObject target)
        {
            SpawnRandom(target.transform.position);
        }

        public void SpawnRandom(Vector3 pos)
        {
            SpawnRandomItems(SourcePool, pos, RoundManager.Instance.spawnedScrapContainer, transform.rotation, CustomList, count: 1, rotation: spawnRotation);
        }

        // Returned list contains spawned items and override values on server and is empty on client.
        public static List<KeyValuePair<GrabbableObject, int>> SpawnRandomItems(SpawnPoolSource sourcePool, Vector3 position, Transform parent, Quaternion sourceRot, WeightedItemRefrence[]? customList = null, Vector3[]? offsets = null, int count = 1, bool spawnOnNetwork = true, RotationType rotation = RotationType.NoRotation)
        {
            List<KeyValuePair<GrabbableObject, int>> grabbableObjects = new List<KeyValuePair<GrabbableObject, int>>();
            if (RoundManager.Instance.IsServer || RoundManager.Instance.IsHost)
            {
                customList ??= new WeightedItemRefrence[0];
                offsets ??= new Vector3[0];

                for (int i = 0; i < count; i++)
                {
                    Item? itemToSpawn = GetRandomItem(sourcePool, out int overrideValue, out Vector3 offset, customList);
                    if (itemToSpawn != null && itemToSpawn.spawnPrefab != null)
                    {
                        if (offsets.Length > i)
                        {
                            offset += offsets[i];
                        }
                        GrabbableObject? spawned = SpawnItem(itemToSpawn, position + offset, parent, sourceRot, overrideValue, spawnOnNetwork, rotation: rotation);
                        if (spawned != null)
                        {
                            grabbableObjects.Add(new KeyValuePair<GrabbableObject, int>(spawned, overrideValue));
                        }
                    }
                }
            }
            return grabbableObjects;
        }

        public static GrabbableObject? SpawnItem(Item item, Vector3 pos, Transform? parent, Quaternion sourceRot, int overrideValue = -1, bool spawnOnNetwork = true, RotationType rotation = RotationType.NoRotation)
        {
            JLogHelper.LogInfo($"Spawn on network: {spawnOnNetwork}", JLogLevel.Wesley);
            if (RoundManager.Instance.IsServer || RoundManager.Instance.IsHost)
            {
                GrabbableObject grabbable = Instantiate(item.spawnPrefab, pos, GetRot(rotation, sourceRot), parent).GetComponent<GrabbableObject>();
                grabbable.fallTime = 0f;
                if (spawnOnNetwork)
                {
                    JLogHelper.LogInfo("Spawning item on network.", JLogLevel.Wesley);
                    grabbable.NetworkObject.Spawn();
                    OverrideScrapValue(grabbable, overrideValue);
                }
                return grabbable;
            }
            return null;
        }

        public static void OverrideScrapValue(GrabbableObject grabbable, int overrideValue = -1)
        {
            if (overrideValue >= 0)
            {
                JLLNetworkManager.Instance.UpdateScanNodeServerRpc(grabbable.NetworkObject, overrideValue);
            }
            else
            {
                JLLNetworkManager.Instance.UpdateScanNodeServerRpc(grabbable.NetworkObject, Mathf.RoundToInt(UnityEngine.Random.Range(grabbable.itemProperties.minValue, grabbable.itemProperties.maxValue) * RoundManager.Instance.scrapValueMultiplier));
            }
        }
    }
}
