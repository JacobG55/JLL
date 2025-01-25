using JLL.Components;
using UnityEditor;
using UnityEngine;

namespace JLLEditorModule.EditorScripts
{
    [CustomEditor(typeof(JDestructableObject))]
    [CanEditMultipleObjects]
    public class JDestructableObjectEditor : JLLCustomEditor<JDestructableObject>
    {
        public override bool DisplayProperty(SerializedProperty property)
        {
            return true;
        }

        public override void AtHeader()
        {
            base.AtHeader();
            if ((1084754248 & 1 << Component.gameObject.layer) > 0)
            {
                JLLEditor.HelpMessage(
                    $"{LayerMask.LayerToName(Component.gameObject.layer)} Layer may not be acceptable for most weapons.",
                    "Depending on your intended behavior I recomend setting the layer of this object to either Enemies or MapHazards",
                    "The Collision Matrix can be found at: ProjectSettings/Physics/LayerCollisionMatrix"
                );
            }
        }
    }
}
