using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace JLLEditorModule.EditorScripts
{
    public abstract class JLLCustomEditor<T> : Editor where T : UnityEngine.MonoBehaviour
    {
        public T Component;

        public bool hasTriggerCollider = false;
        public bool hasCollider = false;

        public virtual void OnEnable()
        {
            Component = (T)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            AtHeader();

            SerializedProperty iterator = serializedObject.GetIterator();

            bool first = true;
            for (int i = 0; iterator.NextVisible(first); i++)
            {
                if (first)
                {
                    first = false;
                    continue;
                }
                if (DisplayProperty(iterator))
                {
                    EditorGUILayout.PropertyField(iterator);
                    DisplayWarnings(iterator);
                }
            }

            AtFooter();

            serializedObject.ApplyModifiedProperties();
        }

        public virtual void AtHeader()
        {
            if (Component is NetworkBehaviour behavior && behavior.GetComponent<NetworkObject>() == null)
            {
                JLLEditor.HelpMessage(
                    "Network Behaviours require a Network Object to run some of their code. Ensure you have a Network Object attatched to this script in some way."
                );
            }
        }

        public void CheckForColliders()
        {
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

        public abstract bool DisplayProperty(SerializedProperty property);

        public virtual void DisplayWarnings(SerializedProperty property)
        {

        }
        public static bool IsInLayerMask(int layer, LayerMask mask) => (mask.value & (1 << layer)) != 0;

        public void WarnIncompatibleLayer(GameObject gameObject, int layer, bool checkColliderOverrides, params string[] messages)
        {
            bool overrideAllows = false;
            bool overrideBlocks = false;
            if (checkColliderOverrides)
            {
                foreach (Collider collider in gameObject.GetComponents<Collider>())
                {
                    if (collider.layerOverridePriority <= 0) continue;
                    if (IsInLayerMask(layer, collider.includeLayers)) overrideAllows = true;
                    if (IsInLayerMask(layer, collider.excludeLayers)) overrideBlocks = true;
                }
            }

            if (!overrideAllows && (Physics.GetIgnoreLayerCollision(gameObject.layer, layer) || overrideBlocks))
            {
                List<string> msgs = new List<string>
                {
                    $"{LayerMask.LayerToName(gameObject.layer)} Layer does not interact with the ${LayerMask.LayerToName(layer)} Layer."
                };
                msgs.AddRange(messages);
                msgs.Add("The Collision Matrix can be found at: ProjectSettings/Physics/LayerCollisionMatrix");
                JLLEditor.HelpMessage(msgs.ToArray());
            }
        }

        public void WarnVehicleLayer(params string[] messages)
            => WarnIncompatibleLayer(Component.gameObject, 30, true, messages);

        public virtual void AtFooter()
        {

        }
    }
}
