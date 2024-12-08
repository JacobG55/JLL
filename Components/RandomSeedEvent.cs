using JLL.API;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class RandomSeedEvent : MonoBehaviour
    {
        [Tooltip("Value gets added to level seed to determine random seed.")]
        public int relativeSeed = 25;
        public bool rollOnStart = true;

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

        [HideInInspector]
        public int Seed => StartOfRound.Instance.randomMapSeed + relativeSeed;

        public void Roll()
        {
            System.Random random = new System.Random(Seed);
            WeightedPosEvent posEvent = Events.GetWeightedRandom(random);

            foreach (TransformMove tMove in posEvent.MoveObjects)
            {
                tMove.Move();
            }
            posEvent.Event.Invoke();
        }

        void Start()
        {
            if (rollOnStart)
            {
                Roll();
            }
        }
    }
}
