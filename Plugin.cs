using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using JLL.API;
using JLL.Components;
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
    public class JLL : BaseUnityPlugin
    {
        private const string modGUID = "JacobG5.JLL";
        private const string modName = "JLL";
        private const string modVersion = "1.4.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static JLL Instance;

        internal ManualLogSource mls;

        public GameObject networkObject;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            NetcodeRequired(mls);

            networkObject = NetworkPrefabs.CreateNetworkPrefab("JLL");
            networkObject.AddComponent<JLLNetworkManager>();
            networkObject.name = "JLL";

            JCompatabilityHelper.Init();

            harmony.PatchAll(typeof(ItemChargerPatch));
            harmony.PatchAll(typeof(TimeOfDayPatch));
            harmony.PatchAll(typeof(HudManagerPatch));
            harmony.PatchAll(typeof(StartOfRoundPatch));
            harmony.PatchAll(typeof(RoundManagerPatch));
            harmony.PatchAll(typeof(LungPropPatch));
            harmony.PatchAll(typeof(VehicleControllerPatch));
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
