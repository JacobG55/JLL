using UnityEngine;

namespace JLL.Components
{
    public class JFloodPath : MonoBehaviour
    {
        [Header("Object getting modified")]
        public Transform Target;

        [Header("Start/End transforms")]
        public Transform StartOfDay;
        public Transform EndOfDay;

        public void FixedUpdate()
        {
            float dayProgress = TimeOfDay.Instance.currentDayTime / TimeOfDay.Instance.totalTime;

            Target.position = Vector3.Lerp(StartOfDay.position, EndOfDay.position, dayProgress);
            Target.rotation = Quaternion.Lerp(StartOfDay.rotation, EndOfDay.rotation, dayProgress);
            Target.localScale = Vector3.Lerp(StartOfDay.localScale, EndOfDay.localScale, dayProgress);
        }
    }
}
