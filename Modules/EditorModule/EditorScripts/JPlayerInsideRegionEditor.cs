using JLL.Components;
using UnityEditor;

namespace JLLEditorModule.EditorScripts
{
    [CustomEditor(typeof(JPlayerInsideRegion))]
    [CanEditMultipleObjects]
    public class JPlayerInsideRegionEditor : JLLCustomEditor<JPlayerInsideRegion>
    {
        public override bool DisplayProperty(SerializedProperty property)
        {
            if (!Component.limitEventTriggers)
            {
                if (property.name == "maxEventTriggers") return false;
            }
            return true;
        }
    }
}
