using GameNetcodeStuff;
using JLL.API;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class JLineOfSight : NetworkBehaviour
    {
        public int range = 60;
        public float angle = 45f;

        public bool limitTriggers = false;
        public float interval = 0.5f;
        private float timer = 0f;
        private bool viewedLast = false;

        public UnityEvent<PlayerControllerB> OnViewed = new();
        public UnityEvent OnLookAway = new();

        public void OnEnable()
        {
            timer = interval;
        }

        public void Update()
        {
            if (!IsServer) return;

            timer -= Time.deltaTime;
            if (timer > 0) return;

            timer = Mathf.Max(0.05f, interval);
            bool viewed = false;
            foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
            {
                if (player.isPlayerControlled || player.isPlayerDead) continue;

                if (Vector3.Distance(player.transform.position, transform.position) < range)
                {
                    if (player.HasLineOfSightToPosition(transform.position, angle, range))
                    {
                        PlayerViewedRpc(player.Index());
                        viewed = true;
                        if (limitTriggers) break;
                    }
                }
            }

            if (viewedLast && !viewed)
            {
                OnLookAwayRpc();
            }
            viewedLast = viewed;
        }

        [Rpc(SendTo.Everyone, RequireOwnership = true)]
        public void PlayerViewedRpc(int playerIndex)
        {
            OnViewed.Invoke(StartOfRound.Instance.allPlayerScripts[playerIndex]);
        }

        [Rpc(SendTo.Everyone, RequireOwnership = true)]
        public void OnLookAwayRpc()
        {
            OnLookAway.Invoke();
        }
    }
}
