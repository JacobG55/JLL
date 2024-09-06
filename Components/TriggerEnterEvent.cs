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
        public ObjectEvent anythingEntered = new ObjectEvent();

        [Space(15)]
        public InteractEvent playerEntered = new InteractEvent();
        public EnemyEvent enemyEntered = new EnemyEvent();
        public VehicleEvent vehicleEntered = new VehicleEvent();
        public DamageableEvent hittableEntered = new DamageableEvent();
        public ObjectEvent unknownEntered = new ObjectEvent();

        public void Awake()
        {
            onAwake.Invoke();
        }

        public void OnTriggerEnter(Collider other)
        {
            anythingEntered.Invoke(other.gameObject);
            InvokeType(other.gameObject, DamageTrigger.IdentifyCollider(other.gameObject));
        }

        private void InvokeType(GameObject target, int type)
        {
            switch (type)
            {
                case (int)DamageTrigger.ColliderType.Player:
                    playerEntered.Invoke(target.GetComponent<PlayerControllerB>());
                    break;
                case (int)DamageTrigger.ColliderType.Enemy:
                    enemyEntered.Invoke(target.GetComponent<EnemyAICollisionDetect>().mainScript);
                    break;
                case (int)DamageTrigger.ColliderType.Vehicle:
                    vehicleEntered.Invoke(target.GetComponent<VehicleController>());
                    break;
                case (int)DamageTrigger.ColliderType.Object:
                    hittableEntered.Invoke(target.GetComponent<IHittable>());
                    break;
                default:
                    unknownEntered.Invoke(target);
                    return;
            }
        }
    }
}
