using GameNetcodeStuff;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class JPlayerInsideRegion : NetworkBehaviour
    {
        public InteractEvent PlayerEnterEvent = new InteractEvent();

        [Header("Chance Event")]
        [Range(0, 100)]
        public float eventChance = 100f;
        public float chanceInterval = 1f;
        private float timeAtLastRoll = 0f;

        public UnityEvent ChanceEvent = new UnityEvent();

        [Header("Limit Triggers")]
        public bool limitEventTriggers = true;
        public int maxEventTriggers = 1;
        private int eventTriggers = 0;

        private readonly List<int> playersInside = new List<int>();
        private readonly List<int> foundInside = new List<int>();
        private readonly List<int> markRemoval = new List<int>();

        public void OnTriggerStay(Collider collider)
        {
            if (collider.gameObject.CompareTag("Player") && collider.gameObject.TryGetComponent(out PlayerControllerB player))
            {
                foundInside.Add((int)player.actualClientId);
                if (!playersInside.Contains((int)player.actualClientId))
                {
                    playersInside.Add((int)player.actualClientId);
                    PlayerEnterEvent.Invoke(player);
                }
            }
        }

        public void FixedUpdate()
        {
            for (int i = 0; i < playersInside.Count; i++)
            {
                if (!foundInside.Contains(playersInside[i]))
                {
                    markRemoval.Add(playersInside[i]);
                }
            }
            foundInside.Clear();

            for (int i = 0; i < markRemoval.Count; i++)
            {
                playersInside.Remove(markRemoval[i]);
            }
            markRemoval.Clear();
        }

        public void Update()
        {
            if (IsServer || IsHost) ChanceEventServerRpc();
        }

        [ServerRpc(RequireOwnership = true)]
        private void ChanceEventServerRpc()
        {
            if (playersInside.Count == 0) return;
            if (limitEventTriggers && eventTriggers >= maxEventTriggers) return;

            if (Time.realtimeSinceStartup - timeAtLastRoll > chanceInterval)
            {
                timeAtLastRoll = Time.realtimeSinceStartup;

                if (Random.Range(0f, 100f) < eventChance)
                {
                    eventTriggers++;
                    ChanceEventClientRpc();
                }
            }
        }

        [ClientRpc]
        private void ChanceEventClientRpc()
        {
            ChanceEvent.Invoke();
        }
    }
}
