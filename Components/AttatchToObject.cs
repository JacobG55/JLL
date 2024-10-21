using UnityEngine;

namespace JLL.Components
{
    public class AttatchToObject : MonoBehaviour
    {
        public Transform parent;
        public bool attatchPos = true;
        public bool attatchRot = true;

        public void LateUpdate()
        {
            if (parent == null || !parent) return;
            if (attatchPos)
            {
                transform.position = parent.position;
            }
            if (attatchRot)
            {
                transform.rotation = parent.rotation;
            }
        }

    }
}
