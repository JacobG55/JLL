using GameNetcodeStuff;
using JLL.API;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using static JLL.Components.DamageTrigger;

namespace JLL.Components
{
    public class DamageTriggerNetworking : NetworkBehaviour
    {
        public DamageTrigger DamageTrigger;

        public UnityEvent PlayerKilled = new UnityEvent();
        public UnityEvent EnemyKilled = new UnityEvent();
        public UnityEvent VehicleKilled = new UnityEvent();

        private readonly List<PlayerCorpse> playerCorpses = new List<PlayerCorpse>();
        private readonly List<PlayerCorpse> removeCorpses = new List<PlayerCorpse>();

        private class PlayerCorpse
        {
            public PlayerControllerB player;
            public float startTime;
            public float stickTime = 0;
            private bool stuck = false;

            public bool Update(DamageTrigger trigger)
            {
                if (player.deadBody != null)
                {
                    if (trigger.corpseType < 0)
                    {
                        Destroy(player.deadBody.gameObject);
                        player.deadBody = null;
                        return false;
                    }

                    if (stickTime > 0)
                    {
                        if (stuck)
                        {
                            stickTime -= Time.deltaTime;
                        }
                        else if (trigger.attachCorpseToPoint && trigger.corpseAttachPoint != null)
                        {
                            JLogHelper.LogInfo($"Attatching {player.playerUsername}'s corpse to {trigger.corpseAttachPoint.name}");
                            player.deadBody.matchPositionExactly = trigger.matchPointExactly;
                            player.deadBody.attachedTo = trigger.corpseAttachPoint;
                            player.deadBody.attachedLimb = player.deadBody.bodyParts[(int)trigger.connectedBone];
                            stuck = true;
                        }

                        if (stickTime <= 0)
                        {
                            JLogHelper.LogInfo($"Releasing {player.playerUsername}'s corpse");
                            player.deadBody.matchPositionExactly = false;
                            player.deadBody.attachedTo = null;
                            player.deadBody.attachedLimb = null;
                            stuck = false;
                        }
                    }
                    else
                    {
                        if (trigger.OverrideCorpseMesh != null)
                        {
                            player.deadBody.ChangeMesh(trigger.OverrideCorpseMesh);
                        }
                        return false;
                    }
                }
                else if (Time.realtimeSinceStartup - startTime > 2f)
                {
                    JLogHelper.LogInfo("Player Corpse could not be found after two seconds!");
                    return false;
                }

                return true;
            }
        }

        void Update()
        {
            if (DamageTrigger == null) return;

            if (playerCorpses.Count > 0)
            {
                for (int i = 0; i < playerCorpses.Count; i++)
                {
                    if (!playerCorpses[i].Update(DamageTrigger))
                    {
                        removeCorpses.Add(playerCorpses[i]);
                    }
                }
                for (int i = 0; i < removeCorpses.Count; i++)
                {
                    playerCorpses.Remove(removeCorpses[i]);
                }
                removeCorpses.Clear();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void DamageTriggerKilledPlayerServerRpc(int playerTarget)
        {
            DamageTriggerKilledClientRpc((int)ColliderType.Player, playerTarget);
        }

        [ServerRpc(RequireOwnership = true)]
        public void DamageTriggerKilledServerRpc(int type)
        {
            DamageTriggerKilledClientRpc(type);
        }

        [ClientRpc]
        private void DamageTriggerKilledClientRpc(int type, int playerTarget = -1)
        {
            if (DamageTrigger != null)
            {
                switch (type)
                {
                    case (int)ColliderType.Player:
                        if (playerTarget >= 0 && playerTarget < RoundManager.Instance.playersManager.allPlayerScripts.Length)
                        {
                            if (RoundManager.Instance.playersManager.allPlayerScripts[playerTarget].isPlayerDead)
                            {
                                playerCorpses.Add(new PlayerCorpse
                                {
                                    player = StartOfRound.Instance.allPlayerScripts[playerTarget],
                                    startTime = Time.realtimeSinceStartup,
                                    stickTime = DamageTrigger.corpseStickTime
                                });
                                PlayerKilled.Invoke();
                            }
                        }
                        break;
                    case (int)ColliderType.Enemy:
                        EnemyKilled.Invoke();
                        break;
                    case (int)ColliderType.Vehicle:
                        VehicleKilled.Invoke();
                        break;
                    default: break;
                }
            }
            else
            {
                JLogHelper.LogWarning($"{name} DamageTriggerNetworking is missing a linked DamageTrigger!");
            }
        }
    }
}
