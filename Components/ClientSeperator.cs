using GameNetcodeStuff;
using JLL.API;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class ClientSeperator : MonoBehaviour
    {
        [Tooltip("Event run for the given client.")]
        public InteractEvent eventsForClient = new();
        [Tooltip("Event run for every client except the given one.")]
        public UnityEvent eventsForEveryoneElse = new();

        public void SeperateClientEvents(PlayerControllerB player)
        {
            if (player.IsLocalPlayer())
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
