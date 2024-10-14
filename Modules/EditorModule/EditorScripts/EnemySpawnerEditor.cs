using JLL.Components;
using UnityEditor;

namespace JLLEditorModule.EditorScripts
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(EnemySpawner))]
    public class EnemySpawnerEditor : Editor
    {
        private EnemySpawner EnemySpawner;

        private void OnEnable()
        {
            EnemySpawner = (EnemySpawner)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty iterator = serializedObject.GetIterator();

            for (int i = 0; iterator.NextVisible(i == 0); i++)
            {
                if (EnemySpawner.spawnRandom)
                {
                    if (iterator.name == "type") continue;
                }
                else
                {
                    if (iterator.name == "randomPool") continue;
                }

                EditorGUILayout.PropertyField(iterator);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
