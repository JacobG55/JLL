using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class JLevelEventTriggers : MonoBehaviour
    {
        public static List<JLevelEventTriggers> EventTriggers = new List<JLevelEventTriggers>();

        public UnityEvent ShipLanded = new UnityEvent();
        public UnityEvent LevelLoaded = new UnityEvent();
        public bool onlyOnFirstApparatus = false;
        private bool apparatusWasPulled = false;
        public UnityEvent ApparatusPulled = new UnityEvent();

        public HourEvent[] hourlyEvents = new HourEvent[0];
        private int prevHour = 0;

        [Serializable]
        public class HourEvent
        {
            public UnityEvent hourEvent = new UnityEvent();
            [Range(0, 7)]
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
