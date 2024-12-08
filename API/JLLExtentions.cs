using GameNetcodeStuff;
using JLL.Components;
using System.Collections.Generic;
using System.Linq;

namespace JLL.API
{
    public static class JLLExtentions
    {
        public static T GetWeightedRandom<T>(this IEnumerable<T> weightedItems) where T : class, IWeightedItem
        {
            T[] array = weightedItems.ToArray();
            return array[IWeightedItem.GetRandomIndex(array)];
        }
        public static T GetWeightedRandom<T>(this IEnumerable<T> weightedItems, System.Random random) where T : class, IWeightedItem
        {
            T[] array = weightedItems.ToArray();
            return array[IWeightedItem.GetRandomIndex(random, array)];
        }

        public static bool IsLocalPlayer(this PlayerControllerB player)
        {
            if (GameNetworkManager.Instance != null && GameNetworkManager.Instance.localPlayerController != null)
            {
                return player.actualClientId == GameNetworkManager.Instance.localPlayerController.actualClientId;
            }
            return false;
        }

        public static int CombinedWeights<T>(this IEnumerable<T> weightedItems) where T : class, IWeightedItem
        {
            int combinedWeights = 0;
            foreach (T weightedItem in weightedItems)
            {
                combinedWeights += weightedItem.GetWeight();
            }
            return combinedWeights;
        }
    }
}
