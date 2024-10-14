using JLL.Components;
using UnityEditor;
using UnityEngine;

namespace JLLEditorModule.EditorScripts
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(JClientAttatchedObject))]
    public class JClientAttatchedObjectEditor : Editor
    {
        private JClientAttatchedObject JClientAttatchedObject;

        private void OnEnable()
        {
            JClientAttatchedObject = (JClientAttatchedObject)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty iterator = serializedObject.GetIterator();

            for (int i = 0; iterator.NextVisible(i == 0); i++)
            {
                if (iterator.name == "lerpPosition" && !JClientAttatchedObject.attachToLocalPlayer)
                {
                    continue;
                }

                EditorGUILayout.PropertyField(iterator);

                if (iterator.name == "target" && JClientAttatchedObject.enableCondition != JClientAttatchedObject.ActiveCondition.None && (JClientAttatchedObject.gameObject == JClientAttatchedObject.target || ObjectIsParent(JClientAttatchedObject.target, JClientAttatchedObject.transform)))
                {
                    JLLEditor.HelpMessage("Target should not be self or parent when an Enable Condition is set.", "Disabling the object containing this script will stop it from working.");
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private bool ObjectIsParent(GameObject target, Transform search)
        {
            if (search.transform.parent != null)
            {
                if (search.transform.parent.gameObject == target)
                {
                    return true;
                }
                else return ObjectIsParent(target, search.transform.parent);
            }
            return false;
        }
    }
}
