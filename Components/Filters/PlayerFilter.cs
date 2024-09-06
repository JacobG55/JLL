using GameNetcodeStuff;
using System;
using UnityEngine;

namespace JLL.Components.Filters
{
    public class PlayerFilter : JFilter<PlayerControllerB>
    {
        [Header("Inventory")]
        public HeldItemFilter heldItem = new HeldItemFilter();
        public string[] inventoryContents = new string[0];

        [Header("Player Stats")]
        public NumericFilter healthCheck = new NumericFilter() { value = 20f };
        public NumericFilter staminaCheck = new NumericFilter() { value = 20f };
        public NumericFilter weightCheck = new NumericFilter() { value = 2f };

        public override void Filter(PlayerControllerB player)
        {
            bool success = true;

            if (heldItem.shouldCheck)
            {
                success &= heldItem.CheckValue(player);
            }

            if (inventoryContents.Length > 0)
            {
                int foundItems = 0;
                for (int j = 0; j < inventoryContents.Length; j++)
                {
                    for (int i = 0; i < player.ItemSlots.Length; i++)
                    {
                        if (player.ItemSlots[i] == null) continue;
                        if (player.ItemSlots[i].itemProperties.itemName.ToLower().Equals(inventoryContents[j].ToLower()))
                        {
                            foundItems++;
                            break;
                        }
                    }
                }
                success &= foundItems == inventoryContents.Length;
            }

            if (healthCheck.shouldCheck)
            {
                success &= healthCheck.CheckValue(player.health);
            }

            if (staminaCheck.shouldCheck)
            {
                success &= staminaCheck.CheckValue(player.sprintMeter);
            }

            if (weightCheck.shouldCheck)
            {
                success &= weightCheck.CheckValue(player.carryWeight);
            }

            Result(success, player);
        }

        [Serializable]
        public class HeldItemFilter
        {
            public bool shouldCheck = false;
            public  NameFilter itemName = new NameFilter();
            public NumericFilter itemCharge = new NumericFilter() { value = 100 };

            public bool CheckValue(PlayerControllerB player)
            {
                if (shouldCheck && player.currentlyHeldObject != null)
                {
                    GrabbableObject item = player.currentlyHeldObject;

                    bool success = true;

                    if (itemName.shouldCheck)
                    {
                        success &= itemName.CheckValue(item.itemProperties.itemName);
                    }
                    if (itemCharge.shouldCheck)
                    {
                        if (item.insertedBattery != null)
                        {
                            success &= itemCharge.CheckValue(item.insertedBattery.charge);
                        }
                    }
                    return success;
                }
                return false;
            }
        }
    }
}
