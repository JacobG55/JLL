﻿using GameNetcodeStuff;
using JLL.Components.Filters;
using JLL.API;
using Unity.Netcode;

namespace JLL.Components
{
    public class ItemConsumer : NetworkBehaviour
    {
        public Item[] validItems = new Item[0];
        public bool checkExact = false;
        public ItemFilter.Properties[] validItemFilters = new ItemFilter.Properties[0];

        public InteractEvent OnSuccess = new InteractEvent();
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

            if (isLocalPlayer) player.DestroyItemInSlotAndSync(player.currentItemSlot);

            if (runEventOnAllClients || isLocalPlayer)
            {
                OnSuccess.Invoke(player);
            }
        }
    }
}
