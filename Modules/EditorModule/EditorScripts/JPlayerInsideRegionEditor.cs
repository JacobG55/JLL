using JLL.Components;
using UnityEditor;

namespace JLLEditorModule.EditorScripts
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(JPlayerInsideRegion))]
    public class JPlayerInsideRegionEditor : Editor
    {
        private JPlayerInsideRegion JPlayerInsideRegion;

        private void OnEnable()
        {
            JPlayerInsideRegion = (JPlayerInsideRegion)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty iterator = serializedObject.GetIterator();

            for (int i = 0; iterator.NextVisible(i == 0); i++)
            {
                if (!JPlayerInsideRegion.limitEventTriggers)
                {
                    if (iterator.name == "maxEventTriggers") 
                        continue;
                }

                EditorGUILayout.PropertyField(iterator);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
