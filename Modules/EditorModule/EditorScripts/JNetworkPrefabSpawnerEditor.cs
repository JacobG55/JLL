using JLL.Components;
using UnityEditor;

namespace JLLEditorModule.EditorScripts
{
    [CustomEditor(typeof(JNetworkPrefabSpawner))]
    [CanEditMultipleObjects]
    public class JNetworkPrefabSpawnerEditor : JLLCustomEditor<JNetworkPrefabSpawner>
    {
        public override bool DisplayProperty(SerializedProperty property)
        {
            if (Component.spawnRandom)
            {
                if (property.name == "spawnPrefabName") return false;
            }
            return true;
        }
    }
}
