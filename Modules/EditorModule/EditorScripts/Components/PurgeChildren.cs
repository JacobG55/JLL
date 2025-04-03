using System.Collections.Generic;
using UnityEngine;

namespace JLLEditorModule.EditorScripts.Components
{
    [ExecuteInEditMode]
    public class PurgeChildren : MonoBehaviour
    {
        public bool Execute = false;
        public int split = 2;

        public void Update()
        {
            if (Execute)
            {
                Execute = false;

                GameObject container = new GameObject("Container");
                container.transform.parent = transform;

                List<Transform> SplitTransforms = new List<Transform>();
                for (int i = split; i < transform.childCount; i += split)
                {
                    SplitTransforms.Add(transform.GetChild(i));
                }

                foreach (Transform t in SplitTransforms)
                {
                    t.SetParent(container.transform);
                }
            }
        }
    }
}
