using JLL.Components;
using UnityEditor;
using UnityEngine;

namespace JLLEditorModule.EditorScripts
{
    [CustomEditor(typeof(JBridgeTrigger))]
    [CanEditMultipleObjects]
    public class JBridgeTriggerEditor : JLLCustomEditor<JBridgeTrigger>
    {
        public override void AtHeader()
        {
            base.AtHeader();

            bool hasTriggerCollider = false;

            foreach (Collider collider in Component.gameObject.GetComponents<Collider>())
            {
                if (collider.isTrigger)
                {
                    hasTriggerCollider = true;
                    break;
                }
            }

            if (!hasTriggerCollider || Component.bridgeAnimator == null || Component.bridgeAudioSource == null)
            {
                JLLEditor.HelpMessage("This script is missing properties that are required for this component to function", "It is recomended to base this off of the vanilla bridges on Vow or Adamance to ensure it is set up correctly.");
            }

            WarnVehicleLayer("If you intend for this bridge to be collapsable by cruisers then you need to set the colliders to a layer that can interact with cruisers");
        }

        public override bool DisplayProperty(SerializedProperty property)
        {
            return true;
        }
    }
}
