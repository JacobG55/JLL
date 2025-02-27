using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class EventLimiter : MonoBehaviour
    {
        public int maxTriggers = 1;

        public UnityEvent Event = new UnityEvent();
        public InteractEvent PlayerEvent = new InteractEvent();

        public void Trigger()
        {
            Trigger(null);
        }

        public void Trigger(PlayerControllerB? player)
        {
            if (maxTriggers > 0)
            {
                maxTriggers--;

                Event.Invoke();

                if (player != null)
                {
                    PlayerEvent.Invoke(player);
                }
            }
        }

        public void AddUses(int num) => maxTriggers += num;
        public void SetUses(int num) => maxTriggers = num;
    }
}
