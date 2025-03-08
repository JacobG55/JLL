using HarmonyLib;
using JLL.API.LevelProperties;
using JLL.Components;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(LungProp))]
    internal class LungPropPatch
    {
        private static EnemyType? VanillaRadMech;

        [HarmonyPatch("EquipItem")]
        [HarmonyPrefix]
        public static void patchEquipItem(LungProp __instance)
        {
            if (__instance.isLungDocked)
            {
                // This addition was recomended by v0xx. 
                // It'd be better if this was accounted for in LLL's Content Restoration instead of here.
                // If / When this gets added to LLL this will be removed as it's intended to be temporary anyways.
                if (VanillaRadMech == null) VanillaRadMech = JLevelPropertyRegistry.GetRegisteredEnemy("RadMech");

                __instance.radMechEnemyType = VanillaRadMech;

                JLevelPropertyRegistry.ApparatusPulled = true;
                for (int i = 0; i < JLevelEventTriggers.EventTriggers.Count; i++)
                {
                    JLevelEventTriggers.EventTriggers[i].InvokeApparatus();
                }
            }
        }
    }
}
