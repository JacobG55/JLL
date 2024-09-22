using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class ClientSeperator : MonoBehaviour
    {
        [Tooltip("Event run for the given client.")]
        public InteractEvent eventsForClient = new InteractEvent();
        [Tooltip("Event run for every client except the given one.")]
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
