using GameNetcodeStuff;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class RandomizedEvent : NetworkBehaviour
    {
        public bool triggerOnAwake = false;

        public WeightedEvent[] weightedEvents = new WeightedEvent[0];

        [Serializable]
        public class WeightedEvent
        {
            public UnityEvent Event = new UnityEvent();
            public InteractEvent PlayerEvent = new InteractEvent();

            [Range(0f, 100f)]
            public int Weight = 20;
        }

        public InteractEvent RandomPlayerEvent = new InteractEvent();
        
        public void Awake()
        {
            if (triggerOnAwake)
            {
                RollEvent();
            }
        }

        public void RollEvent()
        {
            RollRandomServerRpc(-1);
        }

        public void RollPlayerEvent(PlayerControllerB player)
        {
            RollRandomServerRpc((int)player.actualClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void RollRandomServerRpc(int playerId)
        {
            if (weightedEvents.Length > 0)
            {
                int random = GetWeightedIndex();
                JLL.Instance.mls.LogInfo($"Server Generated: {random}");
                RollResultClientRpc(random, playerId);
            }
        }

        private int GetWeightedIndex()
        {
            int combinedWeights = 0;
            for (int i = 0; i < weightedEvents.Length; i++)
            {
                combinedWeights += weightedEvents[i].Weight;
            }
            int random = UnityEngine.Random.Range(0, combinedWeights);
            for (int i = 0; i < weightedEvents.Length; i++)
            {
                random -= weightedEvents[i].Weight;
                if (random <= 0)
                {
                    return i;
                }
            }
            return UnityEngine.Random.Range(0, weightedEvents.Length);
        }

        [ClientRpc]
        private void RollResultClientRpc(int value, int playerId)
        {
            JLL.Instance.mls.LogInfo($"Client Received: {value}");
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
