using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class ClientSeperator : MonoBehaviour
    {
        public InteractEvent eventsForClient = new InteractEvent();
        public UnityEvent eventsForEveryoneElse = new UnityEvent();

        public void SeperateClientEvents(PlayerControllerB player)
        {
            if (player.actualClientId == HUDManager.Instance.localPlayer.actualClientId)
            {
                eventsForClient.Invoke(player);
            }
            else
            {
                eventsForEveryoneElse.Invoke();
            }
        }
    }
}
