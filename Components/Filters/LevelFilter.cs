using JLL.API;
using JLL.API.Compatability;
using UnityEngine;

namespace JLL.Components.Filters
{
    public class LevelFilter : JFilter<SelectableLevel>
    {
        public bool checkCurrentLevelOnEnable = true;

        [Header("Level Info")]
        public NameFilter sceneName = new NameFilter() { caseSensitive = true };
        public NameFilter planetName = new NameFilter();
        public string[] contentTags = new string[0];
        public bool mustHaveAllTags = true;

        [Header("Level Properties")]
        public IntFilter routePrice = new IntFilter() { value = 600 };
        public IntFilter calculatedDifficulty = new IntFilter();
        public NumericFilter scrapInside = new NumericFilter() { value = 500 };
        public IntFilter moldIterations = new IntFilter();

        public void OnEnable()
        {
            if (checkCurrentLevelOnEnable)
            {
                FilterCurrentLevel();
            }
        }

        public override void Filter(SelectableLevel level)
        {
            if (sceneName.shouldCheck && !sceneName.CheckValue(level.sceneName))
            {
                Result(level);
                return;
            }

            if (planetName.shouldCheck && !planetName.CheckValue(level.PlanetName))
            {
                Result(level);
                return;
            }

            if (scrapInside.shouldCheck && !scrapInside.CheckValue(RoundManager.Instance.totalScrapValueInLevel))
            {
                Result(level);
                return;
            }

            if (moldIterations.shouldCheck && !moldIterations.CheckValue(level.moldSpreadIterations))
            {
                Result(level);
                return;
            }

            if (JCompatabilityHelper.IsModLoaded.LLL)
            {
                if (!LLLHelper.ExtendedLevelFilters(this, level))
                {
                    Result(level);
                    return;
                }
            }

            Result(level, true);
        }

        public void FilterCurrentLevel()
        {
            Filter(RoundManager.Instance.currentLevel);
        }
    }
}
