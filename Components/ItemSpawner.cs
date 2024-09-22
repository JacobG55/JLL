using JLL.API.LevelProperties;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace JLL.Components
{
    public class ItemSpawner : MonoBehaviour
    {
        public SpawnPoolSource SourcePool = SpawnPoolSource.CustomList;
        public WeightedItemRefrence[] CustomList = new WeightedItemRefrence[0];
        [FormerlySerializedAs("spawnOnAwake")]
        public bool spawnOnEnabled = true;

        [Serializable]
        public class WeightedItemRefrence : IWeightedItem
        {
            [Range(0,100)]
            public int Weight = 20;
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
                if (FindRegisteredItem && Item != null)
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

        [Serializable]
        public enum SpawnPoolSource
        {
            CustomList,
            AllItems,
            LevelItems,
            StoreItems,
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
                        WeightedItemRefrence weightedItem = weightedItems[IWeightedItem.GetRandomIndex(weightedItems)];
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
                    randomItem = levelItems[IWeightedItem.GetRandomIndex(levelItems.ToArray())].Item;
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
            SpawnRandomItems(SourcePool, transform.position, RoundManager.Instance.spawnedScrapContainer, CustomList, count: count);
        }

        // Returned list contains spawned items on server and is empty on client.
        public static List<GrabbableObject> SpawnRandomItems(SpawnPoolSource sourcePool, Vector3 position, Transform parent, WeightedItemRefrence[]? customList = null, Vector3[]? offsets = null, int count = 1, bool spawnOnNetwork = true)
        {
            List<GrabbableObject> grabbableObjects = new List<GrabbableObject>();
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
                        GrabbableObject? spawned = SpawnItem(itemToSpawn, position + offset, parent, overrideValue, spawnOnNetwork);
                        if (spawned != null)
                        {
                            grabbableObjects.Add(spawned);
                        }
                    }
                }
            }
            return grabbableObjects;
        }

        public static GrabbableObject? SpawnItem(Item item, Vector3 pos, Transform? parent, int overrideValue = -1, bool spawnOnNetwork = true)
        {
            if (RoundManager.Instance.IsServer || RoundManager.Instance.IsHost)
            {
                GrabbableObject grabbable = Instantiate(item.spawnPrefab, pos, Quaternion.identity, parent).GetComponent<GrabbableObject>();
                grabbable.fallTime = 0f;
                OverrideScrapValue(grabbable, overrideValue);
                if (spawnOnNetwork)
                {
                    grabbable.NetworkObject.Spawn();
                }
                return grabbable;
            }
            return null;
        }

        public static void OverrideScrapValue(GrabbableObject grabbable, int overrideValue = -1)
        {
            if (overrideValue >= 0)
            {
                grabbable.scrapValue = overrideValue;
            }
            else
            {
                grabbable.scrapValue = Mathf.RoundToInt(UnityEngine.Random.Range(grabbable.itemProperties.minValue, grabbable.itemProperties.maxValue) * RoundManager.Instance.scrapValueMultiplier);
            }
        }
    }
}
