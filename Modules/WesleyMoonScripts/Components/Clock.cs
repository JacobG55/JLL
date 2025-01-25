using System;
using UnityEngine;

namespace WesleyMoonScripts.Components
{
    public class Clock : MonoBehaviour
    {
        public Transform hourHand;
        public Transform minuteHand;

        public TimeScale timeScale = TimeScale.GameTime;
        public bool smoothTicks = false;
        public Vector2 randomScaleRange = new Vector2(0, 1.5f);

        public enum TimeScale
        {
            GameTime,
            RealTime,
            Random,
            Disabled
        }

        [Header("Complying with wesly's demands")]
        public AudioClip[] tick = new AudioClip[0];
        private int clip = 0;
        public AudioSource audioSource;

        private float lastMinute = 0;
        private float randomCurrent;
        private float randomScale;

        public void Start()
        {
            randomCurrent = UnityEngine.Random.Range(0f, 1440f);
            randomScale = UnityEngine.Random.Range(randomScaleRange.x, randomScaleRange.y);
        }

        public void Update()
        {
            switch (timeScale)
            {
                case TimeScale.GameTime:
                    int minutes = (int)(TimeOfDay.Instance.normalizedTimeOfDay * (60f * TimeOfDay.Instance.numberOfHours)) + 360;
                    UpdateHandRots(minutes / 60, minutes);
                    break;
                case TimeScale.RealTime:
                    DateTime time = DateTime.Now;
                    UpdateHandRots(time.Hour + (time.Minute / 60f), time.Minute + (time.Second / 60f));
                    break;
                case TimeScale.Random:
                    randomCurrent += Time.deltaTime * randomScale;
                    UpdateHandRots(randomCurrent / 60f, randomCurrent);
                    break;
                default: break;
            }
        }

        public void UpdateHandRots(float hours, float minutes)
        {
            float floored = Mathf.Floor(minutes);
            if (lastMinute != floored)
            {
                lastMinute = floored;
                if (audioSource != null && tick.Length > 0)
                {
                    audioSource.PlayOneShot(tick[clip]);
                    clip++;
                    if (clip >= tick.Length) clip = 0;
                }
            }
            if (!smoothTicks) minutes = floored;
            hourHand.localEulerAngles = new Vector3(0, 360f * (hours / 12f), 0);
            minuteHand.localEulerAngles = new Vector3(0, 360f * (minutes / 60f), 0);
        }

        public void SetRandomTimeScale(float timeScale)
        {
            randomScale = timeScale;
        }
    }
}
