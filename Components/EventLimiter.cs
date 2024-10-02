using GameNetcodeStuff;
using UnityEngine.Events;

namespace JLL.Components
{
    public class EventLimiter
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
    }
}
