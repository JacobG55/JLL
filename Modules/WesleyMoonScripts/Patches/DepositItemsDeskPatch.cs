using HarmonyLib;

namespace WesleyMoonScripts.Patches
{
    [HarmonyPatch(typeof(DepositItemsDesk))]
    internal class DepositItemsDeskPatch
    {
        [HarmonyPatch("CheckAnimationGrabPlayerServerRpc")]
        [HarmonyPrefix]
        public static bool patchCheckAnimationGrabPlayerServerRpc(int monsterAnimationID, int playerID, DepositItemsDesk __instance)
            => GrabNullCheck(monsterAnimationID, __instance);
        [HarmonyPatch("ConfirmAnimationGrabPlayerClientRpc")]
        [HarmonyPrefix]
        public static bool patchConfirmAnimationGrabPlayerClient(int monsterAnimationID, int playerID, DepositItemsDesk __instance)
            => GrabNullCheck(monsterAnimationID, __instance);
        public static bool GrabNullCheck(int monsterAnimationID, DepositItemsDesk desk)
            => desk.monsterAnimations[monsterAnimationID].monsterAnimatorGrabPoint != null;
    }
}
