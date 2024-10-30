using JLL.Components;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static JLL.Components.JRandomPropPlacer;

namespace JLLEditorModule.EditorScripts
{
    [CustomEditor(typeof(JRandomPropPlacer))]
    [CanEditMultipleObjects]
    public class JRandomPropPlacerEditor : JLLCustomEditor<JRandomPropPlacer>
    {
        private SerializedProperty SpawnablePropsList;
        private ReorderableList spawnablePropProperties;

        public override void OnEnable()
        {
            base.OnEnable();
            SpawnablePropsList = serializedObject.FindProperty("spawnableProps");
            spawnablePropProperties = CreateSpawnablePropProperties(serializedObject, SpawnablePropsList);
        }

        public override bool DisplayProperty(SerializedProperty property)
        {
            if (property.name == "rebakeSurfaces" && Component.rebakeNavMesh != NavMeshToRebake.Custom)
            {
                return false;
            }
            if (property.name == "spawnableProps")
            {
                spawnablePropProperties.DoLayoutList();
                return false;
            }
            return true;
        }

        public ReorderableList CreateSpawnablePropProperties(SerializedObject obj, SerializedProperty CustomList)
        {
            ReorderableList weightedProperties = new ReorderableList(obj, CustomList);
            weightedProperties.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused)
                => SpawnablePropProperty(weightedProperties.serializedProperty.GetArrayElementAtIndex(index), rect);
            weightedProperties.elementHeightCallback = (int index) =>
            {
                return JLLEditor.GetElementRectHeight(Component.spawnableProps[index].spawnRotation == SpawnRotation.BackToWall ? 7 : 6) + 5f;
            };
            weightedProperties.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, new GUIContent("Spawnable Props"));
            };
            return weightedProperties;
        }

        public static void SpawnablePropProperty(SerializedProperty item, Rect rect)
        {
            int i = 0;
            EditorGUI.PropertyField(JLLEditor.GetElementRect(rect, i++), item.FindPropertyRelative("prefabToSpawn"));
            EditorGUI.PropertyField(JLLEditor.GetElementRect(rect, i++), item.FindPropertyRelative("randomAmount"));

            SerializedProperty overrideValue = item.FindPropertyRelative("spawnRotation");
            EditorGUI.PropertyField(JLLEditor.GetElementRect(rect, i++), overrideValue);
            if (overrideValue.enumValueFlag == (int)SpawnRotation.BackToWall)
            {
                EditorGUI.PropertyField(JLLEditor.GetElementRect(rect, i++), item.FindPropertyRelative("spawnFlushAgainstWall"));
            }

            EditorGUI.PropertyField(JLLEditor.GetElementRect(rect, i++), item.FindPropertyRelative("distanceFromEntrances"));
            EditorGUI.PropertyField(JLLEditor.GetElementRect(rect, i++), item.FindPropertyRelative("distanceBetweenSpawns"));
            EditorGUI.PropertyField(JLLEditor.GetElementRect(rect, i++), item.FindPropertyRelative("randomSpawnRange"));
        }
    }
}
