using JLL.API.Events;
using UnityEngine;

namespace JLLItemsModule.Components
{
    public class JGrabbableObject : GrabbableObject
    {
        [Header("JGrabbableObject")]
        public GameObject chargedEffects;
        public GameObject[] hideWhenPocketed = new GameObject[0];

        public BoolEvent OnSetInShip = new BoolEvent();

        private bool pocketedLastFrame;

        public override void Start()
        {
            base.Start();
            pocketedLastFrame = isPocketed;
        }

        public override void ItemInteractLeftRight(bool right)
        {
            base.ItemInteractLeftRight(right);

            if (playerHeldBy != null)
            {
                if (isHeld) HeldUpdate();
            }

            if (itemProperties.requiresBattery && insertedBattery != null)
            {
                chargedEffects?.SetActive(!insertedBattery.empty);
            }
        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            if (pocketedLastFrame != isPocketed)
            {
                foreach (GameObject obj in hideWhenPocketed)
                {
                    obj.SetActive(!isPocketed);
                }
                pocketedLastFrame = isPocketed;
            }
        }

        public virtual void HeldUpdate()
        {

        }

        public virtual void OnSetInsideShip(bool isEntering)
        {

        }
    }
}
