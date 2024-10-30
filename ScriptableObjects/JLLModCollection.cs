using UnityEngine;

namespace JLL.ScriptableObjects
{
    [CreateAssetMenu(menuName = "JLL/Collections/JLLModCollection", order = 20)]
    public class JLLModCollection : ScriptableObject
    {
        public JLLMod[] Mods = new JLLMod[0];
    }
}
