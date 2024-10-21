using HarmonyLib;
using JLL.API;
using JLL.Components;
using UnityEngine;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(ItemDropship))]
    internal class ItemDropshipPatch
    {
        [HarmonyPatch("OpenShipDoorsOnServer")]
        [HarmonyPostfix]
        public static void patchOpenShipDoorsOnServer(ItemDropship __instance)
        {
            if (__instance.TryGetComponent(out JItemDropshipModifier jmodifier))
            {
                for (int i = 0; i < __instance.itemSpawnPositions.Length; i++)
                {
                    Collider[] colliders = Physics.OverlapSphere(__instance.itemSpawnPositions[i].position, 0.5f, 64);

                    for (int c = 0; c < colliders.Length; c++)
                    {
                        if (colliders[c].gameObject.TryGetComponent(out GrabbableObject item))
                        {
                            JLogHelper.LogInfo($"Found dropship item! {item.itemProperties.itemName}", JLogLevel.Wesley);
                            jmodifier.ModifyDroppedItems(item);
                        }
                    }
                }
            }
        }
    }
}
