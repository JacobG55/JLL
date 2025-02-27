using UnityEngine;
using UnityEngine.Serialization;

namespace JLL.Components
{
    [RequireComponent (typeof(Animator))]
    public class DayCycleAnimator : MonoBehaviour
    {
        private Animator anim;

        [Tooltip("Scale of time the animation synced to the day progression will use. A value of 1 plays the animation from beginning to end once. Numbers larger than 1 requires the animation to have looping enabled otherwise it will stop on the final frame.")]
        [FormerlySerializedAs("timeScale")]
        public float fixedAnimEndTime = 1f;

        public void Start ()
        {
            anim = GetComponent<Animator>();
        }

        public void FixedUpdate()
        {
            float currentTime = Mathf.Max(0, TimeOfDay.Instance.currentDayTime - 100);
            float normalizedDayTime = currentTime / TimeOfDay.Instance.totalTime;
            //JLogHelper.LogInfo($"{currentTime} / {TimeOfDay.Instance.totalTime} = {normalizedDayTime} | {normalizedDayTime * fixedAnimEndTime}", JLogLevel.Wesley);
            anim.SetFloat("time", normalizedDayTime * fixedAnimEndTime);
        }

        public void SetFixedAnimTime(float time) => fixedAnimEndTime = time;
    }
}
