using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class JClientSync : NetworkBehaviour
    {
        /*
        [Header("Synced Filter")]
        [SerializeField, SerializeReference]
        public IJFilter hostFilter;
        public bool runFilterOnStart = false;
        public UnityEvent SyncedFilterSuccess = new UnityEvent();
        public UnityEvent SyncedFilterFailure = new UnityEvent();
        */

        [Header("Host Only")]
        public bool runHostEventOnStart = false;
        public UnityEvent hostEvent = new UnityEvent();
        public InteractEvent hostPlayerEvent = new InteractEvent();

        [Header("Synced")]
        public bool runSyncedEventOnStart = false;
        public UnityEvent syncedEvent = new UnityEvent();
        public InteractEvent syncedPlayerEvent = new InteractEvent();

        public void Start()
        {
            if (IsHost || IsServer)
            {
                /*
                if (hostFilter != null)
                {
                    hostFilter.GetResultEvent().AddListener(FilterResultServerRpc);
                    if (runFilterOnStart) hostFilter.FilterDefault();
                }
                */
                if (runHostEventOnStart) TriggerOnHost();
                if (runSyncedEventOnStart) TriggerSyncedEvent();
            }
        }

        /*
        public void SendFilterRequest()
        {
            hostFilter?.FilterDefault();
        }

        [ServerRpc(RequireOwnership = false)]
        private void FilterResultServerRpc(bool success)
        {
            FilterResultClientRpc(success);
        }

        [ClientRpc]
        private void FilterResultClientRpc(bool success)
        {
            if (success)
            {
                SyncedFilterSuccess.Invoke();
            }
            else
            {
                SyncedFilterFailure.Invoke();
            }
        }
        */

        public void TriggerOnHost()
        {
            RunOnHostServerRpc();
        }

        public void TriggerOnHost(PlayerControllerB player)
        {
            RunOnHostServerRpc((int)player.actualClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void RunOnHostServerRpc(int playerId = -1)
        {
            hostEvent.Invoke();
            if (playerId >= 0)
            {
                hostPlayerEvent.Invoke(RoundManager.Instance.playersManager.allPlayerScripts[playerId]);
            }
        }

        public void TriggerSyncedEvent()
        {
            SyncEventServerRpc();
        }

        public void TriggerSyncedEvent(PlayerControllerB player)
        {
            SyncEventServerRpc((int)player.actualClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SyncEventServerRpc(int playerId = -1)
        {
            SyncEventClientRpc(playerId);
        }

        [ClientRpc]
        private void SyncEventClientRpc(int playerId)
        {
            syncedEvent.Invoke();
            if (playerId >= 0)
            {
                syncedPlayerEvent.Invoke(RoundManager.Instance.playersManager.allPlayerScripts[playerId]);
            }
        }
    }
}
