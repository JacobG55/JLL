using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class DelayScript : MonoBehaviour
    {
        public float delaySeconds = 2f;
        public UnityEvent events = new UnityEvent();

        public void StartWaiting()
        {
            StartCoroutine(WaitForDelay());
        }

        private IEnumerator WaitForDelay()
        {
            yield return new WaitForSeconds(delaySeconds);
            events.Invoke();
        }
    }
}
