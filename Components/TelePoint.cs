using GameNetcodeStuff;
using UnityEngine;

namespace JLL.Components
{
    public class TelePoint : MonoBehaviour
    {
        public bool rotatePlayer = false;
        public bool rotateObjects = true;

        public void TeleportPlayer(PlayerControllerB player)
        {
            player.TeleportPlayer(transform.position, rotatePlayer, transform.rotation.y);
        }
        public void Teleport(GameObject obj)
        {
            obj.transform.position = transform.position;
            if (rotateObjects)
            {
                obj.transform.rotation = transform.rotation;
            }
        }
    }
}
