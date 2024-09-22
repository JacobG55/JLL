using UnityEngine;

namespace JLL.Components
{
    [RequireComponent (typeof(Animator))]
    public class DayCycleAnimator : MonoBehaviour
    {
        private Animator anim;

        [Tooltip("Scale of time the animation synced to the day progression will use. Defaults to 1 second.")]
        public float timeScale = 1f;

        public void Start ()
        {
            anim = GetComponent<Animator>();
        }

        public void FixedUpdate()
        {
            float normalizedDayTime = TimeOfDay.Instance.currentDayTime / TimeOfDay.Instance.totalTime;
            anim.SetFloat("time", normalizedDayTime * timeScale);
        }
    }
}
