using JLL.Components;
using UnityEditor;

namespace JLLEditorModule.EditorScripts
{
    [CustomEditor(typeof(EnemySpawner))]
    [CanEditMultipleObjects]
    public class EnemySpawnerEditor : JLLCustomEditor<EnemySpawner>
    {
        public override bool DisplayProperty(SerializedProperty property)
        {
            if (Component.spawnRandom)
            {
                if (property.name == "type") return false;
            }
            else
            {
                if (property.name == "randomPool") return false;
            }
            return true;
        }
    }
}
