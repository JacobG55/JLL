using JLL.Components;
using UnityEditor;
using UnityEditorInternal;

namespace JLLEditorModule.EditorScripts
{
    [CustomEditor(typeof(EnemySpawner))]
    [CanEditMultipleObjects]
    public class EnemySpawnerEditor : JLLCustomEditor<EnemySpawner>
    {
        private SerializedProperty randomPool;
        private ReorderableList weightedProperties;

        public override void OnEnable()
        {
            base.OnEnable();
            randomPool = serializedObject.FindProperty("randomPool");
            weightedProperties = JLLEditor.CreateWeightedEnemySpawnProperties(serializedObject, randomPool);
        }

        public override void AtHeader()
        {
            base.AtHeader();
            if (!string.IsNullOrEmpty(Component.enemyName))
            {
                Component.type = null;
            }
            foreach(var enemyWeight in Component.randomPool)
            {
                if (!string.IsNullOrEmpty(enemyWeight.enemyName))
                {
                    enemyWeight.enemyType = null;
                }
            }
        }

        public override bool DisplayProperty(SerializedProperty property)
        {
            if (Component.spawnRandom)
            {
                if (property.name == "randomPool")
                {
                    weightedProperties.DoLayoutList();
                    return false;
                }
                if (property.name == "type" ||  property.name == "enemyName")
                {
                    return false;
                }
            }
            else
            {
                if (property.name == "randomPool" || (property.name == "type" && !string.IsNullOrEmpty(Component.enemyName)))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
