using System;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.API.Events
{
    [Serializable]
    public class EnemyEvent : UnityEvent<EnemyAI> { }
    
    [Serializable]
    public class VehicleEvent : UnityEvent<VehicleController> { }
    
    [Serializable]
    public class DamageableEvent : UnityEvent<IHittable> { }

    [Serializable]
    public class ObjectEvent : UnityEvent<GameObject> { }

    [Serializable]
    public class FloatEvent : UnityEvent<float> { }
}
