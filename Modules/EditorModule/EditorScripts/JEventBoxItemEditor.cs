using JLL.Components;
using JLLItemsModule.Components;
using UnityEditor;
using UnityEditorInternal;

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

        public override void AtHeader()
        {
            base.AtHeader();
            foreach (var itemWeight in Component.CustomList)
            {
                if (itemWeight.ItemName != "")
                {
                    itemWeight.Item = null;
                }
            }
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
