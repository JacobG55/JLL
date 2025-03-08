using GameNetcodeStuff;
using JLL.Components;
using System;
using Unity.Netcode;
using UnityEngine;

namespace JLL.API
{
    public class JLLNetworkManager : NetworkBehaviour
    {
        public static JLLNetworkManager Instance;

        public Collider[] tempColliderResults = new Collider[20];

        public void Awake()
        {
            Instance = this;
        }

        public void DestroyTerrainObstacleOnLocalClient(Vector3 pos, int damage)
        {
            if (DestroyTerrainObstacleAtPosition(pos, damage))
            {
                JLogHelper.LogInfo($"Sending Terrain Obstacle RPC! {pos} {damage}", JLogLevel.Wesley);
                BreakTerrainObstacleServerRpc(pos, damage, (int)GameNetworkManager.Instance.localPlayerController.playerClientId);
            }
        }

        private bool DestroyTerrainObstacleAtPosition(Vector3 pos, int damage)
        {
            int num = Physics.OverlapSphereNonAlloc(pos, 5f, tempColliderResults, 33554432, QueryTriggerInteraction.Ignore);
            if (num == 0)
            {
                return false;
            }
            bool success = false;
            for (int i = 0; i < num; i++)
            {
                if (tempColliderResults[i].TryGetComponent(out TerrainObstacle obstacle))
                {
                    obstacle.Damage(damage);
                    success = true;
                }
                JLogHelper.LogInfo($"Damaging {tempColliderResults[i]} {success}", JLogLevel.Wesley);
            }
            if (success)
            {
                float num2 = Vector3.Distance(StartOfRound.Instance.audioListener.transform.position, pos);
                if (num2 < 15f)
                {
                    HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
                }
                else if (num2 < 25f)
                {
                    HUDManager.Instance.ShakeCamera(ScreenShakeType.Small);
                }
                return true;
            }
            return false;
        }

        [ServerRpc(RequireOwnership = false)]
        private void BreakTerrainObstacleServerRpc(Vector3 pos, int damage, int playerWhoSent)
        {
            BreakTerrainObstacleClientRpc(pos, damage, playerWhoSent);
        }

        [ClientRpc]
        private void BreakTerrainObstacleClientRpc(Vector3 pos, int damage, int playerWhoSent)
        {
            JLogHelper.LogInfo($"Received RPC! {pos} {damage} {playerWhoSent}", JLogLevel.Wesley);
            if ((int)GameNetworkManager.Instance.localPlayerController.playerClientId != playerWhoSent)
            {
                DestroyTerrainObstacleAtPosition(pos, damage);
            }
        }
        public void DestroyPlayerCorpse(PlayerControllerB player)
        {
            DestroyPlayerCorpseServerRpc(player.Index());
        }

        [ServerRpc(RequireOwnership = false)]
        private void DestroyPlayerCorpseServerRpc(int playerTarget)
        {
            DestroyPlayerCorpseClientRpc(playerTarget);
        }

        [ClientRpc]
        private void DestroyPlayerCorpseClientRpc(int playerTarget)
        {
            PlayerControllerB player = RoundManager.Instance.playersManager.allPlayerScripts[playerTarget];
            if (player.isPlayerDead && player.deadBody != null)
            {
                Destroy(player.deadBody.gameObject);
                player.deadBody = null;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void RandomTeleportServerRpc(int playerTarget, int selection, bool withRotation = false, float rotation = 0, float range = 10f)
        {
            if (Enum.IsDefined(typeof(RandomTeleportRegion), selection))
            {
                GameObject? node = null;
                bool inside = false;

                switch (selection)
                {
                    case (int)RandomTeleportRegion.Indoor:
                        node = GetRandom(RoundManager.Instance.insideAINodes);
                        inside = true;
                        break;
                    case (int)RandomTeleportRegion.Outdoor:
                        node = GetRandom(RoundManager.Instance.outsideAINodes);
                        break;
                    case (int)RandomTeleportRegion.Moon:
                        int random = UnityEngine.Random.Range(0, RoundManager.Instance.outsideAINodes.Length + RoundManager.Instance.insideAINodes.Length);
                        if (random < RoundManager.Instance.outsideAINodes.Length)
                        {
                            node = RoundManager.Instance.insideAINodes[random];
                        }
                        else
                        {
                            inside = true;
                            node = RoundManager.Instance.outsideAINodes[random - RoundManager.Instance.outsideAINodes.Length];
                        }
                        break;
                    case (int)RandomTeleportRegion.Nearby:
                        PlayerControllerB player = RoundManager.Instance.playersManager.allPlayerScripts[playerTarget];
                        node = player.gameObject;
                        inside = player.isInsideFactory;
                        break;
                    case (int)RandomTeleportRegion.RandomPlayer:
                        PlayerControllerB randomPlayer = GetRandom(RoundManager.Instance.playersManager.allPlayerScripts);
                        node = randomPlayer.gameObject;
                        inside = randomPlayer.isInsideFactory;
                        break;
                    default: break;
                }

                if (node != null)
                {
                    RandomTeleportClientRpc(playerTarget, RoundManager.Instance.GetRandomNavMeshPositionInRadius(node.transform.position, range), inside, withRotation, rotation);
                }
            }
        }

        [ClientRpc]
        private void RandomTeleportClientRpc(int playerTarget, Vector3 pos, bool inside, bool withRotation = false, float rotation = 0)
        {
            JLogHelper.LogInfo($"Random Teleporting {playerTarget}", JLogLevel.Debuging);
            PlayerControllerB player = RoundManager.Instance.playersManager.allPlayerScripts[playerTarget];
            player.TeleportPlayer(pos, withRotation, rotation);
            player.isInsideFactory = inside;
        }

        public static T GetRandom<T>(T[] array)
        {
            return array[UnityEngine.Random.Range(0, array.Length)];
        }

        public static string GetPath(Transform current)
        {
            if (current.parent == null) return "/" + current.name;
            return GetPath(current.parent) + "/" + current.name;
        }

        [ServerRpc(RequireOwnership = true)]
        public void UpdateScanNodeServerRpc(NetworkObjectReference target, int value)
        {
            UpdateScanNodeClientRpc(target, value);
        }

        [ClientRpc]
        private void UpdateScanNodeClientRpc(NetworkObjectReference target, int value)
        {
            if (target.TryGet(out NetworkObject netObj) && netObj.gameObject.TryGetComponent(out GrabbableObject item))
            {
                item.SetScrapValue(value);
            }
            else
            {
                JLogHelper.LogInfo("Networked GrabbableObject could not be found. Safely ignoring operation.");
            }
        }
    }
}
