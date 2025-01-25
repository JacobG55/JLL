using DunGen.Graph;
using JLL.Components.Filters;
using LethalLevelLoader;
using System.Collections.Generic;
using UnityEngine;

namespace JLL.API.Compatability
{
    public class LLLHelper
    {
        public static void LockExtendedLevel(string sceneName, bool isLocked = true)
        {
            ExtendedLevel? extendedLevel = GetLevel(sceneName);
            if (extendedLevel != null)
            {
                extendedLevel.IsRouteLocked = isLocked;
                JLogHelper.LogInfo($"{extendedLevel.SelectableLevel.PlanetName} Locked: {extendedLevel.IsRouteLocked}");
            }
        }

        public static void HideExtendedLevel(string sceneName, bool isHidden = true)
        {
            ExtendedLevel? extendedLevel = GetLevel(sceneName);
            if (extendedLevel != null)
            {
                extendedLevel.IsRouteHidden = isHidden;
                JLogHelper.LogInfo($"{extendedLevel.SelectableLevel.PlanetName} Hidden: {extendedLevel.IsRouteHidden}");
            }
        }

        public static ExtendedLevel? GetLevel(SelectableLevel level) => GetLevel(level.sceneName);
        public static ExtendedLevel? GetLevel(string sceneName)
        {
            foreach (ExtendedLevel level in PatchedContent.ExtendedLevels)
            {
                if (level.SelectableLevel.sceneName == sceneName)
                {
                    return level;
                }
            }
            return null;
        }

        public static ExtendedDungeonFlow? GetDungeon(DungeonFlow flow)
        {
            for (int i = 0; i < PatchedContent.ExtendedDungeonFlows.Count; i++)
            {
                if (PatchedContent.ExtendedDungeonFlows[i].DungeonFlow == flow)
                {
                    return PatchedContent.ExtendedDungeonFlows[i];
                }
            }
            return null;
        }

        public static ExtendedItem? GetItem(Item item)
        {
            for (int i = 0; i < PatchedContent.ExtendedItems.Count; i++)
            {
                if (PatchedContent.ExtendedItems[i].Item == item)
                {
                    return PatchedContent.ExtendedItems[i];
                }
            }
            return null;
        }

        public static bool LevelHasTag(SelectableLevel level, string tag)
        {
            ExtendedLevel? extended = GetLevel(level);
            if (extended != null)
            {
                return extended.TryGetTag(tag);
            }
            return false;
        }

        public static bool ItemHasTag(Item item, string tag)
        {
            ExtendedItem? extended = GetItem(item);
            if (extended != null)
            {
                return extended.TryGetTag(tag);
            }
            return false;
        }

        public static List<string> GetItemTags(Item item)
        {
            ExtendedItem? extended = GetItem(item);
            if (extended != null)
            {
                return GetExtendedTags(extended);
            }
            return new List<string>();
        }

        public static List<string> GetLevelTags(SelectableLevel level)
        {
            ExtendedLevel? extended = GetLevel(level);
            if (extended != null)
            {
                return GetExtendedTags(extended);
            }
            return new List<string>();
        }

        public static List<string> GetExtendedTags(ExtendedContent content)
        {
            List<string> tags = new List<string>();
            for (int i = 0; i < content.ContentTags.Count; i++)
            {
                tags.Add(content.ContentTags[i].contentTagName);
            }
            return tags;
        }

        internal static bool ExtendedLevelFilters(LevelFilter levelFilter, SelectableLevel level)
        {
            ExtendedLevel? extendedLevel = GetLevel(level);

            if (extendedLevel != null)
            {
                if (levelFilter.contentTags.Length > 0 && !CheckTags(extendedLevel, levelFilter.contentTags, levelFilter.mustHaveAllTags)) return false;
                if (!levelFilter.routePrice.Check(extendedLevel.RoutePrice)) return false;
                if (!levelFilter.calculatedDifficulty.Check(extendedLevel.CalculatedDifficultyRating)) return false;
            }
            return true;
        }

        internal static bool ExtendedDungeonFilters(DungeonFilter dungeonFilter, DungeonFlow flow)
        {
            ExtendedDungeonFlow? extendedDungeon = GetDungeon(flow);

            if (extendedDungeon != null)
            {
                if (!dungeonFilter.dungeonName.Check(extendedDungeon.DungeonName)) return false;
                if (dungeonFilter.dungeonContentTags.Length > 0 && !CheckTags(extendedDungeon, dungeonFilter.dungeonContentTags, dungeonFilter.mustHaveAllDungeonTags)) return false;
            }
            else
            {
                JLogHelper.LogInfo($"({dungeonFilter.name}) Couldn't find ExtendedDungeonFlow");
            }
            return true;
        }

        internal static bool ItemTagFilter(Item item, string[] contentTags, bool mustHaveAllTags)
        {
            ExtendedItem? extendedItem = GetItem(item);

            if (extendedItem != null)
            {
                if (!CheckTags(extendedItem, contentTags, mustHaveAllTags))
                {
                    return false;
                }
                return true;
            }
            else
            {
                JLogHelper.LogInfo($"Extended Item couldn't be found for: {item.itemName}", JLogLevel.Wesley);
            }
            return false;
        }

        private static bool CheckTags(ExtendedContent content, string[] contentTags, bool mustHaveAllTags)
        {
            List<string> levelTags = GetExtendedTags(content);

            int foundItems = 0;
            for (int j = 0; j < contentTags.Length; j++)
            {
                for (int i = 0; i < levelTags.Count; i++)
                {
                    if (levelTags[i].ToLower().Equals(contentTags[j].ToLower()))
                    {
                        if (!mustHaveAllTags) return true;
                        foundItems++;
                        break;
                    }
                }
            }

            return foundItems == contentTags.Length;
        }

        internal static void LinkBundlesLoadedEvent()
        {
            JFileHelper.LLLBundlesLoaded = AssetBundleLoader.CurrentLoadingStatus == AssetBundleLoader.LoadingStatus.Complete;
            AssetBundleLoader.onBundlesFinishedLoading += LLLBundlesLoaded;
        }

        internal static void LLLBundlesLoaded()
        {
            JFileHelper.LLLBundlesLoaded = true;
            JFileHelper.SearchAllBundles();
        }
    }
}
