using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using JLL.API;
using System;
using System.Reflection;
using WesleyMoonScripts.Patches;
using WesleyMoonScripts.Patches.ReLocker;

namespace WesleyMoonScripts
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("JacobG5.JLL")]
    [BepInDependency("JacobG5.JLLItemModule")]
    [BepInDependency("imabatby.lethallevelloader")]
    [BepInDependency("JacobG5.WesleyMoons", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("ProjectBots.MoonUnlockUnhide", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.xmods.lethalmoonunlocks", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("DisplayAllMoons", BepInDependency.DependencyFlags.SoftDependency)]
    public class WesleyScripts : BaseUnityPlugin
    {
        private const string modGUID = "JacobG5.WesleyMoonScripts";
        private const string modName = "WesleyMoonScripts";
        private const string modVersion = "1.1.2";

        public static WesleyScripts Instance;

        internal ManualLogSource mls;

        public static ConfigEntry<bool> LockMoons;

        private readonly Harmony harmony = new Harmony(modGUID);

        internal static bool WesleyPresent = false;

        public void Awake()
        {
            Instance = this;

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            JLL.JLL.NetcodePatch(mls, Assembly.GetExecutingAssembly().GetTypes());

            WesleyPresent = JCompatabilityHelper.IsLoaded(JCompatabilityHelper.CachedMods.WesleyMoons);

            if (WesleyPresent)
            {
                LockMoons = Config.Bind("Core", "LockMoons", true, "Locks moons that have progression integration set up to enable playing the campaign.");

                if (LockMoons.Value)
                {
                    if (JCompatabilityHelper.IsLoaded("ProjectBots.MoonUnlockUnhide"))
                    {
                        mls.LogInfo("Patching MoonUnlockUnhide");
                        try
                        {
                            harmony.PatchAll(typeof(MoonUnlockUnhidePatch));
                        }
                        catch (Exception e)
                        {
                            mls.LogWarning($"Patching MoonUnlockUnhide Failed!\n{e}");
                        }
                    }
                    if (JCompatabilityHelper.IsLoaded("DisplayAllMoons"))
                    {
                        mls.LogInfo("Patching DisplayAllMoons");
                        try
                        {
                            harmony.PatchAll(typeof(DisplayAllMoonsPatch));
                        }
                        catch (Exception e)
                        {
                            mls.LogWarning($"Patching DisplayAllMoons Failed!\n{e}");
                        }
                    }
                }
            }
            try
            {
                harmony.PatchAll(typeof(LLLContentPatch));
            }
            catch (Exception e)
            {
                mls.LogWarning($"Patching LLL PatchedContent Failed!\n{e}");
            }

            harmony.PatchAll(typeof(EntranceTeleportPatch));
        }

        public static bool ProtectionEnabled() => WesleyPresent && LockMoons.Value;
    }
}
