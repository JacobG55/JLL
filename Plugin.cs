using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using JLL.API;
using JLL.API.Compatability;
using JLL.Patches;
using JLL.ScriptableObjects;
using LethalLib.Modules;
using System;
using System.Reflection;
using UnityEngine;

namespace JLL
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("evaisa.lethallib")]
    [BepInDependency("mrov.WeatherRegistry", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("imabatby.lethallevelloader", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("ainavt.lc.lethalconfig", BepInDependency.DependencyFlags.SoftDependency)]
    public class JLL : BaseUnityPlugin
    {
        private const string modGUID = "JacobG5.JLL";
        private const string modName = "JLL";
        private const string modVersion = "1.7.5";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static JLL Instance;

        internal ManualLogSource mls;
        internal ManualLogSource wesley;

        public GameObject networkObject;

        public static ConfigEntry<JLogLevel> loggingLevel;
        public static ConfigEntry<bool> purgeWesley;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            wesley = BepInEx.Logging.Logger.CreateLogSource("Wesley");

            loggingLevel = Config.Bind("Logging", "LoggingLevel", JLogLevel.User, "Changes the amount of logging JLL performs in it's scripts.");
            loggingLevel.SettingChanged += (obj, args) => JLogHelper.UpdateLogLevel();
            purgeWesley = Config.Bind("Logging", "PurgeWesley", false, "Destroys him.");

            JLogHelper.UpdateLogLevel();

            NetcodePatch(mls, Assembly.GetExecutingAssembly().GetTypes());

            networkObject = NetworkPrefabs.CreateNetworkPrefab("JLL");
            networkObject.AddComponent<JLLNetworkManager>();
            networkObject.name = "JLL";

            JNetworkPrefabSet.EmptyNetworkObject = NetworkPrefabs.CreateNetworkPrefab("EmptyPrefab");

            JCompatabilityHelper.Init();
            if (JCompatabilityHelper.IsModLoaded.LethalConfig)
            {
                LethalConfigHelper.CreateJLLModConfig();
            }

            HarmonyPatch(harmony, mls,
                typeof(ItemChargerPatch),
                typeof(TimeOfDayPatch),
                typeof(HudManagerPatch),
                typeof(StartOfRoundPatch),
                typeof(RoundManagerPatch),
                typeof(LungPropPatch),
                typeof(VehicleControllerPatch),
                typeof(MenuManagerPatch),
                typeof(BreakerBoxPatch),
                typeof(ItemDropshipPatch),
                typeof(GameNetworkManagerPatch)
            );

            JFileHelper.LoadFilesInPlugins();
        }

        public static void HarmonyPatch(Harmony harmony, ManualLogSource logSource, params Type[] patches)
        {
            foreach (Type patch in patches)
            {
                try
                {
                    harmony.PatchAll(patch);
                }
                catch (Exception e)
                {
                    logSource.LogError($"Caught Error while trying to patch {patch.Name}\n{e}");
                }
            }
        }

        public static void NetcodePatch(ManualLogSource logSource, params Type[] types)
        {
            foreach (Type type in types)
            {
                try
                {
                    var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                    foreach (var method in methods)
                    {
                        var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                        if (attributes.Length > 0)
                        {
                            method.Invoke(null, null);
                        }
                    }
                }
                catch
                {
                    logSource.LogInfo("Skipping Netcode Class");
                }
            }
            logSource.LogInfo("Netcode Successfully Patched!");
        }
    }
}
