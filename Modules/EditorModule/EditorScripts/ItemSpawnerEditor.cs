using JLL.Components;
using UnityEditor;
using UnityEditorInternal;
using static JLL.Components.ItemSpawner;

namespace JLLEditorModule.EditorScripts
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ItemSpawner))]
    public class ItemSpawnerEditor : Editor
    {
        private ItemSpawner ItemSpawner;

        private SerializedProperty CustomList;
        private ReorderableList weightedProperties;

        private void OnEnable()
        {
            ItemSpawner = (ItemSpawner)target;

            CustomList = serializedObject.FindProperty("CustomList");
            weightedProperties = JLLEditor.CreateWeightedItemSpawnProperties(serializedObject, CustomList);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty iterator = serializedObject.GetIterator();

            for (int i = 0; iterator.NextVisible(i == 0); i++)
            {
                if (iterator.name == "CustomList")
                {
                    if (ItemSpawner.SourcePool == SpawnPoolSource.CustomList)
                    {
                        weightedProperties.DoLayoutList();
                    }
                    continue;
                }

                EditorGUILayout.PropertyField(iterator);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
