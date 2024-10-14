using GameNetcodeStuff;
using JLL.Components;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static JLL.Components.DamageTrigger;

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

        [ServerRpc(RequireOwnership = false)]
        public void RandomTeleportServerRpc(int playerTarget, int selection, bool withRotation = false, float rotation = 0, float range = 10f)
        {
            if (Enum.IsDefined(typeof(RandomTeleportRegion), selection))
            {
                List<GameObject> nodes = new List<GameObject>();

                switch (selection)
                {
                    case (int)RandomTeleportRegion.Indoor:
                        nodes.AddRange(RoundManager.Instance.insideAINodes);
                        break;
                    case (int)RandomTeleportRegion.Outdoor:
                        nodes.AddRange(RoundManager.Instance.outsideAINodes);
                        break;
                    case (int)RandomTeleportRegion.Moon:
                        nodes.AddRange(RoundManager.Instance.insideAINodes);
                        nodes.AddRange(RoundManager.Instance.outsideAINodes);
                        break;
                    case (int)RandomTeleportRegion.Nearby:
                        nodes.Add(RoundManager.Instance.playersManager.allPlayerObjects[playerTarget]);
                        break;
                    case (int)RandomTeleportRegion.RandomPlayer:
                        nodes.AddRange(RoundManager.Instance.playersManager.allPlayerObjects);
                        break;
                    default: break;
                }

                if (nodes.Count > 0)
                {
                    RandomTeleportClientRpc(playerTarget, RoundManager.Instance.GetRandomNavMeshPositionInRadius(nodes[UnityEngine.Random.Range(0, nodes.Count)].transform.position, range), withRotation, rotation);
                }
            }
        }

        [ClientRpc]
        private void RandomTeleportClientRpc(int playerTarget, Vector3 pos, bool withRotation = false, float rotation = 0)
        {
            RoundManager.Instance.playersManager.allPlayerScripts[playerTarget].TeleportPlayer(pos, withRotation, rotation);
        }

        [ServerRpc(RequireOwnership = false)]
        public void DamageTriggerKilledPlayerServerRpc(string objectPath, int playerTarget)
        {
            DamageTriggerKilledPlayerClientRpc(objectPath, (int)ColliderType.Player, playerTarget);
        }

        [ServerRpc(RequireOwnership = true)]
        public void DamageTriggerKilledServerRpc(string objectPath, int type)
        {
            DamageTriggerKilledPlayerClientRpc(objectPath, type);
        }

        [ClientRpc]
        private void DamageTriggerKilledPlayerClientRpc(string objectPath, int type, int playerTarget = -1)
        {
            GameObject obj = GameObject.Find(objectPath);

            if (obj && obj.TryGetComponent(out DamageTrigger damageTrigger))
            {
                switch (type)
                {
                    case (int)ColliderType.Player:
                        if (playerTarget >= 0)
                        {
                            PlayerControllerB player = RoundManager.Instance.playersManager.allPlayerScripts[playerTarget];

                            if (player.isPlayerDead)
                            {
                                if (player.deadBody != null)
                                {
                                    Destroy(player.deadBody.gameObject);
                                    player.deadBody = null;
                                }

                                damageTrigger.playerTargets.killEvent.Invoke();
                            }
                        }
                        break;
                    case (int)ColliderType.Enemy:
                        damageTrigger.enemyTargets.killEvent.Invoke();
                        break;
                    case (int)ColliderType.Vehicle:
                        damageTrigger.vehicleTargets.killEvent.Invoke();
                        break;
                    default: break;
                }
            }
        }

        public static string GetPath(Transform current)
        {
            if (current.parent == null) return "/" + current.name;
            return GetPath(current.parent) + "/" + current.name;
        }
    }
}
