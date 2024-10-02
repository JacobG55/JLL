using JLL.API.LevelProperties;
using JLL.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

namespace JLL.API.JSON.Objects
{
    public class JsonModSettings
    {
        public string modAuthor = "";
        public string modName = "";

        public JsonConfigValue[] configOptions = new JsonConfigValue[0];
        public JsonLevelPropertyOverrides[] levelPropertyOverrides = new JsonLevelPropertyOverrides[0];

        public JLLMod ToMod()
        {
            JLLMod mod = ScriptableObject.CreateInstance<JLLMod>();
            mod.modAuthor = modAuthor;
            mod.modName = modName;

            foreach (var config in configOptions)
            {
                switch (config.type.ToLower())
                {
                    case "bool":
                        List<string> trueValues = new List<string>() { "true", "1", "t" };
                        mod.Booleans.Add(ParseConfig(config, trueValues.Contains(config.defaultValue.ToLower())));
                        break;
                    case "int":
                        int intDefault = 1;
                        if (int.TryParse(config.defaultValue, out int intVal))
                        {
                            intDefault = intVal;
                        }
                        mod.Integers.Add(ParseConfig(config, intDefault));
                        break;
                    case "float":
                        float floatDefault = 1.0f;
                        if (float.TryParse(config.defaultValue, out float floatVal))
                        {
                            floatDefault = floatVal;
                        }
                        mod.Floats.Add(ParseConfig(config, floatDefault));
                        break;
                    case "string":
                        mod.Strings.Add(ParseConfig(config, config.defaultValue));
                        break;
                }
            }

            return mod;
        }

        private JLLMod.ConfigValue<T> ParseConfig<T>(JsonConfigValue config, T defaultValue)
        {
            return new JLLMod.ConfigValue<T>
            {
                configName = config.configName,
                configCategory = config.configCategory,
                configDescription = config.configDescription,
                defaultValue = defaultValue
            };
        }
    }

    public class JsonConfigValue
    {
        public string configName = "Example";
        public string configCategory = "Main";
        public string configDescription = "";
        public string type = "bool";
        public string defaultValue = "false";
        public bool isSlider = false;
        public int max = 100;
        public int min = 0;
    }

    public class JsonLevelPropertyOverrides
    {
        public string sceneName = "";
        public EnemyPropertyOverride[] enemyPropertyOverrides = new EnemyPropertyOverride[0];
    }

    public class JsonVector
    {
        public float x = 0;
        public float y = 0;
        public float z = 0;

        public Vector3 GetUnityVector()
        {
            return new Vector3(x, y, z);
        }

        public static JsonVector Create(Vector3 vector)
        {
            return new JsonVector { x = vector.x, y = vector.y, z = vector.z };
        }
    }
}
