using JLL.API;
using System;

namespace JLL.Components
{
    public interface IDungeonLoadListener
    {
        public abstract void PostDungeonGeneration();
    }

    public interface IWeightedItem
    {
        public int GetWeight();

        public static int GetRandomIndex(IWeightedItem[] weightedItems)
        => GetRandomIndex(UnityEngine.Random.Range(0, weightedItems.CombinedWeights() + 1), weightedItems, () => UnityEngine.Random.Range(0, weightedItems.Length));

        public static int GetRandomIndex(System.Random random, IWeightedItem[] weightedItems)
        => GetRandomIndex(random.Next(0, weightedItems.CombinedWeights() + 1), weightedItems, () => random.Next(0, weightedItems.Length));

        private static int GetRandomIndex(int random, IWeightedItem[] weightedItems, Func<int> failState)
        {
            for (int i = 0; i < weightedItems.Length; i++)
            {
                random -= weightedItems[i].GetWeight();
                if (random <= 0)
                {
                    return i;
                }
            }
            return failState.Invoke();
        }
    }
}
