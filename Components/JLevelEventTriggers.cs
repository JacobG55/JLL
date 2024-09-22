using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class JLevelEventTriggers : MonoBehaviour
    {
        public static List<JLevelEventTriggers> EventTriggers = new List<JLevelEventTriggers>();

        [Tooltip("Invoked after the ship landing animation finishes")]
        public UnityEvent ShipLanded = new UnityEvent();
        [Tooltip("Invoked when the level loads on the client")]
        public UnityEvent LevelLoaded = new UnityEvent();
        [Tooltip("Invoked when the ship leaves")]
        public UnityEvent ShipLeaving = new UnityEvent();
        [Tooltip("If you only want the Apparatus event to run 1 time then check this so interiors with multiple Apparatuses don't break things")]
        public bool onlyOnFirstApparatus = false;
        private bool apparatusWasPulled = false;
        [Tooltip("Invoked when an Apparatus gets pulled inside the facility")]
        public UnityEvent ApparatusPulled = new UnityEvent();

        public HourEvent[] hourlyEvents = new HourEvent[0];
        private int prevHour = 0;

        [Serializable]
        public class HourEvent
        {
            public UnityEvent hourEvent = new UnityEvent();
            [Range(0, 18)]
            public int hour;
        }

        public void Start()
        {
            EventTriggers.Add(this);
            LevelLoaded.Invoke();
        }

        public void FixedUpdate()
        {
            if (hourlyEvents.Length > 0)
            {
                int hour = TimeOfDay.Instance.hour;
                if (prevHour != hour)
                {
                    for (int i = 0; i < hourlyEvents.Length; i++)
                    {
                        if (hourlyEvents[i].hour == hour)
                        {
                            hourlyEvents[i].hourEvent.Invoke();
                        }
                    }
                }
                prevHour = hour;
            }
        }

        public void InvokeApparatus()
        {
            if (onlyOnFirstApparatus && apparatusWasPulled) return;
            ApparatusPulled.Invoke();
            apparatusWasPulled = true;
        }
    }
}
