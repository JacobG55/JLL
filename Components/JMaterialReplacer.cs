using System;
using UnityEngine;

namespace JLL.Components
{
    public class JMaterialReplacer : MonoBehaviour
    {
        public MaterialReplacement[] replacements;
        public bool searchChildren = true;
        public bool triggerPostDunGen = false;

        [Serializable]
        public class MaterialReplacement
        {
            public Material original;
            public Material replacement;
        }

        public void SearchAndReplace()
        {
            SearchAndReplace(transform);
        }

        private void SearchAndReplace(Transform target)
        {
            if (target.TryGetComponent(out Renderer renderer))
            {
                int success = 0;
                for (int i = 0; i < replacements.Length; i++)
                {
                    Material[] mats = renderer.materials;
                    for (int j = 0; j < mats.Length; j++)
                    {
                        if (mats[j].name.Equals($"{replacements[i].original.name} (Instance)"))
                        {
                            mats[j] = replacements[i].replacement;
                            success++;
                        }
                    }
                    target.GetComponent<Renderer>().materials = mats;
                }
            }
            if (searchChildren)
            {
                for(int i = 0; i < target.transform.childCount; i++)
                {
                    SearchAndReplace(target.transform.GetChild(i));
                }
            }
        }
    }
}
