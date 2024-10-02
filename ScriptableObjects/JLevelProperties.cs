using JLL.API.LevelProperties;
using UnityEngine;

namespace JLL.ScriptableObjects
{
    [CreateAssetMenu(menuName = "JLL/JLevelProperties")]
    public class JLevelProperties : ScriptableObject
    {
        [Header("-=-EXPERIMENTAL-=-")]
        public JLevelPropertyEntry Properties = new JLevelPropertyEntry();
    }
}
