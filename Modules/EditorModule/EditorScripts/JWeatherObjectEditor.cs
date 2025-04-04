﻿using JLL.Components;
using UnityEditor;

namespace JLLEditorModule.EditorScripts
{
    [CustomEditor(typeof(JWeatherObject))]
    [CanEditMultipleObjects]
    public class JWeatherObjectEditor : JLLCustomEditor<JWeatherObject>
    {
        public override bool DisplayProperty(SerializedProperty property)
        {
            if (property.name == "defaultToSelf" && Component.activeObject != null)
            {
                return false;
            }
            return true;
        }
    }
}
