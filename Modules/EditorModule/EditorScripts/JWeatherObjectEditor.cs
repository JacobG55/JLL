using JLL.Components;
using UnityEditor;

namespace JLLEditorModule.EditorScripts
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(JWeatherObject))]
    public class JWeatherObjectEditor : Editor
    {
        private JWeatherObject JWeatherObject;

        private void OnEnable()
        {
            JWeatherObject = (JWeatherObject)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty iterator = serializedObject.GetIterator();

            for (int i = 0; iterator.NextVisible(i == 0); i++)
            {
                if (iterator.name == "defaultToSelf" && JWeatherObject.activeObject != null)
                {
                    continue;
                }

                EditorGUILayout.PropertyField(iterator);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
