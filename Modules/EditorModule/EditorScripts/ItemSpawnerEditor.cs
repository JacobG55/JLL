using JLL.Components;
using UnityEditor;
using UnityEditorInternal;
using static JLL.Components.ItemSpawner;

namespace JLLEditorModule.EditorScripts
{
    [CustomEditor(typeof(ItemSpawner))]
    [CanEditMultipleObjects]
    public class ItemSpawnerEditor : JLLCustomEditor<ItemSpawner>
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
