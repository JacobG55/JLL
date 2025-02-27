using GameNetcodeStuff;
using JLL.API.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace JLL.Components
{
    public class TriggerEnterEvent : MonoBehaviour
    {
        [FormerlySerializedAs("onAwake")]
        public UnityEvent onEnabled = new UnityEvent();

        [Header("OnTriggerEnter")]
        [Tooltip("Event run when a GameObject enters the trigger")]
        public ObjectEvent anythingEntered = new ObjectEvent();

        [Space(15)]
        [Tooltip("Event run when the entered collider is a Player")]
        public InteractEvent playerEntered = new InteractEvent();
        [Tooltip("Event run when the entered collider is an Enemy")]
        public EnemyEvent enemyEntered = new EnemyEvent();
        [Tooltip("Event run when the entered collider is a Vehicle")]
        public VehicleEvent vehicleEntered = new VehicleEvent();
        [Tooltip("Event run when the entered collider is damagable by weapons and not any of the above types")]
        public DamageableEvent hittableEntered = new DamageableEvent();
        [Tooltip("Event run when the entered collider is not identified as any of the above things")]
        public ObjectEvent unknownEntered = new ObjectEvent();

        public void OnEnable()
        {
            onEnabled.Invoke();
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
                case (int)ColliderType.Player:
                    playerEntered.Invoke(target.GetComponent<PlayerControllerB>());
                    break;
                case (int)ColliderType.Enemy:
                    enemyEntered.Invoke(target.GetComponent<EnemyAICollisionDetect>().mainScript);
                    break;
                case (int)ColliderType.Vehicle:
                    vehicleEntered.Invoke(target.GetComponent<VehicleController>());
                    break;
                case (int)ColliderType.Object:
                    hittableEntered.Invoke(target.GetComponent<IHittable>());
                    break;
                default:
                    unknownEntered.Invoke(target);
                    return;
            }
        }
    }
}
