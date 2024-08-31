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

            [Range(0f, 100f)]
            public int Weight = 20;
        }
        
        public void Awake()
        {
            if (triggerOnAwake)
            {
                RollEvent();
            }
        }

        public void RollEvent()
        {
            RollRandomServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void RollRandomServerRpc()
        {
            if (weightedEvents.Length > 0)
            {
                int random = GetWeightedIndex();
                JLL.Instance.mls.LogInfo($"Server Generated: {random}");
                RollResultClientRpc(random);
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
        private void RollResultClientRpc(int value)
        {
            JLL.Instance.mls.LogInfo($"Client Received: {value}");
            weightedEvents[value].Event.Invoke();
        }
    }
}
