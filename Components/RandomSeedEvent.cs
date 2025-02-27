using JLL.API;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace JLL.Components
{
    public class RandomSeedEvent : MonoBehaviour, IDungeonLoadListener
    {
        private System.Random Random;
        [Tooltip("Value gets added to level seed to determine random seed.")]
        public int relativeSeed = 25;
        [FormerlySerializedAs("rollOnStart")]
        public bool rollOnDungeonLoad = true;

        public WeightedPosEvent[] Events = new WeightedPosEvent[] { new WeightedPosEvent() };

        [Serializable]
        public class WeightedPosEvent : IWeightedItem
        {
            public TransformMove[] MoveObjects = new TransformMove[0];
            public UnityEvent Event = new UnityEvent();

            [Range(0f, 100f)]
            public int Weight = 20;
            public int GetWeight() => Weight;
        }

        [Serializable]
        public class TransformMove
        {
            public Transform Object;
            public Transform TargetLocation;
            public void Move()
            {
                if (Object != null && TargetLocation != null)
                {
                    Object.position = TargetLocation.position;
                    Object.rotation = TargetLocation.rotation;
                }
            }
        }

        public void PostDungeonGeneration()
        {
            Random ??= new System.Random(RoundManager.Instance.playersManager.randomMapSeed + relativeSeed);
            if (rollOnDungeonLoad)
            {
                Roll();
            }
        }

        public void Roll()
        {
            if (Random == null) return;
            WeightedPosEvent posEvent = Events.GetWeightedRandom(Random);

            foreach (TransformMove tMove in posEvent.MoveObjects)
            {
                tMove.Move();
            }
            posEvent.Event.Invoke();
        }
    }
}
