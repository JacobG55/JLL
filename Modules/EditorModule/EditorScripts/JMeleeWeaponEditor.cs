using JLLItemsModule.Components;
using UnityEditor;

namespace JLLEditorModule.EditorScripts
{
    [CustomEditor(typeof(JMeleeWeapon))]
    [CanEditMultipleObjects]
    public class JMeleeWeaponEditor : JLLCustomEditor<JMeleeWeapon>
    {
        public override bool DisplayProperty(SerializedProperty property)
        {
            if (!Component.isHeavyWeapon && (property.name == "reelingTime" || property.name == "swingTime" || property.name == "reelUpSFX"))
            {
                return false;
            }

            if (property.name == "OnPlayerHit" && !Component.damagePlayers)
            {
                return false;
            }
            if (property.name == "OnEnemyHit" && !Component.damageEnemies)
            {
                return false;
            }
            if (property.name == "OnVehicleHit" && !Component.damageVehicles)
            {
                return false;
            }
            if (property.name == "OnObjectHit" && !Component.damageObjects)
            {
                return false;
            }
            return true;
        }
    }
}
