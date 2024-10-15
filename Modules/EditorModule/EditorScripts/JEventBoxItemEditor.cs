using JLLItemsModule.Components;
using UnityEditor;
using UnityEditorInternal;
using static JLL.Components.ItemSpawner;

namespace JLLEditorModule.EditorScripts
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(JEventBoxItem))]
    public class JEventBoxItemEditor : Editor
    {
        private JEventBoxItem JEventBoxItem;

        private SerializedProperty CustomList;
        private ReorderableList weightedProperties;

        private void OnEnable()
        {
            JEventBoxItem = (JEventBoxItem)target;

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
                    if (JEventBoxItem.SourcePool == SpawnPoolSource.CustomList)
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
