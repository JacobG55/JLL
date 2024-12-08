using GameNetcodeStuff;
using JLL.Components;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static JLL.Components.ItemSpawner;
using static JLL.Components.RandomizedEvent;

namespace JLLItemsModule.Components
{
    public class JEventBoxItem : JGrabbableObject
    {
        [Header("Event Box")]
        public bool canOpenInOrbit = true;
        public int boxUses = 1;
        public bool destroyWhenEmpty = true;
        private float lastUsedTime = 0;
        public float openCooldown = 0.4f;

        public PlayerEvent OpenEvent = new PlayerEvent();
        public WeightedEvent[] RandomOpenEvent = new WeightedEvent[1] { new WeightedEvent() { Weight = 20 } };

        [Header("Item Spawner")]
        public bool spawnItemsOnOpen = true;
        public int numberToSpawn = 1;
        public SpawnPoolSource SourcePool = SpawnPoolSource.CustomList;
        public WeightedItemRefrence[] CustomList = new WeightedItemRefrence[1] { new WeightedItemRefrence() };

        [Header("Audio & FX")]
        public ParticleSystem? PoofParticle;
        public AudioSource? BoxAudio;
        public AudioClip? OpenBoxClip;

        private PlayerControllerB previousPlayerHeldBy;

        public override void Start()
        {
            base.Start();
            foreach (WeightedItemRefrence refrence in CustomList)
            {
                refrence.FindRegistered();
            }
        }

        public override void LoadItemSaveData(int saveData)
        {
            base.LoadItemSaveData(saveData);
            boxUses = saveData;
        }

        public override int GetItemDataToSave()
        {
            base.GetItemDataToSave();
            return boxUses;
        }

        public override void EquipItem()
        {
            base.EquipItem();
            previousPlayerHeldBy = playerHeldBy;
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (!canOpenInOrbit && StartOfRound.Instance.inShipPhase)
            {
                return;
            }
            if (playerHeldBy != null && boxUses > 0 && Time.realtimeSinceStartup - lastUsedTime > openCooldown)
            {
                lastUsedTime = Time.realtimeSinceStartup;
                playerHeldBy.activatingItem = true;
                if (IsOwner)
                {
                    OpenGiftBoxServerRpc();
                }
            }
        }

        public override void PocketItem()
        {
            base.PocketItem();
            playerHeldBy.activatingItem = false;
        }

        [ServerRpc(RequireOwnership = false)]
        public void OpenGiftBoxServerRpc()
        {
            boxUses--;
            int random = -1;
            if (RandomOpenEvent.Length > 0)
            {
                random = IWeightedItem.GetRandomIndex(RandomOpenEvent);
                if (!RandomOpenEvent[random].SendClientRPC)
                {
                    InvokeRandomEvent(random);
                    random = -1;
                }
            }
            if (spawnItemsOnOpen) SpawnItemsOnServer(numberToSpawn);
            OpenBoxClientRpc(random, boxUses);
        }

        public virtual void SpawnItemsOnServer(int amount)
        {
            SpawnItemsServerRpc(amount);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnItemsServerRpc(int amount)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.25f;
            Transform parent = (((!(playerHeldBy != null) || !playerHeldBy.isInElevator) && !StartOfRound.Instance.inShipPhase) || !(RoundManager.Instance.spawnedScrapContainer != null)) ? StartOfRound.Instance.elevatorTransform : RoundManager.Instance.spawnedScrapContainer;
            List<KeyValuePair<GrabbableObject, int>> spawned = SpawnRandomItems(SourcePool, spawnPos, parent, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), CustomList, count: amount, spawnOnNetwork: false);

            foreach (var spawnedItem in spawned)
            {
                spawnedItem.Key.startFallingPosition = spawnPos;
                StartCoroutine(SetObjectToHitGroundSFX(spawnedItem.Key));
                spawnedItem.Key.targetFloorPosition = spawnedItem.Key.GetItemFloorPosition(transform.position);
                if (previousPlayerHeldBy != null && previousPlayerHeldBy.isInHangarShipRoom)
                {
                    previousPlayerHeldBy.SetItemInElevator(droppedInShipRoom: true, droppedInElevator: true, spawnedItem.Key);
                }
                spawnedItem.Key.NetworkObject.Spawn();
                OverrideScrapValue(spawnedItem.Key, spawnedItem.Value);
            }
        }

        private IEnumerator SetObjectToHitGroundSFX(GrabbableObject gObject)
        {
            yield return new WaitForEndOfFrame();
            Debug.Log("Setting " + gObject.itemProperties.itemName + " hit ground to false");
            gObject.reachedFloorTarget = false;
            gObject.hasHitGround = false;
            gObject.fallTime = 0f;
        }

        [ClientRpc]
        private void OpenBoxClientRpc(int weightedEvent, int usesLeft)
        {
            if (PoofParticle != null)
            {
                PoofParticle.Play();
            }
            if (BoxAudio != null && OpenBoxClip != null)
            {
                BoxAudio.PlayOneShot(OpenBoxClip);
                WalkieTalkie.TransmitOneShotAudio(BoxAudio, OpenBoxClip);
                RoundManager.Instance.PlayAudibleNoise(BoxAudio.transform.position, 8f, 0.5f, 0, isInShipRoom && StartOfRound.Instance.hangarDoorsClosed);
            }
            boxUses = usesLeft;
            OnBoxOpen();
            InvokeRandomEvent(weightedEvent);
            if (playerHeldBy != null)
            {
                OpenEvent.Invoke(playerHeldBy);
                playerHeldBy.activatingItem = false;
                if (destroyWhenEmpty && boxUses <= 0)
                {
                    DestroyObjectInHand(playerHeldBy);
                }
            }
        }

        private void InvokeRandomEvent(int weightedEvent)
        {
            if (weightedEvent >= 0 && weightedEvent < RandomOpenEvent.Length)
            {
                RandomOpenEvent[weightedEvent].Event.Invoke();
                if (playerHeldBy != null) RandomOpenEvent[weightedEvent].PlayerEvent.Invoke(playerHeldBy);
            }
        }

        public virtual void OnBoxOpen()
        {

        }
    }
}
