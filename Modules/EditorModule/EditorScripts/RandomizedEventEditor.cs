using JLL.Components;
using UnityEditor;

namespace JLLEditorModule.EditorScripts
{
    [CustomEditor(typeof(RandomizedEvent))]
    [CanEditMultipleObjects]
    public class RandomizedEventEditor : JLLCustomEditor<RandomizedEvent>
    {
        public override bool DisplayProperty(SerializedProperty property)
        {
            return true;
        }
    }
}
