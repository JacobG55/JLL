using GameNetcodeStuff;
using JLL.Components.Filters;
using UnityEngine;

namespace JLL.Components
{
    public class InventoryRemover : MonoBehaviour
    {
        public string[] itemsToRemove;
        public ItemFilter.Properties[] removeByFilter = [];
        public bool removeAllInstances = false;

        public void RemoveItems(PlayerControllerB player)
        {
            if (player.ItemOnlySlot != null && CheckSlot(player, 50) && !removeAllInstances)
            {
                return;
            }

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
            GrabbableObject item = slot == 50 ? player.ItemOnlySlot : player.ItemSlots[slot];

            for (int r = 0; r < itemsToRemove.Length; r++)
            {
                if (item.itemProperties.itemName.ToLower() == itemsToRemove[r].ToLower())
                {
                    player.DestroyItemInSlotAndSync(slot);
                    return true;
                }
            }
            for (int r = 0; r < removeByFilter.Length; r++)
            {
                if (removeByFilter[r].Check(item))
                {
                    player.DestroyItemInSlotAndSync(slot);
                    return true;
                }
            }
            return false;
        }
    }
}
