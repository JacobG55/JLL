using JLLItemsModule.Components;
using UnityEditor;
using UnityEditorInternal;
using static JLL.Components.ItemSpawner;

namespace JLLEditorModule.EditorScripts
{
    [CustomEditor(typeof(JEventBoxItem))]
    [CanEditMultipleObjects]
    public class JEventBoxItemEditor : JLLCustomEditor<JEventBoxItem>
    {
        private SerializedProperty CustomList;
        private ReorderableList weightedProperties;

        public override void OnEnable()
        {
            base.OnEnable();
            CustomList = serializedObject.FindProperty("CustomList");
            weightedProperties = JLLEditor.CreateWeightedItemSpawnProperties(serializedObject, CustomList);
        }

        public override bool DisplayProperty(SerializedProperty property)
        {
            if (property.name == "CustomList")
            {
                if (Component.SourcePool == SpawnPoolSource.CustomList)
                {
                    weightedProperties.DoLayoutList();
                }
                return false;
            }
            return true;
        }
    }
}
