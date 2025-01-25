using UnityEngine;

namespace WesleyMoonScripts.Components
{
    public class ExitDoorEffects : MonoBehaviour
    {
        public EntranceTeleport entranceDoor;

        public ParticleSystem exitParticles;
        public AudioClip exitSound;
        public AudioSource audioSource;

        public void PlayExitFX()
        {
            exitParticles.Play();
            audioSource.PlayOneShot(exitSound);
        }
    }
}
