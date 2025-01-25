using DunGen;
using JLL.Components;
using System.Collections.Generic;
using UnityEngine;

namespace WesleyMoonScripts.Components
{
    [RequireComponent(typeof(Doorway))]
    public class GrandArmoryBlocker : MonoBehaviour, IDungeonLoadListener
    {
        private Doorway doorway;
        public List<GameObjectWeight> BlockerPrefabWeights = new List<GameObjectWeight>();

        void Start()
        {
            doorway = GetComponent<Doorway>();
        }

        public void PostDungeonGeneration()
        {
            if (doorway.ConnectedDoorway == null)
            {
                Instantiate(BlockerPrefabWeights.GetRandom(RoundManager.Instance.dungeonGenerator.Generator.RandomStream), transform.position, transform.rotation, transform);
            }
        }
    }
}
