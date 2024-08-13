using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using JLL.Patches;

namespace JLL
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class JLLBase : BaseUnityPlugin
    {
        private const string modGUID = "JacobG5.JLL";
        private const string modName = "JLL";
        private const string modVersion = "1.1.5";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static JLLBase Instance;

        internal ManualLogSource mls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            harmony.PatchAll(typeof(ItemChargerPatch));
            harmony.PatchAll(typeof(TimeOfDayPatch));
            harmony.PatchAll(typeof(HudManagerPatch));
            harmony.PatchAll(typeof(StartOfRoundPatch));
        }
    }
}
