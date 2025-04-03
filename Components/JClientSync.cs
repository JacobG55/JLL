using GameNetcodeStuff;
using JLL.API;
using JLL.Components.Filters;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class JClientSync : NetworkBehaviour
    {
        [Header("Synced Filter")]
        [SerializeField]
        public JFilter hostFilter;
        public bool runFilterOnStart = false;
        public UnityEvent SyncedFilterSuccess = new UnityEvent();
        public UnityEvent SyncedFilterFailure = new UnityEvent();

        [Header("Host Only")]
        public bool runHostEventOnStart = false;
        public UnityEvent hostEvent = new UnityEvent();
        public InteractEvent hostPlayerEvent = new InteractEvent();

        [Header("Synced")]
        public bool runSyncedEventOnStart = false;
        public UnityEvent syncedEvent = new UnityEvent();
        public InteractEvent syncedPlayerEvent = new InteractEvent();

        public override void OnNetworkSpawn()
        {
            if (IsHost || IsServer)
            {
                if (hostFilter != null)
                {
                    hostFilter.FilteredResult.AddListener(FilterResultServerRpc);
                    if (runFilterOnStart) hostFilter.FilterDefault();
                }
                if (runHostEventOnStart) TriggerOnHost();
                if (runSyncedEventOnStart) TriggerSyncedEvent();
            }
            base.OnNetworkSpawn();
        }

        public void FilterOnHost()
        {
            RunFilterOnHostServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void RunFilterOnHostServerRpc()
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

        public void TriggerOnHost()
        {
            RunOnHostServerRpc();
        }

        public void TriggerOnHost(PlayerControllerB player)
        {
            RunOnHostServerRpc(player.Index());
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
            SyncEventServerRpc(player.Index());
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

        public override void OnDestroy()
        {
            if (hostFilter != null) hostFilter.FilteredResult.RemoveListener(FilterResultServerRpc);
            base.OnDestroy();
        }
    }
}
