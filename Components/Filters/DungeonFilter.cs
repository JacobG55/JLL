using DunGen.Graph;
using JLL.API;
using JLL.API.Compatability;
using UnityEngine;

namespace JLL.Components.Filters
{
    public class DungeonFilter : JFilter<DungeonFlow>, IDungeonLoadListener
    {
        public bool checkPostDunGen = true;

        [Header("Dungeon Properties")]
        public NameFilter dungeonName = new NameFilter();
        public string[] dungeonContentTags = new string[0];
        public bool mustHaveAllDungeonTags = true;

        public override void Filter(DungeonFlow dungeon)
        {
            if (dungeon == null)
            {
                FilteredResult.Invoke(false);
                return;
            }

            if (JCompatabilityHelper.IsLoaded(JCompatabilityHelper.CachedMods.LethalLevelLoader))
            {
                if (!LLLHelper.ExtendedDungeonFilters(this, dungeon))
                {
                    goto Failed;
                }
            }

            Result(dungeon, true);
            return;

            Failed:
            Result(dungeon);

        }

        public override void FilterDefault()
        {
            if (RoundManager.Instance.currentDungeonType >= 0)
            {
                Filter(RoundManager.Instance.dungeonFlowTypes[RoundManager.Instance.currentDungeonType].dungeonFlow);
            }
        }

        public void FilterDungeon(int id)
        {
            if (id < 0 || id >= RoundManager.Instance.dungeonFlowTypes.Length)
            {
                JLogHelper.LogWarning($"Dungeon {id} is out of bounds of the DungeonFlowTypes array.");
            }
            else
            {
                Filter(RoundManager.Instance.dungeonFlowTypes[id].dungeonFlow);
            }
        }

        public void PostDungeonGeneration()
        {
            FilterDefault();
        }
    }
}
