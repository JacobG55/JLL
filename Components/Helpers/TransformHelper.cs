using UnityEngine;

namespace JLL.Components.Helpers
{
    public class TransformHelper : MonoBehaviour
    {
        public void ParentToThis(MonoBehaviour target)
        {
            ParentToThis(target.gameObject);
        }

        public void ParentToThis(GameObject target)
        {
            target.transform.parent = transform;
        }

        public void Unparent(MonoBehaviour target)
        {
            Unparent(target.gameObject);
        }

        public void Unparent(GameObject target)
        {
            target.transform.parent = null;
        }

        public void ParentTo(MonoBehaviour target)
        {
            ParentTo(target.gameObject);
        }

        public void ParentTo(GameObject target)
        {
            transform.parent = target.transform;
        }
    }
}
