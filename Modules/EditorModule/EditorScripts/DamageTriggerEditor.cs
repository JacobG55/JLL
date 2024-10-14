using JLL.Components;
using UnityEditor;
using UnityEngine;

namespace JLLEditorModule.EditorScripts
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DamageTrigger))]
    public class DamageTriggerEditor : Editor
    {
        private DamageTrigger DamageTrigger;

        private void OnEnable()
        {
            DamageTrigger = (DamageTrigger)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            bool hasTriggerCollider = false;
            bool hasCollider = false;

            foreach (Collider collider in DamageTrigger.gameObject.GetComponents<Collider>())
            {
                if (collider.isTrigger)
                {
                    hasTriggerCollider = true;
                }
                else
                {
                    hasCollider = true;
                }
                if (hasTriggerCollider && hasCollider)
                {
                    break;
                }
            }

            SerializedProperty iterator = serializedObject.GetIterator();

            for (int i = 0; iterator.NextVisible(i == 0); i++)
            {
                EditorGUILayout.PropertyField(iterator);

                if (iterator.name == "vehicleTargets" && DamageTrigger.vehicleTargets.enabled && Physics.GetIgnoreLayerCollision(DamageTrigger.gameObject.layer, 30))
                {
                    JLLEditor.HelpMessage(
                        $"{LayerMask.LayerToName(DamageTrigger.gameObject.layer)} Layer does not interact with the Vehicle Layer.",
                        "In order to damage Vehicles you will need to make sure your colliders / Layermasks can interact with the vehicle layer.",
                        "The Collision Matrix can be found at: ProjectSettings/Physics/LayerCollisionMatrix"
                    );
                }

                if (!hasCollider && iterator.name == "damageOnCollision" && DamageTrigger.damageOnCollision)
                {
                    JLLEditor.HelpMessage("'damageOnCollision' requires a collider on the same object that has 'isTrigger' disabled.");
                }

                if (!hasTriggerCollider)
                {
                    if (iterator.name == "damageOnEnter" && DamageTrigger.damageOnEnter)
                    {
                        JLLEditor.HelpMessage("'damageOnEnter' requires a collider on the same object that has 'isTrigger' enabled.");
                    }
                    else if (iterator.name == "damageOnExit" && DamageTrigger.damageOnExit)
                    {
                        JLLEditor.HelpMessage("'damageOnExit' requires a collider on the same object that has 'isTrigger' enabled.");
                    }
                    else if (iterator.name == "continuousDamage" && DamageTrigger.continuousDamage)
                    {
                        JLLEditor.HelpMessage("'continuousDamage' requires a collider on the same object that has 'isTrigger' enabled.");
                    }
                }

                if (iterator.name == "continuousRaycastDamage" && DamageTrigger.continuousRaycastDamage && DamageTrigger.raycastDirections.Length == 0)
                {
                    JLLEditor.HelpMessage("'continuousRaycastDamage' requires at least one raycast direction to be defined in 'raycastDirections'.");
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
