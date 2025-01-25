using BepInEx;
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
    [BepInDependency("ProjectBots.MoonUnlockUnhide", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.xmods.lethalmoonunlocks", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("DisplayAllMoons", BepInDependency.DependencyFlags.SoftDependency)]
    public class WesleyScripts : BaseUnityPlugin
    {
        private const string modGUID = "JacobG5.WesleyMoonScripts";
        private const string modName = "WesleyMoonScripts";
        private const string modVersion = "1.0.0";

        public static WesleyScripts Instance;

        internal ManualLogSource mls;

        private readonly Harmony harmony = new Harmony(modGUID);

        public void Awake()
        {
            Instance = this;

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            JLL.JLL.NetcodePatch(mls, Assembly.GetExecutingAssembly().GetTypes());

            if (JCompatabilityHelper.IsLoaded(JCompatabilityHelper.CachedMods.WesleyMoons))
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
                if (JCompatabilityHelper.IsLoaded("com.xmods.lethalmoonunlocks"))
                {
                    mls.LogInfo("Patching LethalMoonUnlocks");
                    try
                    {
                        harmony.PatchAll(typeof(LethalMoonUnlocksPatch));
                    }
                    catch (Exception e)
                    {
                        mls.LogWarning($"Patching LethalMoonUnlocks Failed!\n{e}");
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
            try
            {
                harmony.PatchAll(typeof(LLLContentPatch));
            }
            catch (Exception e)
            {
                mls.LogWarning($"Patching LLL PatchedContent Failed!\n{e}");
            }

            harmony.PatchAll(typeof(EntranceTeleportPatch));
            harmony.PatchAll(typeof(DepositItemsDeskPatch));
        }
    }
}
