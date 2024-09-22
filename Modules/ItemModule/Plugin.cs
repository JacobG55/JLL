using BepInEx;
using BepInEx.Logging;
using JLL.API;
using LethalLib.Modules;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace JLLItemsModule
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("JacobG5.JLL")]
    [BepInDependency("evaisa.lethallib")]
    public class JLLItemsCore : BaseUnityPlugin
    {
        private const string modGUID = "JacobG5.JLLItemModule";
        private const string modName = "JLLItemModule";
        private const string modVersion = "1.0.0";

        public void Awake()
        {
            NetcodeRequired(JLogHelper.GetSource());
            //RegisterTestItems();
        }

        private static void NetcodeRequired(ManualLogSource logSource)
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
                    logSource.LogInfo("Item Module: Skipping Netcode Class");
                }
            }
            logSource.LogInfo("Item Module: Netcode Successfully Patched!");
        }

        private void RegisterTestItems()
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "testitems"));

            Items.RegisterItem(assetBundle.LoadAsset<Item>("Assets/LethalCompany/Mods/ItemTests/TestStunGrenade_0.asset"));
            Items.RegisterItem(assetBundle.LoadAsset<Item>("Assets/LethalCompany/Mods/ItemTests/TestShovel.asset"));
            Items.RegisterItem(assetBundle.LoadAsset<Item>("Assets/LethalCompany/Mods/ItemTests/TestKnife.asset"));
            Items.RegisterItem(assetBundle.LoadAsset<Item>("Assets/LethalCompany/Mods/ItemTests/TestGiftBox.asset"));
            Items.RegisterItem(assetBundle.LoadAsset<Item>("Assets/LethalCompany/Mods/ItemTests/TestEasterEgg.asset"));
        }
    }
}
