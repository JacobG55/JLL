using GameNetcodeStuff;
using JLL.Components;
using System.Collections.Generic;
using System.Linq;

namespace JLL.API
{
    public static class JLLExtentions
    {
        internal static readonly Dictionary<PlayerControllerB, int> PlayerIndexes = new Dictionary<PlayerControllerB, int>();

        public static int Index(this PlayerControllerB player)
        {
            if (PlayerIndexes.TryGetValue(player, out var index)) return index;
            for (int i = 0; i < RoundManager.Instance.playersManager.allPlayerScripts.Length; i++)
            {
                if (RoundManager.Instance.playersManager.allPlayerScripts[i] == player)
                {
                    PlayerIndexes.Add(player, i);
                    return i;
                }
            }
            return -1;
        }

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
                return player == GameNetworkManager.Instance.localPlayerController;
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
