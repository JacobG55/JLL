using JLL.API.Events;
using UnityEngine;
using UnityEngine.Events;

namespace JLLItemsModule.Components
{
    public class JGrabbableObject : GrabbableObject
    {
        [Header("JGrabbableObject")]
        public GameObject chargedEffects;
        public GameObject[] hideWhenPocketed = new GameObject[0];

        public UnityEvent OnEquip = new UnityEvent();
        public InteractEvent OnEquipPlayer = new InteractEvent();
        public UnityEvent OnPocketed = new UnityEvent();
        public InteractEvent OnPocketedPlayer = new InteractEvent();
        public BoolEvent OnSetInShip = new BoolEvent();
        public UnityEvent OnPlacedOnDepositDesk = new UnityEvent();

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

        public override void EquipItem()
        {
            base.EquipItem();
            foreach (GameObject obj in hideWhenPocketed)
            {
                obj.SetActive(true);
            }
            OnEquip.Invoke();
            if (playerHeldBy != null) OnEquipPlayer.Invoke(playerHeldBy);
        }
        public override void PocketItem()
        {
            base.PocketItem();
            foreach (GameObject obj in hideWhenPocketed)
            {
                obj.SetActive(false);
            }
            OnPocketed.Invoke();
            if (playerHeldBy != null) OnPocketedPlayer.Invoke(playerHeldBy);
        }

        public virtual void HeldUpdate()
        {

        }

        public virtual void OnSetInsideShip(bool isEntering)
        {

        }

        public virtual void PlacedOnDepositDesk()
        {

        }

        public bool IsBeingHeldByPlayer()
        {
            return playerHeldBy != null && isHeld;
        }

        public void DamageHolder(int damage)
        {
            if (IsBeingHeldByPlayer())
            {
                playerHeldBy.DamagePlayer(damage);
            }
        }

        public void DestroyItemInHand()
        {
            if (IsBeingHeldByPlayer())
            {
                DestroyObjectInHand(playerHeldBy);
            }
        }

        public void ForceDrop(bool dropAll)
        {
            if (IsBeingHeldByPlayer())
            {
                if (dropAll)
                {
                    playerHeldBy.DropAllHeldItemsAndSync();
                }
                else
                {
                    playerHeldBy.DiscardHeldObject();
                }
            }
        }
    }
}
