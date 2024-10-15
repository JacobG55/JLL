using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace JLLEditorModule
{
    public class JLLEditor
    {
        public static void HelpMessage(params string[] messages)
        {
            if (messages.Length > 1)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < messages.Length; i++)
                {
                    stringBuilder.Append(messages[i]);
                    if (i < messages.Length - 1) stringBuilder.Append("\n\n");
                }
                EditorGUILayout.HelpBox(new GUIContent(stringBuilder.ToString()));
            }
            else if (messages.Length == 1)
            {
                EditorGUILayout.HelpBox(new GUIContent(messages[0]));
            }
        }

        public static void WeightedItemProperty(SerializedProperty item, Rect rect)
        {
            int i = 0;
            EditorGUI.PropertyField(GetElementRect(rect, i++), item.FindPropertyRelative("Item"));
            EditorGUI.PropertyField(GetElementRect(rect, i++), item.FindPropertyRelative("Weight"));
            EditorGUI.PropertyField(GetElementRect(rect, i++), item.FindPropertyRelative("FindRegisteredItem"));

            SerializedProperty overrideValue = item.FindPropertyRelative("OverrideValue");
            EditorGUI.PropertyField(GetElementRect(rect, 2, new Vector2Int(0, i)), overrideValue);
            if (overrideValue.boolValue)
            {
                EditorGUI.PropertyField(GetElementRect(rect, 2, new Vector2Int(1, i)), item.FindPropertyRelative("ScrapValue"), GUIContent.none);
            }
            i++;

            EditorGUI.PropertyField(GetElementRect(rect, i++), item.FindPropertyRelative("SpawnOffset"));
        }

        public static Rect GetElementRect(Rect rect, int index, float seperation = 5f)
        {
            return new Rect(rect.x, rect.y + GetElementRectHeight(index, seperation) + seperation, rect.width, EditorGUIUtility.singleLineHeight);
        }

        public static Rect GetElementRect(Rect rect, int columns, Vector2Int location, float seperation = 5f)
        {
            return new Rect(rect.x + (rect.width / columns * location.x), rect.y + GetElementRectHeight(location.y, seperation), rect.width / columns, EditorGUIUtility.singleLineHeight);
        }

        public static float GetElementRectHeight(int index, float seperation = 5f)
        {
            return (EditorGUIUtility.singleLineHeight + seperation) * index;
        }

        public static ReorderableList CreateWeightedItemSpawnProperties(SerializedObject obj, SerializedProperty CustomList)
        {
            ReorderableList weightedProperties = new ReorderableList(obj, CustomList);
            weightedProperties.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused)
                => WeightedItemProperty(weightedProperties.serializedProperty.GetArrayElementAtIndex(index), rect);
            weightedProperties.elementHeightCallback = (int index) =>
            {
                return GetElementRectHeight(5) + 5f;
            };
            weightedProperties.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, new GUIContent("Custom Item List"));
            };
            return weightedProperties;
        }
    }
}
