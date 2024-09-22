using GameNetcodeStuff;
using UnityEngine;

namespace JLL.Components
{
    public class InventoryRemover : MonoBehaviour
    {
        public string[] itemsToRemove;
        public bool removeAllInstances = false;

        public void RemoveItems(PlayerControllerB player)
        {
            for (int i = 0; i < player.ItemSlots.Length; i++)
            {
                if (player.ItemSlots[i] == null) continue;

                for (int r = 0; r < itemsToRemove.Length; r++)
                {
                    if (player.ItemSlots[i].itemProperties.itemName.ToLower() == itemsToRemove[r].ToLower())
                    {
                        player.DestroyItemInSlotAndSync(i);
                        if (removeAllInstances) break;
                    }
                }
            }
        }

        public void ClearInventory(PlayerControllerB player)
        {
            for (int i = 0; i < player.ItemSlots.Length; i++)
            {
                if (player.ItemSlots[i] == null) continue;

                player.DestroyItemInSlotAndSync(i);
            }
        }

        public void RemoveHeld(PlayerControllerB player)
        {
            if (player.currentlyHeldObjectServer != null)
            {
                for (int r = 0; r < itemsToRemove.Length; r++)
                {
                    if (player.currentlyHeldObjectServer.itemProperties.itemName.ToLower() == itemsToRemove[r].ToLower())
                    {
                        player.DestroyItemInSlotAndSync(player.currentItemSlot);
                        break;
                    }
                }
            }
        }
    }
}
