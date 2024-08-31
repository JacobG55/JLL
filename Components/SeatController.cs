using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

namespace JLL.Components
{
    public class SeatController : NetworkBehaviour
    {
        public InteractTrigger seatTrigger;
        public InteractTrigger exitTrigger;

        public Transform[] exitPoints = new Transform[0];
        public bool disableExitTrigger = true;

        public AudioClip sitDown;

        [HideInInspector] public PlayerControllerB? currentPassenger;
        [HideInInspector] public bool localPlayerInSeat = false;

        private int exitLayerMask = 2305;

        public void Start()
        {
            exitTrigger.gameObject.SetActive(!disableExitTrigger);
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
                playerControllerB.TeleportPlayer(exitPoint);
                currentPassenger = null;
                if (!base.IsOwner)
                {
                    SetVehicleCollisionForPlayer(setEnabled: true, GameNetworkManager.Instance.localPlayerController);
                }
                seatTrigger.interactable = true;
            }
        }

        public void SetPlayerInSeat(PlayerControllerB player)
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
                    exitTrigger.gameObject.SetActive(true);
                }
            }
            else
            {
                seatTrigger.interactable = false;
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

        public void OnCancelAnim()
        {
            seatTrigger.interactable = true;
            localPlayerInSeat = false;
            currentPassenger = null;
            SetVehicleCollisionForPlayer(setEnabled: true, GameNetworkManager.Instance.localPlayerController);
            LeaveSeatServerRpc((int)GameNetworkManager.Instance.localPlayerController.playerClientId, GameNetworkManager.Instance.localPlayerController.transform.position);
        }

        public void ExitSeat()
        {
            if (localPlayerInSeat)
            {
                if (disableExitTrigger)
                {
                    exitTrigger.gameObject.SetActive(false);
                }
                int pos = GetExitPos();
                if (pos != -1)
                {
                    GameNetworkManager.Instance.localPlayerController.TeleportPlayer(exitPoints[pos].position);
                }
                else
                {
                    GameNetworkManager.Instance.localPlayerController.TeleportPlayer(transform.position + Vector3.up);
                }
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
