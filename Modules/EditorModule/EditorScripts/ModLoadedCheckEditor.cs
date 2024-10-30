using JLL.Components;
using JLL.API;
using UnityEditor;

namespace JLLEditorModule.EditorScripts
{
    [CustomEditor(typeof(ModLoadedCheck))]
    [CanEditMultipleObjects]
    public class ModLoadedCheckEditor : JLLCustomEditor<ModLoadedCheck>
    {
        public override bool DisplayProperty(SerializedProperty property)
        {
            if (property.name == "modGUID" && Component.checkedMod != JCompatabilityHelper.CachedMods.Other)
            {
                return false;
            }
            return true;
        }
    }
}
