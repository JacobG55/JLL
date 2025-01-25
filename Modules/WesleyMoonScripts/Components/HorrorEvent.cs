using Unity.Netcode;
using UnityEngine;
using JLL.API.LevelProperties;
using JLL.Components;
using UnityEngine.Events;

namespace WesleyMoonScripts.Components
{
    public class HorrorEvent : NetworkBehaviour
    {
        [Header("Chances")]
        [Range(0f, 100f)]
        public float StandardChance = 2f;
        [Range(0f, 100f)]
        public float MeltdownChance = 20f;

        [Header("Validation")]
        public float MaxPlayerDistance = 15f;
        public float MinPlayerDistance = 0f;
        public bool RequireLineOfSight = false;
        public LayerMask RaycastMask = 1084754248;

        public float RollInterval = 25f;
        private float RollTimer = 0f;

        public int repeatLimit = 0;
        private bool finished = false;
        public float cooldown = 60f;

        [Header("EventTrigger")]
        public Animator? animator;
        public WeightedHorrorEvent[] horrorEvents = new WeightedHorrorEvent[0];

        [System.Serializable]
        public class WeightedHorrorEvent : IWeightedItem
        {
            public string AnimationTrigger = "";
            public UnityEvent Event = new UnityEvent();

            [Range(0f, 100f)]
            public int Weight = 20;
            public int GetWeight() => Weight;
        }

        public void Start()
        {
            if (cooldown < 0) cooldown = -cooldown;
            RollTimer = Random.Range(-cooldown, 0);
        }

        public void Update()
        {
            if (!(IsServer || IsHost) || finished) return;

            RollTimer += Time.deltaTime;
            if (RollTimer > RollInterval)
            {
                RollTimer -= RollInterval;
                bool valid = false;
                foreach (var player in RoundManager.Instance.playersManager.allPlayerObjects)
                {
                    float playerDistance = Vector3.Distance(transform.position, player.transform.position);
                    if (playerDistance < MaxPlayerDistance && playerDistance >= MinPlayerDistance)
                    {
                        if (RequireLineOfSight && Physics.Raycast(transform.position, player.transform.position, out RaycastHit hit, MaxPlayerDistance, RaycastMask) && hit.collider.gameObject.layer != 3) continue;
                        valid = true;
                        break;
                    }
                }
                if (valid && Random.Range(0, 100) < (JLevelPropertyRegistry.ApparatusPulled ? MeltdownChance : StandardChance))
                {
                    TriggerHorrorEventServerRpc();
                }
            }
        }

        [ServerRpc]
        private void TriggerHorrorEventServerRpc()
        {
            if (horrorEvents.Length > 0)
            {
                TriggerHorrorEventClientRpc(IWeightedItem.GetRandomIndex(horrorEvents));
            }
        }

        [ClientRpc]
        private void TriggerHorrorEventClientRpc(int index)
        {
            if (horrorEvents.Length > 0 && index >= 0 && index < horrorEvents.Length)
            {
                WeightedHorrorEvent Event = horrorEvents[index];
                if (animator != null && !string.IsNullOrEmpty(Event.AnimationTrigger))
                {
                    animator.ResetTrigger(Event.AnimationTrigger);
                    animator.SetTrigger(Event.AnimationTrigger);
                }
                Event.Event.Invoke();

                repeatLimit--;
                if (repeatLimit < 0)
                {
                    finished = true;
                }
                else
                {
                    RollTimer = -cooldown;
                }
            }
        }
    }
}
