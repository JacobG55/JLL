using GameNetcodeStuff;
using JLL.API;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace JLL.Components
{
    public interface IWeightedItem
    {
        public int GetWeight();

        public static int GetRandomIndex(IWeightedItem[] weightedItems)
        {
            int combinedWeights = 0;
            for (int i = 0; i < weightedItems.Length; i++)
            {
                combinedWeights += weightedItems[i].GetWeight();
            }
            int random = UnityEngine.Random.Range(0, combinedWeights + 1);
            for (int i = 0; i < weightedItems.Length; i++)
            {
                random -= weightedItems[i].GetWeight();
                if (random <= 0)
                {
                    return i;
                }
            }
            return UnityEngine.Random.Range(0, weightedItems.Length);
        }
    }

    public class RandomizedEvent : NetworkBehaviour
    {
        [FormerlySerializedAs("triggerOnAwake")]
        [FormerlySerializedAs("triggerOnEnable")]
        public bool triggerOnStart = false;

        public WeightedEvent[] weightedEvents = new WeightedEvent[1] { new WeightedEvent() { Weight = 20 } };

        [Header("Triggered by StartRandomPlayerEvent() using a random player in the lobby")]
        [Tooltip("Event run on a random player in the lobby after StartRandomPlayerEvent() is called by another event.")]
        public InteractEvent RandomPlayerEvent = new InteractEvent();

        [Header("Advanced")]
        public bool isNetworkSynced = true;

        [Serializable]
        public class WeightedEvent : IWeightedItem
        {
            public UnityEvent Event = new UnityEvent();
            public InteractEvent PlayerEvent = new InteractEvent();

            [Range(0f, 100f)]
            public int Weight = 20;
            
            public bool SendClientRPC = true;

            public int GetWeight()
            {
                return Weight;
            }
        }
        
        public void Start()
        {
            if (triggerOnStart)
            {
                RollEvent();
            }
        }

        private IEnumerator RollNextFixedUpdate()
        {
            yield return new WaitForFixedUpdate();
            RollEvent();
        }

        public void RollEvent()
        {
            JLogHelper.LogInfo($"{name} starting random roll!");
            if (isNetworkSynced)
            {
                RollRandomServerRpc(-1);
            }
            else
            {
                TriggerEvent(IWeightedItem.GetRandomIndex(weightedEvents), -1);
            }
        }

        public void RollPlayerEvent(PlayerControllerB player)
        {
            if (isNetworkSynced)
            {
                RollRandomServerRpc((int)player.actualClientId);
            }
            else
            {
                TriggerEvent(IWeightedItem.GetRandomIndex(weightedEvents), (int)player.actualClientId);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void RollRandomServerRpc(int playerId)
        {
            if (weightedEvents.Length > 0)
            {
                int random = IWeightedItem.GetRandomIndex(weightedEvents);
                JLogHelper.LogInfo($"{name} - Server Generated: {random}");
                if (weightedEvents[random].SendClientRPC)
                {
                    RollResultClientRpc(random, playerId);
                }
                else
                {
                    TriggerEvent(random, playerId);
                }
            }
        }

        [ClientRpc]
        private void RollResultClientRpc(int value, int playerId)
        {
            JLogHelper.LogInfo($"{name} - Client Received: {value}");
            TriggerEvent(value, playerId);
        }

        private void TriggerEvent(int value, int playerId = -1)
        {
            weightedEvents[value].Event.Invoke();
            if (playerId != -1)
            {
                weightedEvents[value].PlayerEvent.Invoke(RoundManager.Instance.playersManager.allPlayerScripts[playerId]);
            }
        } 

        public void StartRandomPlayerEvent()
        {
            RandomPlayerEventServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void RandomPlayerEventServerRpc()
        {
            RandomPlayerEventClientRpc(UnityEngine.Random.Range(0, RoundManager.Instance.playersManager.allPlayerScripts.Length));
        }

        [ClientRpc]
        private void RandomPlayerEventClientRpc(int playerId)
        {
            RandomPlayerEvent.Invoke(RoundManager.Instance.playersManager.allPlayerScripts[Math.Clamp(playerId, 0, RoundManager.Instance.playersManager.allPlayerScripts.Length-1)]);
        }
    }
}
