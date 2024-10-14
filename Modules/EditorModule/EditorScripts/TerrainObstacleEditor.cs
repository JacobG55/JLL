using JLL.Components;
using UnityEditor;
using UnityEngine;

namespace JLLEditorModule.EditorScripts
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TerrainObstacle))]
    public class TerrainObstacleEditor : Editor
    {
        private TerrainObstacle TerrainObstacle;

        private void OnEnable()
        {
            TerrainObstacle = (TerrainObstacle)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (Physics.GetIgnoreLayerCollision(TerrainObstacle.gameObject.layer, 30))
            {
                JLLEditor.HelpMessage(
                    $"{LayerMask.LayerToName(TerrainObstacle.gameObject.layer)} Layer does not interact with the Vehicle Layer.",
                    "In order for this script to work you will need to make sure your colliders / Layermasks can interact with the vehicle layer.",
                    "The Collision Matrix can be found at: ProjectSettings/Physics/LayerCollisionMatrix"
                );
            }

            if (TerrainObstacle.TryGetComponent(out MeshCollider collider))
            {
                JLLEditor.HelpMessage(
                    "Mesh Colliders may cause problems being detected by other scripts.",
                    "Please consider using a different type of collider for use with TerrainObstacles."
                );
            }

            SerializedProperty iterator = serializedObject.GetIterator();
            for (int i = 0; iterator.NextVisible(i == 0); i++)
            {
                EditorGUILayout.PropertyField(iterator);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
