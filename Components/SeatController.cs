using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

namespace JLL.Components
{
    public class SeatController : NetworkBehaviour
    {
        public InteractTrigger seatTrigger;
        public InteractTrigger exitTrigger;
        private Collider[] exitColliders = new Collider[0];

        public Transform[] exitPoints = new Transform[0];
        public bool disableExitTrigger = true;

        public AudioClip sitDown;

        [HideInInspector] public PlayerControllerB? currentPassenger;
        [HideInInspector] public bool localPlayerInSeat = false;

        private static readonly int exitLayerMask = 2305;

        public void Start()
        {
            exitColliders = exitTrigger.gameObject.GetComponents<Collider>();
            ToggleExitColliders(!disableExitTrigger);
        }

        private void ToggleExitColliders(bool enabled)
        {
            for (int i = 0; i < exitColliders.Length; i++)
            {
                exitColliders[i].enabled = enabled;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void LeaveSeatServerRpc(int playerId, Vector3 exitPoint)
        {
            LeaveSeatClientRpc(playerId, exitPoint);
        }

        [ClientRpc]
        private void LeaveSeatClientRpc(int playerId, Vector3 exitPoint)
        {
            PlayerControllerB playerControllerB = StartOfRound.Instance.allPlayerScripts[playerId];
            if (!(playerControllerB == GameNetworkManager.Instance.localPlayerController))
            {
                LeaveSeat(playerControllerB, exitPoint);
            }
        }

        private void LeaveSeat(PlayerControllerB player, Vector3 exitPoint, bool forced = false)
        {
            player.TeleportPlayer(exitPoint);
            currentPassenger = null;
            if (!IsOwner)
            {
                SetVehicleCollisionForPlayer(setEnabled: true, GameNetworkManager.Instance.localPlayerController);
            }
            if (forced)
            {
                player.CancelSpecialTriggerAnimations();
                if (player.IsLocalPlayer)
                {
                    if (disableExitTrigger)
                    {
                        ToggleExitColliders(false);
                    }
                }
            }
            seatTrigger.interactable = true;
        }

        public void SetPlayerInSeat(PlayerControllerB player)
        {
            if (player != null)
            {
                if (sitDown != null)
                {
                    player.movementAudio.PlayOneShot(sitDown);
                }
                if (player == GameNetworkManager.Instance.localPlayerController)
                {
                    localPlayerInSeat = true;
                    if (disableExitTrigger)
                    {
                        ToggleExitColliders(true);
                    }
                }
                else
                {
                    seatTrigger.interactable = false;
                }
            }
            currentPassenger = player;
        }

        private int GetExitPos()
        {
            for (int j = 0; j < exitPoints.Length; j++)
            {
                if (!Physics.Linecast(GameNetworkManager.Instance.localPlayerController.gameplayCamera.transform.position, exitPoints[j].position, exitLayerMask, QueryTriggerInteraction.Ignore))
                {
                    return j;
                }
            }
            return -1;
        }

        private Vector3 GetExitPoint()
        {
            int pos = GetExitPos();
            if (pos != -1)
            {
                return exitPoints[pos].position;
            }
            else
            {
                return transform.position + Vector3.up;
            }
        }

        public void OnCancelAnim()
        {
            seatTrigger.interactable = true;
            localPlayerInSeat = false;
            currentPassenger = null;
            SetVehicleCollisionForPlayer(setEnabled: true, GameNetworkManager.Instance.localPlayerController);
            LeaveSeatServerRpc((int)GameNetworkManager.Instance.localPlayerController.playerClientId, GameNetworkManager.Instance.localPlayerController.transform.position);
        }

        void OnDisable()
        {
            if (currentPassenger != null)
            {
                LeaveSeat(currentPassenger, GetExitPoint(), true);
            }
        }

        public override void OnDestroy()
        {
            if (currentPassenger != null)
            {
                LeaveSeat(currentPassenger, GetExitPoint(), true);
            }
            base.OnDestroy();
        }

        public void ExitSeat()
        {
            if (localPlayerInSeat)
            {
                if (disableExitTrigger)
                {
                    ToggleExitColliders(false);
                }
                GameNetworkManager.Instance.localPlayerController.TeleportPlayer(GetExitPoint());
            }
        }

        public void SetVehicleCollisionForPlayer(bool setEnabled, PlayerControllerB player)
        {
            if (setEnabled)
            {
                player.GetComponent<CharacterController>().excludeLayers = 0;
            }
            else
            {
                player.GetComponent<CharacterController>().excludeLayers = 1073741824;
            }
        }
    }
}
