using UnityEngine;

namespace JLL.Components
{
    [RequireComponent(typeof(ItemDropship))]
    public class JItemDropshipModifier : MonoBehaviour
    {
        public ItemDropship ItemDropship;

        [Header("Modifiers")]
        public Transform itemParent;

        public virtual void Start()
        {
            if (ItemDropship == null)
            {
                ItemDropship = GetComponent<ItemDropship>();
            }
        }

        public virtual void ModifyDroppedItems(GrabbableObject item)
        {
            if (itemParent != null)
            {
                item.transform.parent = itemParent;
            }
        }
    }
}
