using UnityEngine;

namespace JLL.Components.Modifier
{
    public abstract class JModifier<T> : MonoBehaviour
    {
        public abstract void Modify(T target);
    }
}
