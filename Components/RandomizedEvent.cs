using GameNetcodeStuff;
using JLL.API;
using JLL.API.Events;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace JLL.Components
{
    public class RandomizedEvent : NetworkBehaviour
    {
        [FormerlySerializedAs("triggerOnAwake")]
        [FormerlySerializedAs("triggerOnEnable")]
        public bool triggerOnStart = false;

        public WeightedEvent[] weightedEvents = new WeightedEvent[1] { new WeightedEvent() { Weight = 20 } };

        [Header("Triggered by StartRandomPlayerEvent() using a random player in the lobby")]
        [Tooltip("Event run on a random player in the lobby after StartRandomPlayerEvent() is called by another event.")]
        public InteractEvent RandomPlayerEvent = new();

        [Header("Triggered by StartRandomNumberEvent() generating a random number between X & Y")]
        [Tooltip("Event run on all clients using a randomly generated number between two values.")]
        public FloatEvent RandomNumberEvent = new();
        [Tooltip("Event run on all clients using a randomly generated number between two values.")]
        public IntEvent RandomRoundedNumberEvent = new();

        [Header("Advanced")]
        public bool isNetworkSynced = true;

        [Serializable]
        public class WeightedEvent : IWeightedItem
        {
            public UnityEvent Event = new();
            public InteractEvent PlayerEvent = new();

            [Range(0f, 100f)]
            public int Weight = 20;
            
            public bool SendClientRPC = true;

            public int GetWeight()
            {
                return Weight;
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (triggerOnStart && !(isNetworkSynced && !(IsHost || IsServer)))
            {
                RollEvent();
            }
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
                RollRandomServerRpc(player.Index());
            }
            else
            {
                TriggerEvent(IWeightedItem.GetRandomIndex(weightedEvents), player.Index());
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

        public void StartRandomNumberEvent(Vector2 minMax)
        {
            RandomNumberEventServerRpc(minMax);
        }

        [ServerRpc(RequireOwnership = false)]
        private void RandomNumberEventServerRpc(Vector2 minMax)
        {
            if (minMax.x > minMax.y) return;
            RandomNumberEventClientRpc(minMax.x == minMax.y ? minMax.x : UnityEngine.Random.Range(minMax.x, minMax.y));
        }

        [ClientRpc]
        private void RandomNumberEventClientRpc(float number)
        {
            RandomNumberEvent.Invoke(number);
            RandomRoundedNumberEvent.Invoke(Mathf.RoundToInt(number));
        }
    }
}
