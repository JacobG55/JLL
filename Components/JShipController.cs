using UnityEngine;

namespace JLL.Components
{
    public class JShipController : MonoBehaviour
    {
        public void MakeShipLeave()
        {
            StartOfRound.Instance.ShipLeaveAutomatically(true);
        }

        public void PowerSurgeShip()
        {
            StartOfRound.Instance.PowerSurgeShip();
        }

        public void ToggleMonitors(bool toggle)
        {
            StartOfRound.Instance.mapScreen.SwitchScreenOn(on: toggle);
        }

        public void ToggleShipLights(bool toggle)
        {
            StartOfRound.Instance.shipRoomLights.SetShipLightsServerRpc(setLightsOn: toggle);
        }
    }
}
