using JLLItemsModule.Components;
using UnityEditor;

namespace JLLEditorModule.EditorScripts
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(JMeleeWeapon))]
    public class JMeleeWeaponEditor : Editor
    {
        private JMeleeWeapon JMeleeWeapon;

        private void OnEnable()
        {
            JMeleeWeapon = (JMeleeWeapon)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty iterator = serializedObject.GetIterator();

            for (int i = 0; iterator.NextVisible(i == 0); i++)
            {
                if (!JMeleeWeapon.isHeavyWeapon && (iterator.name == "reelingTime" || iterator.name == "swingTime" || iterator.name == "reelUpSFX"))
                { 
                    continue; 
                }

                if (iterator.name == "OnPlayerHit" && !JMeleeWeapon.damagePlayers)
                {
                    continue;
                }
                if (iterator.name == "OnEnemyHit" && !JMeleeWeapon.damageEnemies)
                {
                    continue;
                }
                if (iterator.name == "OnVehicleHit" && !JMeleeWeapon.damageVehicles)
                {
                    continue;
                }
                if (iterator.name == "OnObjectHit" && !JMeleeWeapon.damageObjects)
                {
                    continue;
                }

                EditorGUILayout.PropertyField(iterator);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
