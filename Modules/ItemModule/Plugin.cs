using BepInEx;
using HarmonyLib;
using JLL.API;
using JLLItemsModule.Patches;
using System.Reflection;

namespace JLLItemsModule
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("JacobG5.JLL")]
    public class JLLItemsCore : BaseUnityPlugin
    {
        private const string modGUID = "JacobG5.JLLItemModule";
        private const string modName = "JLLItemModule";
        private const string modVersion = "1.3.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        public void Awake()
        {
            JLL.JLL.NetcodePatch(JLogHelper.GetSource(), Assembly.GetExecutingAssembly().GetTypes());
            JLL.JLL.HarmonyPatch(harmony, JLogHelper.GetSource(), typeof(PlayerPatch), typeof(DepositItemsDeskPatch));
        }
    }
}
