using GameNetcodeStuff;
using JLL.Components.Filters;
using UnityEngine;

namespace JLL.Components
{
    public class InventoryRemover : MonoBehaviour
    {
        public string[] itemsToRemove;
        public ItemFilter.Properties[] removeByFilter = new ItemFilter.Properties[0];
        public bool removeAllInstances = false;

        public void RemoveItems(PlayerControllerB player)
        {
            for (int i = 0; i < player.ItemSlots.Length; i++)
            {
                if (player.ItemSlots[i] == null) continue;

                if (CheckSlot(player, i) && !removeAllInstances)
                {
                    break;
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
                CheckSlot(player, player.currentItemSlot);
            }
        }

        private bool CheckSlot(PlayerControllerB player, int slot)
        {
            for (int r = 0; r < itemsToRemove.Length; r++)
            {
                if (player.ItemSlots[slot].itemProperties.itemName.ToLower() == itemsToRemove[r].ToLower())
                {
                    player.DestroyItemInSlotAndSync(slot);
                    return true;
                }
            }
            for (int r = 0; r < removeByFilter.Length; r++)
            {
                if (removeByFilter[r].Check(player.ItemSlots[slot]))
                {
                    player.DestroyItemInSlotAndSync(slot);
                    return true;
                }
            }
            return false;
        }
    }
}
