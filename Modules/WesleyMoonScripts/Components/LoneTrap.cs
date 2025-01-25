using GameNetcodeStuff;
using UnityEngine;

namespace WesleyMoonScripts.Components
{
    public class LoneTrap : MonoBehaviour
    {
        public float range = 12.0f;
        public bool rotate = true;
        public float rotSpeed = 1.0f;

        [Header("Target is Close")]
        public float closeRange = 4f;
        public float closeEventCooldown = 2f;
        private float closeTimer = 0;
        public InteractEvent CloseEvent = new InteractEvent();

        private PlayerControllerB? target;

        public void FixedUpdate()
        {
            if (target == null)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, range);
                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject.layer == 3 && collider.TryGetComponent(out PlayerControllerB player) && player.isPlayerAlone && !player.isPlayerDead)
                    {
                        target = player;
                        break;
                    }
                }
            }
            else
            {
                if (!target.isPlayerAlone || target.isPlayerDead || Vector3.Distance(target.transform.position, transform.position) > range)
                {
                    target = null;
                    closeTimer = 0;
                    return;
                }
            }

            if (target != null)
            {
                if (rotate) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, Quaternion.LookRotation((target.transform.position - transform.position).normalized, Vector3.up).eulerAngles.y, 0), Time.deltaTime * rotSpeed);

                if (closeTimer <= 0)
                {
                    closeTimer = closeRange;
                    if (Vector3.Distance(target.transform.position, transform.position) < closeRange)
                    {
                        CloseEvent.Invoke(target);
                    }
                }
                else
                {
                    closeTimer -= Time.deltaTime;
                }
            }
        }

        public void OnDrawGizmos()
        {
            if (range > 0)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(transform.position, range);
            }
            if (closeRange > 0)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, closeRange);
            }
        }
    }
}
