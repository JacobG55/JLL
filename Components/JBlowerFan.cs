using GameNetcodeStuff;
using UnityEngine;

namespace JLL.Components
{
    public class JBlowerFan : MonoBehaviour
    {
        public float forceMultiplier = 1f;
        public float vehicleforceMultiplier = 6f;
        public Transform sourcePos;

        public void OnTriggerStay(Collider other)
        {
            PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;
            if (other.gameObject == player.gameObject && !player.inVehicleAnimation && !player.inSpecialInteractAnimation)
            {
                player.externalForceAutoFade += CalcPushForce(player.transform.position);
                player.fallValue = -1f;
                player.fallValueUncapped = -1f;
            }
            else if (other.gameObject.TryGetComponent(out VehicleController vehicle) && vehicle.IsOwner)
            {
                vehicle.mainRigidbody.AddForce(CalcPushForce(vehicle.transform.position) * vehicleforceMultiplier, ForceMode.Impulse);
            }
        }

        public Vector3 CalcPushForce(Vector3 pos)
        {
            return (pos - sourcePos.position).normalized * forceMultiplier * (1f / Vector3.Distance(pos, sourcePos.position));
        }
    }
}
