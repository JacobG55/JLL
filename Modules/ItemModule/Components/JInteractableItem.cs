using GameNetcodeStuff;
using JLL.API.Events;
using UnityEngine;
using UnityEngine.Events;

namespace JLLItemsModule.Components
{
    public class JInteractableItem : JGrabbableObject
    {
        [Header("Interactable Item")]
        public InteractionType interactionType = InteractionType.Press;
        public InteractEvent OnUse = new InteractEvent();
        public BoolEvent OnToggle = new BoolEvent();
        public UnityEvent OnHoldingButton = new UnityEvent();

        [Header("Extra Events")]
        public InteractEvent CollisionEvent = new InteractEvent();

        [Space(5f)]
        public AudioClip[] interactSFX = new AudioClip[0];
        public AudioClip[] disableSFX = new AudioClip[0];
        public AudioSource noiseAudio;

        public float maxVolume = 1f;
        public float minVolume = 0.8f;

        public Animator? triggerAnimator;

        private PlayerControllerB? playerLastHeld;

        public enum InteractionType
        {
            Press,
            Toggle,
            Hold
        }

        public override void EquipItem()
        {
            base.EquipItem();
            playerLastHeld = playerHeldBy;
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            if (used && playerLastHeld != null)
            {
                OnUse.Invoke(playerLastHeld);
            }
            switch (interactionType)
            {
                case InteractionType.Toggle:
                    ToggleInteract(used);
                    break;
                case InteractionType.Hold:
                    if (buttonDown)
                    {
                        OnHoldInteraction();
                        OnHoldingButton.Invoke();
                    }
                    break;
                default:
                    InteractFX(interactSFX, "interact");
                    break;
            }
        }

        public virtual void InteractFX(AudioClip[] clips, string trigger)
        {
            triggerAnimator?.SetTrigger(trigger);
            if (noiseAudio != null && noiseAudio && clips.Length > 0)
            {
                int random = Random.Range(0, clips.Length);
                if (clips[random] != null)
                {
                    float volume = Random.Range(minVolume, maxVolume);
                    noiseAudio.PlayOneShot(clips[random], volume);
                    RoundManager.Instance.PlayAudibleNoise(noiseAudio.transform.position, 4f * volume, volume * 0.5f, 0);
                }
            }
        }

        public void ToggleInteract()
        {
            ToggleInteract(!isBeingUsed);
        }

        public void ToggleInteract(bool on)
        {
            if (on)
            {
                InteractFX(interactSFX, "toggleOn");
            }
            else
            {
                InteractFX(disableSFX, "toggleOff");
            }
            isBeingUsed = on;
            OnToggled(on);
            OnToggle.Invoke(on);
        }

        public virtual void OnToggled(bool on)
        {

        }

        public virtual void OnHoldInteraction()
        {

        }

        public override void OnHitGround()
        {
            base.OnHitGround();
            if (playerLastHeld != null)
            {
                CollisionEvent.Invoke(playerLastHeld);
            }
        }
    }
}
