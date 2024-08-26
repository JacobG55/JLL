using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class JLevelEventTriggers : MonoBehaviour
    {
        public UnityEvent ShipLanded;
        public UnityEvent LevelLoaded;
        public UnityEvent ApparatusPulled;

        public static List<JLevelEventTriggers> EventTriggers = new List<JLevelEventTriggers>();

        public void Start()
        {
            EventTriggers.Add(this);
            LevelLoaded.Invoke();
        }
    }
}
