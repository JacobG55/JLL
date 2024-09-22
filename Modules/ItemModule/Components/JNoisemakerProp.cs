using UnityEngine;

namespace JLLItemsModule.Components
{
    public class JNoisemakerProp : JGrabbableObject
    {
        [Header("Noisemaker")]
        public bool isToggle = false;
        public bool waitForCompletion = false;

        private float activeClipLength = 0;
        private float lastUsedTime = 0;

        [Space(5f)]
        public AudioClip[] noiseSFX = new AudioClip[0];
        public AudioClip[] disableSFX = new AudioClip[0];
        public AudioSource noiseAudio;
        public AudioClip[] noiseSFXFar = new AudioClip[0];
        public AudioSource noiseAudioFar;

        [Space(3f)]
        [Tooltip("Airhorn: 65\nCash Register: 25\nClown Horn: 60\nHair Dryer: 30")]
        public float noiseRange = 60f;

        [Tooltip("Airhorn: 1\nCash Register: 1\nClown Horn: 1\nHair Dryer: 0.9")]
        public float maxLoudness = 1f;
        [Tooltip("Airhorn: 0.95\nCash Register: 0.9\nClown Horn: 0.6\nHair Dryer: 0.8")]
        public float minLoudness = 0.6f;

        [Tooltip("Airhorn: 1\nCash Register: 1\nClown Horn: 1\nHair Dryer: 1")]
        public float maxPitch = 1f;
        [Tooltip("Airhorn: 0.8\nCash Register: 0.95\nClown Horn: 0.93\nHair Dryer: 0.96")]
        public float minPitch = 0.93f;

        private System.Random noisemakerRandom;

        public Animator? triggerAnimator;

        public override void Start()
        {
            base.Start();
            noisemakerRandom = new System.Random(StartOfRound.Instance.randomMapSeed + 85);
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);

            if (Time.realtimeSinceStartup - lastUsedTime < activeClipLength)
            {
                if (isToggle)
                {
                    if (noiseAudio != null)
                    {
                        noiseAudio.Stop();
                        if (disableSFX.Length > 0)
                        {
                            int soundIndex = noisemakerRandom.Next(0, disableSFX.Length);
                            float volume = noisemakerRandom.Next((int)(minLoudness * 100f), (int)(maxLoudness * 100f)) / 100f;
                            float pitch = noisemakerRandom.Next((int)(minPitch * 100f), (int)(maxPitch * 100f)) / 100f;

                            noiseAudio.pitch = pitch;
                            noiseAudio.PlayOneShot(disableSFX[soundIndex], volume);
                        }
                    }
                    noiseAudioFar?.Stop();
                    activeClipLength = 0;
                    return;
                }
                if (waitForCompletion)
                {
                    return;
                }
            }

            if (GameNetworkManager.Instance.localPlayerController != null)
            {
                int soundIndex = noisemakerRandom.Next(0, noiseSFX.Length);
                float volume = noisemakerRandom.Next((int)(minLoudness * 100f), (int)(maxLoudness * 100f)) / 100f;
                float pitch = noisemakerRandom.Next((int)(minPitch * 100f), (int)(maxPitch * 100f)) / 100f;

                activeClipLength = noiseSFX[soundIndex].length;
                lastUsedTime = Time.realtimeSinceStartup;

                if (noiseAudio != null)
                {
                    noiseAudio.pitch = pitch;
                    noiseAudio.PlayOneShot(noiseSFX[soundIndex], volume);
                }

                if (noiseAudioFar != null && soundIndex < noiseSFXFar.Length)
                {
                    noiseAudioFar.pitch = pitch;
                    noiseAudioFar.PlayOneShot(noiseSFXFar[soundIndex], volume);
                }

                triggerAnimator?.SetTrigger("playAnim");

                WalkieTalkie.TransmitOneShotAudio(noiseAudio, noiseSFX[soundIndex], volume);
                RoundManager.Instance.PlayAudibleNoise(transform.position, noiseRange, volume, 0, isInElevator && StartOfRound.Instance.hangarDoorsClosed);

                if (minLoudness >= 0.6f && playerHeldBy != null)
                {
                    playerHeldBy.timeSinceMakingLoudNoise = 0f;
                }
            }
        }
    }
}
