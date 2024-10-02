using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using JLL.API;
using JLL.API.Compatability;
using JLL.Patches;
using LethalLib.Modules;
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
        private const string modVersion = "1.6.5";

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

            NetcodeRequired(mls);

            networkObject = NetworkPrefabs.CreateNetworkPrefab("JLL");
            networkObject.AddComponent<JLLNetworkManager>();
            networkObject.name = "JLL";

            JCompatabilityHelper.Init();
            if (JCompatabilityHelper.IsModLoaded.LethalConfig)
            {
                LethalConfigHelper.CreateJLLModConfig();
            }

            harmony.PatchAll(typeof(ItemChargerPatch));
            harmony.PatchAll(typeof(TimeOfDayPatch));
            harmony.PatchAll(typeof(HudManagerPatch));
            harmony.PatchAll(typeof(StartOfRoundPatch));
            harmony.PatchAll(typeof(RoundManagerPatch));
            harmony.PatchAll(typeof(LungPropPatch));
            harmony.PatchAll(typeof(VehicleControllerPatch));
            harmony.PatchAll(typeof(MenuManagerPatch));
            harmony.PatchAll(typeof(BreakerBoxPatch));

            JFileHelper.LoadFilesInPlugins();
            /*
            List<JsonModSettings> jsonMods = JFileHelper.ParseJsonFiles<JsonModSettings>(JFileHelper.GetFilesInDirectory(Paths.PluginPath, new List<string> { "JLLMod.json" }));
            JLogHelper.LogInfo($"Found {jsonMods.Count} JLLMods. Registering them now...", JLogLevel.User);
            int registeredMods = CustomConfigRegistry.RegisterMods(jsonMods);

            int registeredLevelOverrides = 0;
            for (int i = 0; i < jsonMods.Count; i++)
            {
                foreach (var levelPropertis in jsonMods[i].levelPropertyOverrides)
                {
                    JLevelPropertyRegistry.RegisterLevelProperties(new JLevelProperties() { sceneName = levelPropertis.sceneName, enemyPropertyOverrides = levelPropertis.enemyPropertyOverrides });
                    registeredLevelOverrides++;
                }
            }

            JLogHelper.LogInfo($"Successfully Registered {registeredMods} JLLMods with {registeredLevelOverrides} level property overrides.", JLogLevel.User);
            */
        }

        public static void NetcodeRequired(ManualLogSource logSource)
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
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
