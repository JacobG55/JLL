using HarmonyLib;
using JLLItemsModule.Components;
using Unity.Netcode;

namespace JLLItemsModule.Patches
{
    [HarmonyPatch(typeof(DepositItemsDesk))]
    internal class DepositItemsDeskPatch
    {
        [HarmonyPatch("AddObjectToDeskClientRpc")]
        [HarmonyPostfix]
        public static void patchAddObjectToDeskClientRpc(NetworkObjectReference grabbableObjectNetObject)
        {
            if (grabbableObjectNetObject.TryGet(out NetworkObject netObj) && netObj.TryGetComponent(out JGrabbableObject jGrabbable))
            {
                jGrabbable.PlacedOnDepositDesk();
                jGrabbable.OnPlacedOnDepositDesk.Invoke();
            }
        }
    }
}
