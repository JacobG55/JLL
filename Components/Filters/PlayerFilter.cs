using GameNetcodeStuff;
using JLL.API;
using JLL.API.Compatability;
using System;
using UnityEngine;

namespace JLL.Components.Filters
{
    public class PlayerFilter : JFilter<PlayerControllerB>
    {
        [Header("Inventory")]
        public HeldItemFilter heldItem = new HeldItemFilter();
        [Tooltip("Checks inventory for Items with matching names\nNot case sensitive")]
        public string[] inventoryContents = new string[0];

        [Header("Player Stats")]
        [Tooltip("Players have 100 HP")]
        public NumericFilter healthCheck = new NumericFilter() { value = 20f };
        [Tooltip("Stamina is a value between 0 and 1")]
        public NumericFilter staminaCheck = new NumericFilter() { value = 0.5f };
        [Tooltip("Vanilla Weight Calculation: (weight - 1) * 105 lbs")]
        public NumericFilter weightCheck = new NumericFilter() { value = 2f };
        public CheckFilter inFacility = new CheckFilter() { value = false };

        [Header("Player Info")]
        public NameFilter username = new NameFilter() { value = "Player" };
        public CheckFilter isLocalPlayer = new CheckFilter() { value = true };

        public override void Filter(PlayerControllerB player)
        {
            if (heldItem.shouldCheck && !heldItem.CheckValue(player))
            {
                goto Failed;
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
                if (foundItems != inventoryContents.Length)
                {
                    goto Failed;
                }
            }

            if (healthCheck.shouldCheck && !healthCheck.CheckValue(player.health))
            {
                goto Failed;
            }

            if (staminaCheck.shouldCheck && !staminaCheck.CheckValue(player.sprintMeter))
            {
                goto Failed;
            }

            if (weightCheck.shouldCheck && !weightCheck.CheckValue(player.carryWeight))
            {
                goto Failed;
            }

            if (inFacility.shouldCheck && !inFacility.CheckValue(player.isInsideFactory))
            {
                goto Failed;
            }

            if (isLocalPlayer.shouldCheck && !isLocalPlayer.CheckValue(player.actualClientId == GameNetworkManager.Instance.localPlayerController.actualClientId))
            {
                goto Failed;
            }

            if (username.shouldCheck && !username.CheckValue(player.playerUsername))
            {
                goto Failed;
            }

            Result(player, true);
            return;

            Failed:
            Result(player);
        }

        public void FilterLocalClient()
        {
            Filter(StartOfRound.Instance.localPlayerController);
        }

        public void FilterAllPlayers()
        {
            for (int i = 0; i < RoundManager.Instance.playersManager.allPlayerScripts.Length; i++)
            {
                Filter(RoundManager.Instance.playersManager.allPlayerScripts[i]);
            }
        }

        public override void FilterDefault()
        {
            FilterAllPlayers();
        }

        [Serializable]
        public class HeldItemFilter
        {
            public bool shouldCheck = false;
            public  NameFilter itemName = new NameFilter();
            public NumericFilter itemCharge = new NumericFilter() { value = 100 };
            public string[] contentTags = new string[0];
            public bool mustHaveAllTags = true;

            public bool CheckValue(PlayerControllerB player)
            {
                if (shouldCheck && player.currentlyHeldObjectServer != null)
                {
                    GrabbableObject item = player.currentlyHeldObjectServer;

                    if (itemName.shouldCheck && !itemName.CheckValue(item.itemProperties.itemName))
                    {
                        return false;
                    }
                    if (itemCharge.shouldCheck && item.insertedBattery != null && !itemCharge.CheckValue(item.insertedBattery.charge))
                    {
                        return false;
                    }
                    if (JCompatabilityHelper.IsModLoaded.LLL && contentTags.Length > 0)
                    {
                        if (!LLLHelper.ItemTagFilter(item.itemProperties, contentTags, mustHaveAllTags))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            }
        }
    }
}
