using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components.Helpers
{
    public class BooleanHelper : MonoBehaviour
    {
        public UnityEvent OnTrue = new UnityEvent();
        public UnityEvent OnFalse = new UnityEvent();

        public void Split(bool value)
        {
            if (value)
            {
                OnTrue.Invoke();
            }
            else
            {
                OnFalse.Invoke();
            }
        }

        public BooleanEvent InverseEvent = new BooleanEvent();

        public void Inverse(bool value)
        {
            InverseEvent.Invoke(!value);
        }
    }
}
