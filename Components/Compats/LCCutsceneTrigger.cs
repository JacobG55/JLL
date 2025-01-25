using JLL.API;
using JLL.API.Compatability;
using UnityEngine;

namespace JLL.Components.Compats
{
    public class LCCutsceneTrigger : MonoBehaviour, CompatibilityComponent
    {
        public Transform cameraHolder;
        public float cutsceneLength = 4f;

        [Header("Player")]
        public bool allowDeath = true;
        public bool allowMovement = false;

        [Header("Transitions")]
        public float startTransition = 1f;
        public float endTransition = 1f;

        [Header("Animator")]
        public Animator? animator;
        public string triggerName = "Cutscene";

        private static bool? Loaded = null;
        public bool IsModLoaded()
        {
            Loaded ??= JCompatabilityHelper.IsLoaded(JCompatabilityHelper.CachedMods.LCCutscene);
            return Loaded.Value;
        }

        public void StartCutscene()
        {
            if (IsModLoaded())
            {
                CutsceneHelper.StartCutscene(cameraHolder, cutsceneLength, allowDeath, allowMovement, startTransition, endTransition);
                if (animator != null )
                {
                    animator.ResetTrigger(triggerName);
                    animator.SetTrigger(triggerName);
                }
            }
        }
    }
}
