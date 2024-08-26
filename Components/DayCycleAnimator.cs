using UnityEngine;

namespace JLL.Components
{
    [RequireComponent (typeof(Animator))]
    public class DayCycleAnimator : MonoBehaviour
    {
        private Animator anim;

        public void Start ()
        {
            anim = GetComponent<Animator>();
        }

        public void FixedUpdate()
        {
            float normalizedDayTime = TimeOfDay.Instance.currentDayTime / TimeOfDay.Instance.totalTime;
            anim.SetFloat("time", normalizedDayTime);
        }
    }
}
