using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using JLL.API;
using JLL.API.Compatability;
using JLL.Components;
using JLL.Patches;
using JLL.ScriptableObjects;
using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace JLL
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("mrov.WeatherRegistry", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("imabatby.lethallevelloader", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("ainavt.lc.lethalconfig", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Chaos.LCCutscene", BepInDependency.DependencyFlags.SoftDependency)]
    public class JLL : BaseUnityPlugin
    {
        private const string modGUID = "JacobG5.JLL";
        private const string modName = "JLL";
        private const string modVersion = "1.10.1";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static JLL Instance;

        internal ManualLogSource mls;
        internal ManualLogSource wesley;

        private static Transform JLLPrefabContainer;
        public GameObject networkObject;
        public JNetworkPrefabSet JLLNetworkPrefabs;

        internal GameObject ElevatorPrefab;

        public static ConfigEntry<JLogLevel> loggingLevel;
        public static ConfigEntry<bool> purgeWesley;

        // Diversity 
        public static ConfigEntry<bool> disableCutscenes;

        void Awake() 
        {
            if (Instance == null) Instance = this;
            JLLPrefabContainer = new GameObject("JLLPrefabContainer")
            {
                hideFlags = HideFlags.HideAndDontSave
            }.transform;
            JLLPrefabContainer.gameObject.SetActive(false);

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            wesley = BepInEx.Logging.Logger.CreateLogSource("Wesley");

            NetcodePatch(mls, [
                typeof(JLLNetworkManager), 
                typeof(DamageTriggerNetworking), 
                typeof(ItemConsumer), 
                typeof(JActionEvents), 
                typeof(JBridgeTrigger), 
                typeof(JClientSync), 
                typeof(JDestructableObject), 
                typeof(JPlayerInsideRegion), 
                typeof(JTerminalController), 
                typeof(RandomizedEvent), 
                typeof(SeatController)
            ]);

            loggingLevel = Config.Bind("Logging", "LoggingLevel", JLogLevel.User, "Changes the amount of logging JLL performs in it's scripts.");
            loggingLevel.SettingChanged += (obj, args) => JLogHelper.UpdateLogLevel();

            JLogHelper.UpdateLogLevel();

            JLLNetworkPrefabs = ScriptableObject.CreateInstance<JNetworkPrefabSet>();
            JLLNetworkPrefabs.SetName = modName;
            JLLNetworkPrefabs.AddPrefabs(
                new JNetworkPrefabSet.JIdentifiablePrefab { name = modName, prefab = networkObject = CreateNetworkPrefab(modName) },
                new JNetworkPrefabSet.JIdentifiablePrefab { name = "EmptyPrefab", prefab = JNetworkPrefabSet.EmptyNetworkObject = CreateNetworkPrefab("EmptyPrefab") },
                new JNetworkPrefabSet.JIdentifiablePrefab { name = "ElevatorBody", prefab = ElevatorPrefab = CreateNetworkPrefab("JLLElevator") }
            );

            /*
            JElevatorBody elevatorBody = ElevatorPrefab.AddComponent<JElevatorBody>();
            GameObject elevatorPhysicsBody = new("PhysicsRegion");
            elevatorPhysicsBody.transform.SetParent(ElevatorPrefab.transform);

            BoxCollider physicsBox = elevatorPhysicsBody.AddComponent<BoxCollider>();
            physicsBox.isTrigger = true;
            elevatorBody.PhysicsCollider = physicsBox;

            BoxCollider floorCollider = elevatorPhysicsBody.AddComponent<BoxCollider>();
            floorCollider.isTrigger = false;
            elevatorBody.FloorCollider = floorCollider;

            PlayerPhysicsRegion ppr = elevatorPhysicsBody.AddComponent<PlayerPhysicsRegion>();
            ppr.parentNetworkObject = ElevatorPrefab.GetComponent<NetworkObject>();
            ElevatorPrefab.AddComponent<Rigidbody>().isKinematic = true;
            ppr.physicsTransform = ElevatorPrefab.transform;
            ppr.physicsCollider = physicsBox;
            ppr.itemDropCollider = physicsBox;
            ppr.addPositionOffsetToItems = new(0, 0.2f, 0);
            ppr.maxTippingAngle = 180;
            ppr.allowDroppingItems = true;
            elevatorBody.PhysicsRegion = ppr;

            elevatorPhysicsBody.AddComponent<Rigidbody>().isKinematic = true;
            */

            networkObject.AddComponent<JLLNetworkManager>();
            JNetworkPrefabSet.NetworkPrefabSets.Add(JLLNetworkPrefabs);

            purgeWesley = Config.Bind("Logging", "PurgeWesley", false, "Destroys him.");

            if (JCompatabilityHelper.IsLoaded(JCompatabilityHelper.CachedMods.LCCutscene))
            {
                disableCutscenes = Config.Bind("LCCutscene", "DisableJLLCutscenes", false, "A global shutoff for all cutscenes triggered by JLL using LCCutscene.");
            }

            if (JCompatabilityHelper.IsLoaded(JCompatabilityHelper.CachedMods.LethalConfig))
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
                typeof(GameNetworkManagerPatch),
                typeof(StormyWeatherPatch)
            );

            JFileHelper.LoadFilesInPlugins();
        }

        public static GameObject CreateNetworkPrefab(string name)
        {
            GameObject networkObject = new GameObject("JLL")
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            networkObject.transform.SetParent(JLLPrefabContainer);
            byte[] value = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(Assembly.GetCallingAssembly().GetName().Name + name));
            Traverse.Create(networkObject.AddComponent<NetworkObject>()).Field("GlobalObjectIdHash").SetValue(BitConverter.ToUInt32(value, 0));
            return networkObject;
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

        public static void NetcodePatch(ManualLogSource logSource) => NetcodePatch(logSource, Assembly.GetCallingAssembly().GetTypes());
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
