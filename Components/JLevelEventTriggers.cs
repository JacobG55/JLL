using JLL.API;
using JLL.API.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class JLevelEventTriggers : MonoBehaviour, IDungeonLoadListener
    {
        public static List<JLevelEventTriggers> EventTriggers = [];

        [Tooltip("Invoked after the ship landing animation finishes")]
        public UnityEvent ShipLanded = new();
        [Tooltip("Invoked when the level loads on the client")]
        public UnityEvent LevelLoaded = new();
        [Tooltip("Invoked when the ship leaves")]
        public UnityEvent ShipLeaving = new();

        [Tooltip("If you only want the Apparatus event to run 1 time then check this so interiors with multiple Apparatuses don't break things")]
        public bool onlyOnFirstApparatus = false;
        private bool apparatusWasPulled = false;
        [Tooltip("Invoked when an Apparatus gets pulled inside the facility")]
        public UnityEvent ApparatusPulled = new();

        public bool breakerIgnoresApparatus = false;
        [Tooltip("Triggered on breaker box being toggled.")]
        public BoolEvent BreakerBox = new();

        public HourEvent[] hourlyEvents = [];

        [Serializable]
        public class HourEvent
        {
            public UnityEvent hourEvent = new();
            [Range(0, 18)]
            public int hour;
        }

        void OnEnable()
        {
            EventTriggers.Add(this);
            JLogHelper.LogInfo($"Enabled {name} LevelEventTrigger", JLogLevel.Wesley);
        }

        void OnDisable()
        {
            EventTriggers.Remove(this);
            JLogHelper.LogInfo($"Disabled {name} LevelEventTrigger", JLogLevel.Wesley);
        }

        public void InvokeApparatus()
        {
            if (onlyOnFirstApparatus && apparatusWasPulled) return;
            apparatusWasPulled = true;
            ApparatusPulled.Invoke();
        }

        public void ToggleBreakerBox(bool active)
        {
            if (apparatusWasPulled && !breakerIgnoresApparatus) return;
            BreakerBox.Invoke(active);
        }

        public void PostDungeonGeneration()
        {
            LevelLoaded.Invoke();
        }
    }
}
