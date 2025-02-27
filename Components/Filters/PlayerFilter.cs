using GameNetcodeStuff;
using HarmonyLib;
using JLL.API;
using JLL.API.Compatability;
using System;
using UnityEngine;

namespace JLL.Components.Filters
{
    public class PlayerFilter : JFilter<PlayerControllerB>
    {
        [Header("Inventory")]
        public ItemFilter heldItemFilter = new ItemFilter();
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

        [Header("Player Actions")]
        public CheckFilter isWalking = new CheckFilter() { value = true };
        public CheckFilter isSprinting = new CheckFilter() { value = true };
        public CheckFilter isCrouching = new CheckFilter() { value = true };
        public CheckFilter isExhausted = new CheckFilter() { value = true };
        public CheckFilter isUnderwater = new CheckFilter() { value = true };
        public CheckFilter isSinking = new CheckFilter() { value = true };
        public CheckFilter isClimbing = new CheckFilter() { value = true };
        public CheckFilter isInVehicle = new CheckFilter() { value = true };
        public CheckFilter isBleeding = new CheckFilter() { value = true };
        public CheckFilter isAlone = new CheckFilter() { value = true };
        public CheckFilter isEmoting = new CheckFilter() { value = true };
        public CheckFilter isInSpecialAnim = new CheckFilter() { value = true };

        [Header("Legacy (May be removed in the future)")]
        public HeldItemFilter heldItem = new HeldItemFilter();
        public bool ignoreDeadCheck = false;

        public override void Filter(PlayerControllerB player)
        {
            if (!ignoreDeadCheck && player.isPlayerDead) goto Failed;

            if (!heldItemFilter.Check(player.currentlyHeldObjectServer)) goto Failed;
            if (heldItem.shouldCheck && !heldItem.CheckValue(player)) goto Failed;
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
                if (foundItems != inventoryContents.Length) goto Failed;
            }
            if (!healthCheck.Check(player.health)) goto Failed;
            if (!staminaCheck.Check(player.sprintMeter)) goto Failed;
            if (!weightCheck.Check(player.carryWeight)) goto Failed;
            if (!inFacility.Check(player.isInsideFactory)) goto Failed;
            if (!isLocalPlayer.Check(player.actualClientId == GameNetworkManager.Instance.localPlayerController.actualClientId)) goto Failed;
            if (!username.Check(player.playerUsername)) goto Failed;

            if (isWalking.shouldCheck)
            {
                try
                {
                    Traverse playerTraverse = Traverse.Create(player);
                    if (!isWalking.Check(playerTraverse.Field("isWalking").GetValue<bool>()))
                    {
                        goto Failed;
                    }
                }
                catch
                {
                    // This is fine.
                    JLogHelper.LogInfo($"{name} isWalking check failed on {player.playerUsername}.", JLogLevel.Wesley);
                }
            }
            if (!isSprinting.Check(player.isSprinting)) goto Failed;
            if (!isCrouching.Check(player.isCrouching)) goto Failed;
            if (!isExhausted.Check(player.isExhausted)) goto Failed;
            if (!isUnderwater.Check(player.isUnderwater)) goto Failed;
            if (!isSinking.Check(player.isSinking)) goto Failed;
            if (!isClimbing.Check(player.isClimbingLadder)) goto Failed;
            if (!isInVehicle.Check(player.inVehicleAnimation)) goto Failed;
            if (!isBleeding.Check(player.bleedingHeavily)) goto Failed;
            if (!isAlone.Check(player.isPlayerAlone)) goto Failed;
            if (!isEmoting.Check(player.performingEmote)) goto Failed;
            if (!isInSpecialAnim.Check(player.inSpecialInteractAnimation)) goto Failed;

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
                    if (JCompatabilityHelper.IsLoaded(JCompatabilityHelper.CachedMods.LethalLevelLoader) && contentTags.Length > 0)
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
