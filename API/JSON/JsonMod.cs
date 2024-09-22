using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using JLL.API.JSON.Objects;
using System;
using System.Collections.Generic;
using System.IO;

namespace JLL.API.JSON
{
    public class JsonModRegistry
    {
        public static Dictionary<string, JsonModEntry> Mods = new Dictionary<string, JsonModEntry>();

        public static bool Register(string guid, JsonModEntry entry)
        {
            if (Mods.ContainsKey(guid))
            {
                JLogHelper.LogWarning($"Found duplicate entry of {guid}, skipping for now.");
                return false;
            }
            Mods.Add(guid, entry);
            return true;
        }

        public static int RegisterMods(List<JsonModSettings> jsonMods)
        {
            int registeredMods = 0;
            foreach (JsonModSettings mod in jsonMods)
            {
                string guid = $"{mod.modAuthor}.{mod.modName}";
                JsonModEntry jsonMod = JsonModEntry.Create(guid);

                if (Register(guid, jsonMod))
                {
                    registeredMods++;

                    foreach (var config in mod.configOptions)
                    {
                        try
                        {
                            switch (config.type.ToLower())
                            {
                                case "bool":
                                    List<string> trueValues = new List<string>() { "true", "1", "t" };
                                    jsonMod.RegisterConfig(config, trueValues.Contains(config.defaultValue.ToLower()));
                                    break;
                                case "int":
                                    int intDefault = 1;
                                    if (int.TryParse(config.defaultValue, out int intVal))
                                    {
                                        intDefault = intVal;
                                    }
                                    jsonMod.RegisterConfig(config, intDefault);
                                    break;
                                case "float":
                                    float floatDefault = 1.0f;
                                    if (float.TryParse(config.defaultValue, out float floatVal))
                                    {
                                        floatDefault = floatVal;
                                    }
                                    jsonMod.RegisterConfig(config, floatDefault);
                                    break;
                                case "string":
                                    jsonMod.RegisterConfig(config, config.defaultValue);
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            JLogHelper.LogWarning($"Error adding config option {config.configName} to {guid}.\n{e}");
                        }
                    }

                    /*
                    if (JCompatabilityHelper.IsModLoaded.LethalConfig)
                    {
                        LethalConfigHelper.CreateJsonModConfigs(jsonMod);
                    }
                    */
                }
            }
            return registeredMods;
        }
    }

    public class JsonModEntry
    {
        public string GUID;
        private ManualLogSource Logger;

        public ConfigFile Config { get; }
        public Dictionary<string, KeyValuePair<ConfigEntryBase, JsonModProperties>> Configs { get; } = new Dictionary<string, KeyValuePair<ConfigEntryBase, JsonModProperties>>();

        public enum EntryType
        {
            Unknown,
            Bool,
            Int,
            Float,
            String,
        }

        public class JsonModProperties
        {
            public EntryType type;
            public bool isSlider = false;
            public int Min = 0;
            public int Max = 100;
        }

        private JsonModEntry(string id)
        {
            GUID = id;
            Config = new ConfigFile(Path.Combine(Paths.ConfigPath, "JLL", $"{id}.cfg"), true);
            Logger = BepInEx.Logging.Logger.CreateLogSource($"JLL.{id}");
        }

        public static JsonModEntry Create(string guid)
        {
            JsonModEntry modEntry = new JsonModEntry(guid);
            return modEntry;
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

        public void RegisterConfig<T>(JsonConfigValue config, T defaultValue)
        {
            if (Configs.ContainsKey(config.configName)) return;

            EntryType entryType = EntryType.Unknown;
            if (defaultValue is bool)
            {
                entryType = EntryType.Bool;
            }
            else if (defaultValue is int)
            {
                entryType = EntryType.Int;
            }
            else if (defaultValue is float)
            {
                entryType = EntryType.Float;
            }
            else if (defaultValue is string)
            {
                entryType = EntryType.String;
            }

            LogInfo($"Registered config for {GUID}: {config.configName} default({defaultValue})");

            Configs.Add(config.configName, new KeyValuePair<ConfigEntryBase, JsonModProperties>(
                Config.Bind(config.configCategory, config.configName, defaultValue, config.configDescription), 
                new JsonModProperties { type = entryType, isSlider = config.isSlider, Max = config.max, Min = config.min }
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
