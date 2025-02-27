using UnityEngine;

namespace WesleyMoonScripts.Components
{
    public class ProggressionObject : MonoBehaviour
    {
        public bool destroy = true;

        private void Start()
        {
            if (WesleyScripts.ProtectionEnabled()) return;

            if (destroy) Destroy(gameObject);
            else gameObject.SetActive(false);
        }
    }
}
