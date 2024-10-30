using JLL.Components;
using UnityEditor;
using UnityEngine;

namespace JLLEditorModule.EditorScripts
{
    [CustomEditor(typeof(DamageTrigger))]
    [CanEditMultipleObjects]
    public class DamageTriggerEditor : JLLCustomEditor<DamageTrigger>
    {
        bool hasTriggerCollider = false;
        bool hasCollider = false;
        int indent = 0;

        public override void AtHeader()
        {
            base.AtHeader();

            hasTriggerCollider = false;
            hasCollider = false;

            foreach (Collider collider in Component.gameObject.GetComponents<Collider>())
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
        }

        public override bool DisplayProperty(SerializedProperty property)
        {
            if (Component.DamageTriggerNetworking == null && (property.name == "attachCorpseToPoint" || property.name == "OverrideCorpseMesh"))
            {
                return false;
            }
            if (!Component.attachCorpseToPoint || Component.DamageTriggerNetworking == null)
            {
                if (property.name == "corpseAttachPoint" || property.name == "matchPointExactly" || property.name == "corpseStickTime" || property.name == "connectedBone")
                {
                    return false;
                }
            }
            else
            {
                if (indent == 0)
                {
                    if (property.name == "attachCorpseToPoint")
                    {
                        indent++;
                    }
                    else if (property.name == "corpseStickTime")
                    {
                        indent--;
                    }
                }
                else
                {
                    EditorGUI.indentLevel += indent;
                    indent = 0;
                }
            }
            return true;
        }

        public override void DisplayWarnings(SerializedProperty property)
        {
            if (property.name == "vehicleTargets" && Component.vehicleTargets.enabled && Physics.GetIgnoreLayerCollision(Component.gameObject.layer, 30))
            {
                JLLEditor.HelpMessage(
                    $"{LayerMask.LayerToName(Component.gameObject.layer)} Layer does not interact with the Vehicle Layer.",
                    "In order to damage Vehicles you will need to make sure your colliders / Layermasks can interact with the vehicle layer.",
                    "The Collision Matrix can be found at: ProjectSettings/Physics/LayerCollisionMatrix"
                );
            }

            if (!hasCollider && property.name == "damageOnCollision" && Component.damageOnCollision)
            {
                JLLEditor.HelpMessage("'damageOnCollision' requires a collider on the same object that has 'isTrigger' disabled.");
            }

            if (!hasTriggerCollider)
            {
                if (property.name == "damageOnEnter" && Component.damageOnEnter)
                {
                    JLLEditor.HelpMessage("'damageOnEnter' requires a collider on the same object that has 'isTrigger' enabled.");
                }
                else if (property.name == "damageOnExit" && Component.damageOnExit)
                {
                    JLLEditor.HelpMessage("'damageOnExit' requires a collider on the same object that has 'isTrigger' enabled.");
                }
                else if (property.name == "continuousDamage" && Component.continuousDamage)
                {
                    JLLEditor.HelpMessage("'continuousDamage' requires a collider on the same object that has 'isTrigger' enabled.");
                }
            }

            if (property.name == "continuousRaycastDamage" && Component.continuousRaycastDamage && Component.raycastDirections.Length == 0)
            {
                JLLEditor.HelpMessage("'continuousRaycastDamage' requires at least one raycast direction to be defined in 'raycastDirections'.");
            }
        }
    }
}
