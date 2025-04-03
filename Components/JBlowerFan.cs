using GameNetcodeStuff;
using UnityEngine;

namespace JLL.Components
{
    public class JBlowerFan : MonoBehaviour
    {
        public float forceMultiplier = 1f;
        public Transform sourcePos;

        private void OnTriggerStay(Collider other)
        {
            PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;
            if (other.gameObject == player.gameObject)
            {
                player.externalForceAutoFade += (player.transform.position - sourcePos.position).normalized * forceMultiplier * (1 / Vector3.Distance(player.transform.position, sourcePos.position));
                player.fallValue = -1f;
                player.fallValueUncapped = -1f;
            }
        }
    }
}
