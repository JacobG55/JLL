using UnityEngine;
using UnityEngine.Events;
using static JLL.API.JCompatabilityHelper;

namespace JLL.Components
{
    public class ModLoadedCheck : MonoBehaviour
    {
        public bool checkOnStart = true;
        public CachedMods checkedMod = CachedMods.Other;
        public string modGUID = "JacobG5.JLL";

        [Tooltip("Disables GameObject when the select mod isn't loaded.")]
        public GameObject? hideWhenMissing;

        public UnityEvent ModFound = new UnityEvent();
        public UnityEvent ModMissing = new UnityEvent();

        void Start()
        {
            if (checkOnStart) CheckForMod();
        }

        public void CheckForMod()
        {
            if (checkedMod == CachedMods.Other ? IsLoaded(modGUID) : IsLoaded(checkedMod))
            {
                ModFound.Invoke();
            }
            else
            {
                ModMissing.Invoke();
                hideWhenMissing?.SetActive(false);
            }
        }
    }
}
