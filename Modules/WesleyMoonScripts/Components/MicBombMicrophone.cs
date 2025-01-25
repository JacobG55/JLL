using UnityEngine;

namespace WesleyMoonScripts.Components
{
    public class MicBombMicrophone : MonoBehaviour, INoiseListener
    {
        public MicBombItem micBombItem;

        public void DetectNoise(Vector3 noisePosition, float noiseLoudness, int timesPlayedInOneSpot, int noiseID)
        {
            if (Physics.Linecast(transform.position, noisePosition, 256))
            {
                noiseLoudness /= 2f;
            }
            if (noiseLoudness < micBombItem.minLoudness)
            {
                return;
            }
            micBombItem.IncreaseAgression();
        }
    }
}
