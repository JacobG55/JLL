using JLL.Components;
using JLLItemsModule.Components;
using UnityEditor;
using UnityEditorInternal;
using static JLL.Components.ItemSpawner;

namespace JLLEditorModule.EditorScripts
{
    [CustomEditor(typeof(JThrowableItem))]
    [CanEditMultipleObjects]
    public class JThrowableItemEditor : JLLCustomEditor<JThrowableItem>
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
            if (!Component.damageOnExplode)
            {
                if (property.name == "killDistance" || property.name == "damageRange" || property.name == "nonLethalDamage" || property.name == "pushForce" || property.name == "goThroughCar")
                {
                    return false;
                }
            }

            if (!Component.stunOnExplode)
            {
                if (property.name == "affectAudio" || property.name == "flashSeverityMultiplier" || property.name == "enemyStunTime" || property.name == "flashSeverityDistanceRolloff")
                {
                    return false;
                }
            }

            if (!Component.spawnItemsOnExplode)
            {
                if (property.name == "numberToSpawn"|| property.name == "SourcePool" || property.name == "SpawnOffsets")
                {
                    return false;
                }
            }
            if (property.name == "CustomList")
            {
                if (Component.spawnItemsOnExplode && Component.SourcePool == SpawnPoolSource.CustomList)
                {
                    weightedProperties.DoLayoutList();
                }
                return false;
            }
            return true;
        }
    }
}
