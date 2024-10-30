using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using JLL.ScriptableObjects;
using System.Collections.Generic;
using System.IO;

namespace JLL.API
{
    public class CustomConfigRegistry
    {
        public static Dictionary<string, CustomConfigEntry> Mods = new Dictionary<string, CustomConfigEntry>();

        public static bool Register(string guid, CustomConfigEntry entry)
        {
            if (Mods.ContainsKey(guid))
            {
                JLogHelper.LogWarning($"Found duplicate entry of {guid}, skipping for now.");
                return false;
            }
            Mods.Add(guid, entry);
            return true;
        }

        public static bool RegisterMod(JLLMod mod)
        {
            CustomConfigEntry configEntry = CustomConfigEntry.Create(mod);

            if (Register(configEntry.GUID, configEntry))
            {
                foreach (var config in mod.Booleans)
                {
                    configEntry.RegisterConfig(config);
                }
                foreach (var config in mod.Strings)
                {
                    configEntry.RegisterConfig(config);
                }
                foreach (var config in mod.Integers)
                {
                    configEntry.RegisterConfig(config);
                }
                foreach (var config in mod.Floats)
                {
                    configEntry.RegisterConfig(config);
                }
                return true;
            }
            return false;
        }
    }

    public class CustomConfigEntry
    {
        public string GUID;
        private ManualLogSource Logger;
        public JLLMod ScriptableObject;

        public ConfigFile Config { get; }
        public Dictionary<string, KeyValuePair<ConfigEntryBase, JLLModProperties>> Configs { get; } = new Dictionary<string, KeyValuePair<ConfigEntryBase, JLLModProperties>>();

        public enum EntryType
        {
            Unknown,
            Bool,
            Int,
            Float,
            String,
        }

        public class JLLModProperties
        {
            public EntryType type;
        }

        private CustomConfigEntry(JLLMod mod)
        {
            ScriptableObject = mod;
            GUID = mod.GUID();
            Config = new ConfigFile(Path.Combine(Paths.ConfigPath, "JLL", $"{GUID}.cfg"), true);
            Logger = BepInEx.Logging.Logger.CreateLogSource($"JLL.{GUID}");
        }

        public static CustomConfigEntry Create(JLLMod jllMod)
        {
            return new CustomConfigEntry(jllMod);
        }

        public void LogInfo(string message, JLogLevel level = JLogLevel.Debuging)
        {
            if (JLogHelper.AcceptableLogLevel(level))
            {
                Logger.LogInfo(message);
            }
        }

        public void LogWarning(string message, JLogLevel level = JLogLevel.Debuging)
        {
            if (JLogHelper.AcceptableLogLevel(level))
            {
                Logger.LogWarning(message);
            }
        }

        public void RegisterConfig<T>(JLLMod.ConfigValue<T> config)
        {
            if (Configs.ContainsKey(config.configName)) return;

            EntryType entryType = EntryType.Unknown;
            if (config.defaultValue is bool)
            {
                entryType = EntryType.Bool;
            }
            else if (config.defaultValue is int)
            {
                entryType = EntryType.Int;
            }
            else if (config.defaultValue is float)
            {
                entryType = EntryType.Float;
            }
            else if (config.defaultValue is string)
            {
                entryType = EntryType.String;
            }

            LogInfo($"Registered config for {GUID}: {config.configName} default({config.defaultValue})");

            Configs.Add(config.configName, new KeyValuePair<ConfigEntryBase, JLLModProperties>(
                Config.Bind(config.configCategory, config.configName, config.defaultValue, config.configDescription),
                new JLLModProperties { type = entryType }
            ));
        }

        public ConfigEntry<T>? GetConfigEntry<T>(string name)
        {
            if (Configs.ContainsKey(name) && Configs[name].Key is ConfigEntry<T> Entry)
            {
                return Entry;
            }
            return null;
        }

        public bool GetBool(string name)
        {
            ConfigEntry<bool>? config = GetConfigEntry<bool>(name);
            if (config != null)
            {
                return config.Value;
            }
            return false;
        }

        public int GetInt(string name)
        {
            ConfigEntry<int>? config = GetConfigEntry<int>(name);
            if (config != null)
            {
                return config.Value;
            }
            return -1;
        }

        public float GetFloat(string name)
        {
            ConfigEntry<float>? config = GetConfigEntry<float>(name);
            if (config != null)
            {
                return config.Value;
            }
            return -1;
        }

        public string GetString(string name)
        {
            ConfigEntry<string>? config = GetConfigEntry<string>(name);
            if (config != null)
            {
                return config.Value;
            }
            return "";
        }
    }
}
