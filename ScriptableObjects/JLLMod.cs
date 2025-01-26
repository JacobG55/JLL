using BepInEx;
using JLL.API;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JLL.ScriptableObjects
{
    [CreateAssetMenu(menuName = "JLL/JLLMod")]
    public class JLLMod : ScriptableObject
    {
        private static readonly List<JLLMod> m_ModList = new List<JLLMod>();
        public static JLLMod? GetMod(string name, string author)
        {
            return GetMod(author + "." + name);
        }
        public static JLLMod? GetMod(string guid)
        {
            for (int i = 0; i < m_ModList.Count; i++)
            {
                if (m_ModList[i].GUID() == guid)
                {
                    return m_ModList[i];
                }
            }
            return null;
        }
        public static JLLMod[] GetModsFromAuthor(string author)
        {
            List<JLLMod> mods = new List<JLLMod>();
            for (int i = 0; i < m_ModList.Count; i++)
            {
                if (m_ModList[i].modAuthor == author)
                {
                    mods.Add(m_ModList[i]);
                }
            }
            return mods.ToArray();
        }

        public string modAuthor = "";
        public string modName = "";

        public string GUID() => modAuthor + "." + modName;

        [Header("Network Prefabs")]
        public JNetworkPrefabSet[] prefabSets = new JNetworkPrefabSet[0];

        [Header("Config Values")]
        public List<ConfigValue<bool>> Booleans = new List<ConfigValue<bool>>();
        public List<ConfigValue<string>> Strings = new List<ConfigValue<string>>();
        public List<ConfigValue<int>> Integers = new List<ConfigValue<int>>();
        public List<ConfigValue<float>> Floats = new List<ConfigValue<float>>();

        [Serializable]
        public class ConfigValue<T>
        {
            public string configName = "Example";
            public string configCategory = "Main";
            public string configDescription = "";
            public T defaultValue;
        }

        [Header("JLL Addons")]
        public JLLAddon[] jllAddons = new JLLAddon[0];

        public bool Invalid() => modAuthor.IsNullOrWhiteSpace() || modName.IsNullOrWhiteSpace();
        public bool HasConfigs() => Booleans.Count + Strings.Count + Integers.Count + Floats.Count > 0;

        internal void Init()
        {
            if (!m_ModList.Contains(this)) m_ModList.Add(this);
            for (int i = 0; i < prefabSets.Length; i++)
            {
                if (!JNetworkPrefabSet.NetworkPrefabSets.Contains(prefabSets[i])) JNetworkPrefabSet.NetworkPrefabSets.Add(prefabSets[i]);
            }
            for (int i = 0; i < jllAddons.Length; i++)
            {
                try
                {
                    jllAddons[i].Init(this);
                }
                catch (Exception e)
                {
                    JLogHelper.LogError($"JLL Addon failed to initialize from {GUID()}: {e}");
                }
            }
        }

        public JNetworkPrefabSet? GetNetworkPrefabSet(string name)
        {
            for (int i = 0; i < prefabSets.Length; i++)
            {
                if (prefabSets[i].SetName == name)
                {
                    return prefabSets[i];
                }
            }
            return null;
        }
    }
}
