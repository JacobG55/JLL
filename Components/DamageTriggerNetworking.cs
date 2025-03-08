using GameNetcodeStuff;
using JLL.API;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class DamageTriggerNetworking : NetworkBehaviour
    {
        public DamageTrigger DamageTrigger;

        [Tooltip("Warning!\nThe player will be dead at the time of this event. Be careful what you do when passing a player to another method that you don't break anything.")]
        public InteractEvent PlayerKilled = new InteractEvent();
        public UnityEvent EnemyKilled = new UnityEvent();
        public UnityEvent VehicleKilled = new UnityEvent();

        private readonly List<PlayerCorpse> playerCorpses = new List<PlayerCorpse>();
        private readonly List<PlayerCorpse> removeCorpses = new List<PlayerCorpse>();

        private class PlayerCorpse
        {
            public PlayerControllerB player;
            public float startTime = 0;
            public float stickTime = 0;
            public bool permaStuck = false;
            private bool stuck = false;
            private bool initialized = false;

            public bool Update(DamageTrigger trigger)
            {
                if (permaStuck && initialized) return true;
                if (player.deadBody != null)
                {
                    if (!initialized)
                    {
                        if (trigger.corpseType < 0)
                        {
                            Destroy(player.deadBody.gameObject);
                            player.deadBody = null;
                            return false;
                        }
                        if (permaStuck)
                        {
                            OverrideModel(trigger);
                        }
                        if (trigger.attachCorpseToPoint && trigger.corpseAttachPoint != null)
                        {
                            JLogHelper.LogInfo($"Attatching {player.playerUsername}'s corpse to {trigger.corpseAttachPoint.name}");
                            player.deadBody.matchPositionExactly = trigger.matchPointExactly;
                            player.deadBody.attachedTo = trigger.corpseAttachPoint;
                            player.deadBody.attachedLimb = player.deadBody.bodyParts[(int)trigger.connectedBone];
                            stuck = true;
                        }
                        initialized = true;
                    }
                    if (stickTime > 0)
                    {
                        if (stuck)
                        {
                            stickTime -= Time.deltaTime;
                        }

                        if (stickTime <= 0) DetachCorpse();
                    }
                    else
                    {
                        OverrideModel(trigger);
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

            public void OverrideModel(DamageTrigger trigger)
            {
                if (trigger.OverrideCorpseMesh != null)
                {
                    player.deadBody.ChangeMesh(trigger.OverrideCorpseMesh);
                }
            }

            public void DetachCorpse()
            {
                JLogHelper.LogInfo($"Releasing {player.playerUsername}'s corpse");
                player.deadBody.matchPositionExactly = false;
                player.deadBody.attachedTo = null;
                player.deadBody.attachedLimb = null;
                stuck = false;
                permaStuck = false;
                stickTime = 0;
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
            if (DamageTrigger == null)
            {
                JLogHelper.LogWarning($"{name} DamageTriggerNetworking is missing a linked DamageTrigger!");
                return;
            }
            switch (type)
            {
                case (int)ColliderType.Player:
                    if (playerTarget >= 0 && playerTarget < RoundManager.Instance.playersManager.allPlayerScripts.Length)
                    {
                        PlayerCorpse corpse = new PlayerCorpse
                        {
                            player = StartOfRound.Instance.allPlayerScripts[playerTarget],
                            startTime = Time.realtimeSinceStartup,
                            stickTime = Mathf.Abs(DamageTrigger.corpseStickTime),
                            permaStuck = DamageTrigger.corpseStickTime < 0
                        };
                        playerCorpses.Add(corpse);
                        PlayerKilled.Invoke(corpse.player);
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
    }
}
