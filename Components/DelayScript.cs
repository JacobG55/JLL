using GameNetcodeStuff;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class DelayScript : MonoBehaviour
    {
        public float delaySeconds = 2f;
        public UnityEvent events = new UnityEvent();
        public InteractEvent playerEvents = new InteractEvent();

        public void StartWaiting(PlayerControllerB player)
        {
            StartCoroutine(WaitForDelay(player));
        }

        public void StartWaiting()
        {
            StartCoroutine(WaitForDelay());
        }

        private IEnumerator WaitForDelay(PlayerControllerB? player = null)
        {
            yield return new WaitForSeconds(delaySeconds);
            events.Invoke();
            if (player != null )
            {
                playerEvents.Invoke(player);
            }
        }
    }
}
