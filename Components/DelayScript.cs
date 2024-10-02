using GameNetcodeStuff;
using JLL.API;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace JLL.Components
{
    public class DelayScript : MonoBehaviour
    {
        [FormerlySerializedAs("waitOnAwake")]
        public bool waitOnEnabled = false;
        public bool clearOnDisable = false;
        public float delaySeconds = 2f;
        [Tooltip("Event run after StartWaiting() is called by another event.")]
        public UnityEvent events = new UnityEvent();
        [Tooltip("Only gets triggered if a player is given on the StartWaiting() call.")]
        public InteractEvent playerEvents = new InteractEvent();

        private readonly List<QueuedEvent> queuedEvents = new List<QueuedEvent>();
        private readonly List<QueuedEvent> expiredEvents = new List<QueuedEvent>();

        public class QueuedEvent
        {
            public PlayerControllerB? target = null;
            public float timer;
        }

        public void OnEnable()
        {
            if (waitOnEnabled)
            {
                StartWaiting();
            }
        }

        public void OnDisable()
        {
            if (clearOnDisable)
            {
                ClearEventQueue();
            }
        }

        public void Update()
        {
            for (int i = 0; i < queuedEvents.Count; i++)
            {
                queuedEvents[i].timer -= Time.deltaTime;
                if (queuedEvents[i].timer <= 0)
                {
                    expiredEvents.Add(queuedEvents[i]);
                }
            }
            if (expiredEvents.Count > 0)
            {
                for (int i = 0; i < expiredEvents.Count; i++)
                {
                    events.Invoke();

                    if (expiredEvents[i].target != null)
                    {
                        playerEvents.Invoke(expiredEvents[i].target);
                    }

                    queuedEvents.Remove(expiredEvents[i]);
                }
                expiredEvents.Clear();
            }
        }

        public void StartWaiting(PlayerControllerB player)
        {
            queuedEvents.Add(new QueuedEvent { timer = delaySeconds, target = player });
        }

        public void StartWaiting()
        {
            JLogHelper.LogInfo($"{name} started waiting {delaySeconds}", JLogLevel.Debuging);
            queuedEvents.Add(new QueuedEvent { timer = delaySeconds });
        }

        public void ClearEventQueue()
        {
            queuedEvents.Clear();
        }
    }
}
