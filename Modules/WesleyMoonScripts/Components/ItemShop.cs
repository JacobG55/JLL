using JLL.API;
using JLL.API.LevelProperties;
using JLL.Components;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using static JLL.Components.ItemSpawner;

namespace WesleyMoonScripts.Components
{
    public class ItemShop : NetworkBehaviour
    {
        public bool spawnOnStart = true;
        public float priceMultiplier = 1.2f;
        public bool setScrapValue = false;
        public string purchasedString = "Purchased!";
        public string emptyString = "Empty";
        public SaleSpot[] spawnPositions = new SaleSpot[1] { new SaleSpot() };
        public WeightedItemRefrence[] Catalogue = new WeightedItemRefrence[1] { new WeightedItemRefrence() };

        public AudioSource audioSource;
        public AudioClip buyClip;

        public float syncDelay = 2f;

        [System.Serializable]
        public class SaleSpot
        {
            public float emptyChance = 0.2f;
            public Transform transform;
            public InteractTrigger buyInteract;

            [HideInInspector] public int price;
            [HideInInspector] public GrabbableObject? item = null;
            [HideInInspector] public bool ready = false;
        }


        public void Start()
        {
            foreach (SaleSpot itemSpawn in spawnPositions)
            {
                itemSpawn.buyInteract.interactable = false;
                itemSpawn.buyInteract.disabledHoverTip = emptyString;
            }
            if (!(IsServer || IsHost)) return;

            foreach (WeightedItemRefrence refrence in Catalogue)
            {
                refrence.FindRegistered();
            }

            foreach (SaleSpot itemSpawn in spawnPositions)
            {
                if (itemSpawn.transform == null || itemSpawn.buyInteract == null || Random.Range(0f, 1f) < itemSpawn.emptyChance) continue;

                Item? itemToSpawn = GetRandomItem(SpawnPoolSource.CustomList, out int overrideValue, out Vector3 offset, Catalogue);
                if (itemToSpawn != null && itemToSpawn.spawnPrefab != null)
                {
                    GrabbableObject? spawned = SpawnItem(itemToSpawn, itemSpawn.transform.position + offset, itemSpawn.transform, itemSpawn.transform.rotation, spawnOnNetwork: false);
                    if (spawned != null)
                    {
                        itemSpawn.price = overrideValue < 0 ? Mathf.RoundToInt(Random.Range(spawned.itemProperties.minValue, spawned.itemProperties.maxValue) * RoundManager.Instance.scrapValueMultiplier) : overrideValue;
                        itemSpawn.item = spawned;

                        spawned.NetworkObject.Spawn();
                        continue;
                    }
                }
            }

            StartCoroutine(SyncItems());
        }

        private IEnumerator SyncItems()
        {
            yield return new WaitForSeconds(syncDelay);
            
            for (int i = 0; i < spawnPositions.Length; i++)
            {
                SaleSpot itemSpawn = spawnPositions[i];

                if (itemSpawn.item != null)
                {
                    OverrideScrapValue(itemSpawn.item, setScrapValue ? itemSpawn.price : 0);

                    itemSpawn.price = Mathf.RoundToInt(itemSpawn.price * priceMultiplier);

                    SyncBuyableClientRpc(i, itemSpawn.price, itemSpawn.item.NetworkObject);

                    itemSpawn.ready = true;
                }
            }
        }

        [ClientRpc]
        private void SyncBuyableClientRpc(int index, int price, NetworkObjectReference itemRef)
        {
            if (index < 0 || index >= spawnPositions.Length) return;
            SaleSpot saleSpot = spawnPositions[index];
            if (itemRef.TryGet(out NetworkObject netObj) && netObj.gameObject.TryGetComponent(out GrabbableObject item))
            {
                item.grabbable = item.grabbableToEnemies = false;
                saleSpot.price = price;
                saleSpot.item = item;

                saleSpot.buyInteract.interactable = true;
                saleSpot.buyInteract.hoverTip = $"{item.itemProperties.itemName} : {price}";
                saleSpot.buyInteract.onInteract.AddListener((player) => AttemptPurchaseServerRpc(index, (int)player.actualClientId));
            }
        }

        public void AttemptPurchase(int slot) => AttemptPurchaseServerRpc(slot);

        [ServerRpc(RequireOwnership = false)]
        private void AttemptPurchaseServerRpc(int slot, int client = -1)
        {
            if (slot < 0 || slot >= spawnPositions.Length) return;

            SaleSpot saleSpot = spawnPositions[slot];

            if (!saleSpot.ready || saleSpot.item == null) return;

            if (JLevelPropertyRegistry.GetTerminal().groupCredits >= saleSpot.price)
            {
                PurchaseClientRpc(slot, saleSpot.price, saleSpot.item.NetworkObject);
            }
            else FailPurchaseClientRpc(client);
        }

        [ClientRpc]
        private void PurchaseClientRpc(int slot, int price, NetworkObjectReference itemRef)
        {
            JLevelPropertyRegistry.GetTerminal().groupCredits -= price;
            if (itemRef.TryGet(out NetworkObject netObj) && netObj.gameObject.TryGetComponent(out GrabbableObject item))
            {
                item.grabbable = item.grabbableToEnemies = true;

                InteractTrigger interact = spawnPositions[slot].buyInteract;
                interact.interactable = false;
                interact.disabledHoverTip = purchasedString;
            }
            if (audioSource != null && buyClip != null) audioSource.PlayOneShot(buyClip);
        }

        [ClientRpc]
        private void FailPurchaseClientRpc(int clientId)
        {
            if ((int)StartOfRound.Instance.localPlayerController.actualClientId != clientId) return;

            JHudHelper.QueueDisplayTip("Error", "Not enough credits to purchase this item.", isWarning: true);
        }
    }
}
