using GameNetcodeStuff;
using HarmonyLib;
using JLLItemsModule.Components;

namespace JLLItemsModule.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerPatch
    {
        [HarmonyPatch("SetItemInElevator")]
        [HarmonyPrefix]
        public static void patchSetItemInElevator(bool droppedInShipRoom, bool droppedInElevator, GrabbableObject gObject)
        {
            if (gObject.isInShipRoom != droppedInShipRoom)
            {
                if (gObject is JGrabbableObject jObj)
                {
                    jObj.OnSetInsideShip(droppedInShipRoom);
                    jObj.OnSetInShip.Invoke(droppedInShipRoom);
                }
            }
        }
    }
}
