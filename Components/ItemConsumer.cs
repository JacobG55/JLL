using GameNetcodeStuff;
using JLL.Components.Filters;
using JLL.API;
using Unity.Netcode;

namespace JLL.Components
{
    public class ItemConsumer : NetworkBehaviour
    {
        public Item[] validItems = [];
        public bool checkExact = false;
        public ItemFilter.Properties[] validItemFilters = [];

        public InteractEvent OnSuccess = new();
        public bool runEventOnAllClients = true;

        public void CheckHeldItem(PlayerControllerB player)
        {
            CheckItemServerRpc(player.Index());
        }

        [ServerRpc(RequireOwnership = false)]
        private void CheckItemServerRpc(int playerWhoSent)
        {
            PlayerControllerB player = RoundManager.Instance.playersManager.allPlayerScripts[playerWhoSent];

            if (player.currentlyHeldObjectServer == null) return;

            GrabbableObject grabbableObject = player.currentlyHeldObjectServer;

            for (int i = 0; i < validItems.Length; i++)
            {
                if (checkExact ? grabbableObject.itemProperties == validItems[i] : grabbableObject.itemProperties.itemName == validItems[i].itemName)
                {
                    goto OnSuccess;
                }
            }

            for (int i = 0; i < validItemFilters.Length; i++)
            {
                if (validItemFilters[i].Check(grabbableObject))
                {
                    goto OnSuccess;
                }
            }

            return;

            OnSuccess:

            RemoveItemFromClientRpc(playerWhoSent);
        }

        [ClientRpc]
        private void RemoveItemFromClientRpc(int playerWhoSent)
        {
            PlayerControllerB player = RoundManager.Instance.playersManager.allPlayerScripts[playerWhoSent];
            bool isLocalPlayer = player.IsLocalPlayer();

            JLogHelper.LogInfo($"Destroying Item in {player.playerUsername} {playerWhoSent} slot: {player.currentItemSlot}", JLogLevel.Debuging);
            player.DestroyItemInSlot(player.currentItemSlot);

            if (runEventOnAllClients || isLocalPlayer)
            {
                OnSuccess.Invoke(player);
            }
        }
    }
}
