using BepInEx;
using HarmonyLib;
using JLL.API;
using JLLItemsModule.Components;
using JLLItemsModule.Patches;

namespace JLLItemsModule
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("JacobG5.JLL")]
    public class JLLItemsCore : BaseUnityPlugin
    {
        private const string modGUID = "JacobG5.JLLItemModule";
        private const string modName = "JLLItemModule";
        private const string modVersion = "1.3.7";

        private readonly Harmony harmony = new Harmony(modGUID);

        public void Awake()
        {
            JLL.JLL.NetcodePatch(JLogHelper.GetSource(), new System.Type[] { typeof(JEventBoxItem), typeof(JGrabbableObject), typeof(JInteractableItem), typeof(JMeleeWeapon), typeof(JNoisemakerProp), typeof(JThrowableItem) });
            JLL.JLL.HarmonyPatch(harmony, JLogHelper.GetSource(), typeof(PlayerPatch), typeof(DepositItemsDeskPatch));
        }
    }
}
