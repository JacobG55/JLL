using JLL.Components;
using UnityEditor;

namespace JLLEditorModule.EditorScripts
{
    [CustomEditor(typeof(JClientAttatchedObject))]
    [CanEditMultipleObjects]
    public class JClientAttatchedObjectEditor : JLLCustomEditor<JClientAttatchedObject>
    {
        public override bool DisplayProperty(SerializedProperty property)
        {
            if (property.name == "lerpPosition" && !Component.attachToLocalPlayer)
            {
                return false;
            }
            return true;
        }

        public override void DisplayWarnings(SerializedProperty property)
        {
            if (property.name == "target" && Component.enableCondition != JClientAttatchedObject.ActiveCondition.None && (Component.gameObject == Component.target || JLLEditor.ObjectIsParent(Component.target, Component.transform)))
            {
                JLLEditor.HelpMessage("Target should not be self or parent when an Enable Condition is set.", "Disabling the object containing this script will stop it from working.");
            }
        }
    }
}
