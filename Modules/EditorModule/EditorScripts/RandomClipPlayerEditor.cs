using JLL.Components;
using UnityEditor;

namespace JLLEditorModule.EditorScripts
{
    [CustomEditor(typeof(RandomClipPlayer))]
    [CanEditMultipleObjects]
    public class RandomClipPlayerEditor : JLLCustomEditor<RandomClipPlayer>
    {
        public override bool DisplayProperty(SerializedProperty property)
        {
            if (!Component.walkieCanHear)
            {
                if (property.name == "walkieVolumeMultiplier")
                {
                    return false;
                }
            }

            if (!Component.creaturesCanHear)
            {
                if (property.name == "creatureVolumeMultiplier" || property.name == "creatureRangeMultiplier")
                {
                    return false;
                }
            }

            return true;
        }
    }
}
