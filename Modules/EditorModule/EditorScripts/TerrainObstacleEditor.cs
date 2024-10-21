using JLL.Components;
using UnityEditor;
using UnityEngine;

namespace JLLEditorModule.EditorScripts
{
    [CustomEditor(typeof(TerrainObstacle))]
    [CanEditMultipleObjects]
    public class TerrainObstacleEditor : JLLCustomEditor<TerrainObstacle>
    {
        public override void AtHeader()
        {
            base.AtHeader();

            if (Physics.GetIgnoreLayerCollision(Component.gameObject.layer, 30))
            {
                JLLEditor.HelpMessage(
                    $"{LayerMask.LayerToName(Component.gameObject.layer)} Layer does not interact with the Vehicle Layer.",
                    "In order for this script to work you will need to make sure your colliders / Layermasks can interact with the vehicle layer.",
                    "The Collision Matrix can be found at: ProjectSettings/Physics/LayerCollisionMatrix"
                );
            }

            if (Component.TryGetComponent(out MeshCollider collider))
            {
                JLLEditor.HelpMessage(
                    "Mesh Colliders may cause problems being detected by other scripts.",
                    "Please consider using a different type of collider for use with TerrainObstacles."
                );
            }
        }

        public override bool DisplayProperty(SerializedProperty property)
        {
            return true;
        }
    }
}
