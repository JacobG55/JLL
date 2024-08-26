using JLL.API;
using JLL.API.Compatability;
using UnityEngine;

namespace JLL.Components
{
    public class ExtendedLevelLocker : MonoBehaviour
    {
        public SelectableLevel targetLevel;

        [Header("OnTriggerEnter")]
        public bool isTriggerActivated = false;
        public bool shouldUnlock = true;

        public void SetLevelLocked(bool locked)
        {
            if (JCompatabilityHelper.IsModLoaded.LLL)
            {
                LLLHelper.LockExtendedLevel(targetLevel, locked);
            }
        }
        public void SetLevelHidden(bool hidden)
        {
            if (JCompatabilityHelper.IsModLoaded.LLL)
            {
                LLLHelper.HideExtendedLevel(targetLevel, hidden);
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (isTriggerActivated)
            {
                if (other.CompareTag("Player"))
                {
                    SetLevelLocked(!shouldUnlock);
                }
            }
        }
    }
}
