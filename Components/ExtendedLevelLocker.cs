using JLL.API;
using JLL.API.Compatability;
using UnityEngine;

namespace JLL.Components
{
    public class ExtendedLevelLocker : MonoBehaviour
    {
        public string sceneName;

        [Header("OnTriggerEnter")]
        public bool isTriggerActivated = false;
        public bool shouldUnlock = true;

        public void SetLevelLocked(bool locked)
        {
            if (JCompatabilityHelper.IsLoaded(JCompatabilityHelper.CachedMods.LethalLevelLoader))
            {
                LLLHelper.LockExtendedLevel(sceneName, locked);
            }
        }
        public void SetLevelHidden(bool hidden)
        {
            if (JCompatabilityHelper.IsLoaded(JCompatabilityHelper.CachedMods.LethalLevelLoader))
            {
                LLLHelper.HideExtendedLevel(sceneName, hidden);
            }
        }

        public void LockLevel(string sceneName)
        {
            if (JCompatabilityHelper.IsLoaded(JCompatabilityHelper.CachedMods.LethalLevelLoader))
            {
                LLLHelper.LockExtendedLevel(sceneName, shouldUnlock);
            }
        }
        public void HideLevel(string sceneName)
        {
            if (JCompatabilityHelper.IsLoaded(JCompatabilityHelper.CachedMods.LethalLevelLoader))
            {
                LLLHelper.HideExtendedLevel(sceneName, shouldUnlock);
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
