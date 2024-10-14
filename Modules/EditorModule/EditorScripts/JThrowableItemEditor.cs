using JLLItemsModule.Components;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static JLL.Components.ItemSpawner;

namespace JLLEditorModule.EditorScripts
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(JThrowableItem))]
    public class JThrowableItemEditor : Editor
    {
        private JThrowableItem JThrowableItem;

        private SerializedProperty CustomList;
        private ReorderableList weightedProperties;

        private void OnEnable()
        {
            JThrowableItem = (JThrowableItem)target;

            CustomList = serializedObject.FindProperty("CustomList");

            weightedProperties = new ReorderableList(serializedObject, CustomList);
            weightedProperties.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused)
                => JLLEditor.WeightedItemProperty(weightedProperties.serializedProperty.GetArrayElementAtIndex(index), rect);
            weightedProperties.elementHeightCallback = (int index) =>
            {
                return JLLEditor.GetElementRectHeight(5) + 5f;
            };
            weightedProperties.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, new GUIContent("Custom Item List"));
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty iterator = serializedObject.GetIterator();

            for (int i = 0; iterator.NextVisible(i == 0); i++)
            {
                if (iterator.name == "CustomList")
                {
                    if (JThrowableItem.SourcePool == SpawnPoolSource.CustomList)
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
