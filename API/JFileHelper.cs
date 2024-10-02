using BepInEx;
using JLL.API.Compatability;
using JLL.API.JSON.Objects;
using JLL.API.LevelProperties;
using JLL.ScriptableObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace JLL.API
{
    public class JFileHelper
    {
        public readonly static List<AssetBundle> JLLBundles = new List<AssetBundle>();

        public static bool JLLBundlesLoaded { private set; get; } = false;
        public static bool LLLBundlesLoaded { internal set; get; } = true;

        private static int filesRead = 0;
        private static int jllMods = 0;
        private static int levelPropertyOverrides = 0;

        public static string AsJson<T>(T type, bool formatted = true)
        {
            return JsonConvert.SerializeObject(type, formatted ? Formatting.Indented : Formatting.None);
        }

        public static bool ConvertJson<T>(string json, out T output)
        {
            output = JsonConvert.DeserializeObject<T>(json);
            if (output != null)
            {
                return true;
            }
            return false;
        }

        public static string[] GetJsonFilesInDirectory(string directory, bool searchSubs = true)
        {
            return GetFilesInDirectory(directory, new List<string> { ".json" }, searchSubs);
        }

        public static string[] GetFilesInDirectory(string directory, List<string> extensions, bool searchSubs = true)
        {
            return Directory.GetFiles(directory, "*.*", searchSubs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Where(file => extensions.Any(extension => file.EndsWith(extension))).ToArray();
        }

        public static List<string> GetTextFromFiles(string[] files)
        {
            List<string> contents = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                try
                {
                    contents.Add(File.ReadAllText(files[i]));
                }
                catch
                {
                    JLogHelper.LogWarning($"Something went wrong reading: {files[i]}");
                }
            }
            return contents;
        }

        public static List<T> ParseJsonFiles<T>(string[] files)
        {
            List<T> contents = new List<T>();
            for (int i = 0; i < files.Length; i++)
            {
                try
                {
                    if (ConvertJson(File.ReadAllText(files[i]), out T obj))
                    {
                        contents.Add(obj);
                    }
                }
                catch
                {
                    JLogHelper.LogWarning($"Something went wrong reading: {files[i]}");
                }
            }
            return contents;
        }

        public static List<T> ParseJsonFiles<T>(string directory)
        {
            return ParseJsonFiles<T>(GetJsonFilesInDirectory(directory));
        }

        public static void LoadFilesInPlugins()
        {
            if (JCompatabilityHelper.IsModLoaded.LLL)
            {
                JLogHelper.LogInfo("Linking to LLL", JLogLevel.Debuging);
                LLLHelper.LinkBundlesLoadedEvent();
            }

            JLogHelper.LogInfo($"Searching for Bundles and JSONs....", JLogLevel.User);
            string[] files = Directory.GetFiles(Paths.PluginPath, "*.*", true ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            for (int x = 0; x < files.Length; x++)
            {
                string lower = files[x].ToLower();
                if (lower.EndsWith("jllmod.json"))
                {
                    JLogHelper.LogInfo($"Parsing JSON: {files[x]}", JLogLevel.Debuging);
                    filesRead++;
                    try
                    {
                        if (ConvertJson(File.ReadAllText(files[x]), out JsonModSettings modSettings))
                        {
                            if (CustomConfigRegistry.RegisterMod(modSettings.ToMod()))
                            {
                                jllMods++;
                                for (int i = 0; i < modSettings.levelPropertyOverrides.Length; i++)
                                {
                                    levelPropertyOverrides++;
                                    JLevelPropertyRegistry.RegisterLevelProperties(new JLevelPropertyEntry { sceneName = modSettings.levelPropertyOverrides[i].sceneName, enemyPropertyOverrides = modSettings.levelPropertyOverrides[i].enemyPropertyOverrides });
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        JLogHelper.LogWarning($"Error Parsing {files[x]}\n{e}");
                    }
                }
                else if (lower.EndsWith(".jllmod") || (!JCompatabilityHelper.IsModLoaded.LLL && lower.EndsWith(".lethalbundle")))
                {
                    JLogHelper.LogInfo($"Parsing Bundle: {files[x]}", JLogLevel.Debuging);
                    filesRead++;
                    AssetBundle bundle = AssetBundle.LoadFromFile(files[x]);
                    JLLBundles.Add(bundle);
                }
            }

            JLogHelper.LogInfo($"Finished reading {filesRead} files.", JLogLevel.User);

            JLLBundlesLoaded = true;
            SearchAllBundles();
        }

        internal static void SearchAllBundles()
        {
            if (!(JLLBundlesLoaded && LLLBundlesLoaded))
            {
                return;
            }

            foreach (var bundle in AssetBundle.GetAllLoadedAssetBundles())
            {
                if (bundle.isStreamedSceneAssetBundle)
                {
                    continue;
                }
                try
                {
                    JLLMod[] jllConfigMods = bundle.LoadAllAssets<JLLMod>();

                    for (int i = 0; i < jllConfigMods.Length; i++)
                    {
                        jllMods++;
                        CustomConfigRegistry.RegisterMod(jllConfigMods[i]);
                    }

                    JLevelProperties[] levelProperties = bundle.LoadAllAssets<JLevelProperties>();

                    for (int i = 0; i < levelProperties.Length; i++)
                    {
                        levelPropertyOverrides++;
                        JLevelPropertyRegistry.RegisterLevelProperties(levelProperties[i].Properties);
                    }
                }
                catch (Exception e) 
                {

                    JLogHelper.LogWarning($"Failed to read a bundle!\n{e}");
                }
            }

            JLogHelper.LogInfo($"Successfully Registered {jllMods} JLLMods and {levelPropertyOverrides} LevelPropertyOverrides.", JLogLevel.User);
        }
    }
}
