using UnityEngine;

namespace JLL.Components
{
    public class ChargeLimiter : MonoBehaviour
    {
        public int charges = 1;
        public InteractTrigger trigger;

        public void Charge()
        {
            charges -= 1;
            if (charges <= 0 )
            {
                SetHasCharge(false);
            }
        }

        public void SetHasCharge(bool value)
        {
            if (trigger != null)
            {
                if (trigger.TryGetComponent<BoxCollider>(out BoxCollider collider))
                {
                    collider.enabled = value;
                }
            }
            gameObject.SetActive(value);
        }
    }
}
