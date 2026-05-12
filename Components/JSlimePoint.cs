using GameNetcodeStuff;
using UnityEngine;

namespace JLL.Components
{
    public class JSlimePoint : MonoBehaviour
    {
        public float slimeDistance = 2.55f;
        public float slimePotency = 8f;
        public bool useColliderBounds = false;
        [Tooltip("This has no effect in game. it is just a tool for visualizing the effective range of this script with collider bounds.")]
        public Transform debugTransform;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            if (useColliderBounds && debugTransform)
            {
                Vector3 point = GetClosestPoint(debugTransform.position).point;
                Gizmos.DrawWireSphere(point, slimeDistance);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(point, debugTransform.position);
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, slimeDistance);
            }
        }

        public void Update()
        {
            PlayerControllerB local = StartOfRound.Instance.localPlayerController;
            if (local.isPlayerDead || slimeDistance <= 0) return;

            float distance = useColliderBounds ? GetClosestPoint(local.transform.position).distance : Vector3.Distance(transform.position, local.transform.position);

            if (distance < slimeDistance)
            {
                local.slipperyFloor = Mathf.Max(local.slipperyFloor, Mathf.Lerp(slimePotency, 0f, distance / slimeDistance));
            }
        }

        private (Vector3 point, float distance) GetClosestPoint(Vector3 pos)
        {
            float distance = float.MaxValue;
            Vector3 closest = Vector3.zero;
            foreach (Collider collider in GetComponents<Collider>())
            {
                Vector3 point = collider.ClosestPoint(pos);
                float dist = Vector3.Distance(point, pos);
                if (dist < distance)
                {
                    distance = dist;
                    closest = point;
                }
            }
            return (closest, distance);
        }
    }
}
