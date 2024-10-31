using BepInEx;
using HarmonyLib;
using JLL.API;
using JLLItemsModule.Patches;
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
        private const string modVersion = "1.2.2";

        private readonly Harmony harmony = new Harmony(modGUID);

        public void Awake()
        {
            JLL.JLL.NetcodePatch(JLogHelper.GetSource(), Assembly.GetExecutingAssembly().GetTypes());
            //RegisterTestItems();
            JLL.JLL.HarmonyPatch(harmony, JLogHelper.GetSource(), typeof(PlayerPatch), typeof(DepositItemsDeskPatch));
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
