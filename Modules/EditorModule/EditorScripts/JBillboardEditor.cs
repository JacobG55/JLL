using JLL.Components;
using UnityEditor;

namespace JLLEditorModule.EditorScripts
{
    [CustomEditor(typeof(JBillboard))]
    [CanEditMultipleObjects]
    public class JBillboardEditor : JLLCustomEditor<JBillboard>
    {
        public override bool DisplayProperty(SerializedProperty property)
        {
            if (property.name == "LerpSpeed") return Component.LerpRot;
            return true;
        }
    }
}
