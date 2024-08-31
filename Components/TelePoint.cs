using GameNetcodeStuff;
using UnityEngine;

namespace JLL.Components
{
    public class TelePoint : MonoBehaviour
    {
        public bool rotatePlayer = false;

        public void TeleportPlayer(PlayerControllerB player)
        {
            player.TeleportPlayer(transform.position, rotatePlayer, transform.rotation.y);
        }
        public void Teleport(GameObject obj)
        {
            obj.transform.position = transform.position;
        }
    }
}
