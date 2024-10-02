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
    }
}
