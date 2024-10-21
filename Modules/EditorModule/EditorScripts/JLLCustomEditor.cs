using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace JLLEditorModule.EditorScripts
{
    public abstract class JLLCustomEditor<T> : Editor where T : UnityEngine.Object
    {
        public T Component;

        public virtual void OnEnable()
        {
            Component = (T)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            AtHeader();

            SerializedProperty iterator = serializedObject.GetIterator();

            bool first = true;
            for (int i = 0; iterator.NextVisible(first); i++)
            {
                if (first)
                {
                    first = false;
                    continue;
                }
                if (DisplayProperty(iterator))
                {
                    EditorGUILayout.PropertyField(iterator);
                    DisplayWarnings(iterator);
                }
            }

            AtFooter();

            serializedObject.ApplyModifiedProperties();
        }

        public virtual void AtHeader()
        {
            if (Component is NetworkBehaviour behavior && behavior.GetComponent<NetworkObject>() == null)
            {
                JLLEditor.HelpMessage(
                    "Network Behaviours require a Network Object to run some of their code. Ensure you have a Network Object attatched to this script in some way."
                );
            }
        }

        public abstract bool DisplayProperty(SerializedProperty property);

        public virtual void DisplayWarnings(SerializedProperty property)
        {

        }

        public virtual void AtFooter()
        {

        }
    }
}
