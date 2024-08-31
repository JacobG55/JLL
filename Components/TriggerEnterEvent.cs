using GameNetcodeStuff;
using JLL.API.Events;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class TriggerEnterEvent : MonoBehaviour
    {
        public UnityEvent onAwake = new UnityEvent();

        [Header("OnTriggerEnter")]
        public InteractEvent playerEntered = new InteractEvent();
        public EnemyEvent enemyEntered = new EnemyEvent();
        public VehicleEvent vehicleEntered = new VehicleEvent();
        public DamageableEvent hittableEntered = new DamageableEvent();
        public ObjectEvent unknownEntered = new ObjectEvent();

        public ObjectEvent anythingEntered = new ObjectEvent();

        public void Awake()
        {
            onAwake.Invoke();
        }

        public void OnTriggerEnter(Collider other)
        {
            anythingEntered.Invoke(other.gameObject);

            if (other.CompareTag("Player"))
            {
                if (other.TryGetComponent(out PlayerControllerB player))
                {
                    playerEntered.Invoke(player);
                }
            }
            else if (other.CompareTag("Enemy"))
            {
                if (other.TryGetComponent(out EnemyAICollisionDetect enemy))
                {
                    enemyEntered.Invoke(enemy.mainScript);
                }
            }
            else if (other.TryGetComponent(out VehicleController vehicle))
            {
                vehicleEntered.Invoke(vehicle);
            }
            else if (other.TryGetComponent(out IHittable hittable))
            {
                hittableEntered.Invoke(hittable);
            }
            else
            {
                unknownEntered.Invoke(other.gameObject);
            }
        }
    }
}
