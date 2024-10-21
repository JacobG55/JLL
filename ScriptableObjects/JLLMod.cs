using System;
using System.Collections.Generic;
using UnityEngine;

namespace JLL.ScriptableObjects
{
    [CreateAssetMenu(menuName = "JLL/JLLMod")]
    public class JLLMod : ScriptableObject
    {
        public string modAuthor = "";
        public string modName = "";

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

        internal void Init()
        {
            for (int i = 0; i < prefabSets.Length; i++)
            {
                if (!JNetworkPrefabSet.NetworkPrefabSets.Contains(prefabSets[i])) JNetworkPrefabSet.NetworkPrefabSets.Add(prefabSets[i]);
            }
        }

        public JNetworkPrefabSet? GetNetPrefabSet(string name)
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
