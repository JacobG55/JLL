using UnityEngine;

namespace JLL.ScriptableObjects
{
    public abstract class JLLAddon : ScriptableObject
    {
        public abstract void Init(JLLMod parent);
    }
}
