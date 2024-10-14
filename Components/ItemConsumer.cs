using GameNetcodeStuff;
using Unity.Netcode;

namespace JLL.Components
{
    public class ItemConsumer : NetworkBehaviour
    {
        public Item[] validItems;
        public bool checkExact = false;

        public InteractEvent OnSuccess = new InteractEvent();

        public void CheckHeldItem(PlayerControllerB player)
        {
            CheckItemServerRpc((int)player.actualClientId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void CheckItemServerRpc(int playerWhoSent)
        {
            CheckItemClientRpc(playerWhoSent);
        }

        [ClientRpc]
        private void CheckItemClientRpc(int playerWhoSent)
        {
            PlayerControllerB player = RoundManager.Instance.playersManager.allPlayerScripts[playerWhoSent];

            if (player.currentlyHeldObjectServer == null) return;

            GrabbableObject grabbableObject = player.currentlyHeldObjectServer;

            for (int i = 0; i < validItems.Length; i++)
            {
                if (checkExact ? grabbableObject.itemProperties == validItems[i] : grabbableObject.itemProperties.itemName == validItems[i].itemName)
                {
                    player.DestroyItemInSlotAndSync(playerWhoSent);
                    OnSuccess.Invoke(player);
                    break;
                }
            }
        }
    }
}
