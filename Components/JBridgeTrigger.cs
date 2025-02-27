using GameNetcodeStuff;
using JLL.API;
using JLL.API.Compatability;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace JLL.Components
{
    public class JBridgeTrigger : NetworkBehaviour
    {
        public float bridgeDurability = 1f;

        public float bridgeMaxRegen = 1f;

        public float bridgeRegenerationRate = 0.2f;

        private readonly List<PlayerControllerB> playersOnBridge = new List<PlayerControllerB>();
        private readonly List<VehicleController> cruisersOnBridge = new List<VehicleController>();

        public AudioSource bridgeAudioSource;

        public AudioClip[] bridgeCreakSFX;

        public AudioClip bridgeFallSFX;

        public Animator bridgeAnimator;

        private bool hasBridgeFallen;

        public Transform bridgePhysicsPartsContainer;

        private bool giantOnBridge;

        private bool giantOnBridgeLastFrame;

        public Collider[] fallenBridgeColliders;

        public string fallType = "Fall";

        public float weightCapacityAmount = 0.04f;

        public float playerCapacityAmount = 0.02f;

        public float cruiserCapacityAmount = 0.04f;

        private readonly bool LLLPresent = JCompatabilityHelper.IsLoaded(JCompatabilityHelper.CachedMods.LethalLevelLoader);

        private void OnEnable()
        {
            StartOfRound.Instance.playerTeleportedEvent.AddListener(RemovePlayerFromBridge);
        }

        private void OnDisable()
        {
            StartOfRound.Instance.playerTeleportedEvent.RemoveListener(RemovePlayerFromBridge);
        }

        private void Update()
        {
            if (hasBridgeFallen)
            {
                return;
            }
            if (giantOnBridge)
            {
                bridgeDurability -= Time.deltaTime / 4.25f;
            }
            if (playersOnBridge.Count > 0)
            {
                bridgeDurability -= Time.deltaTime * (playerCapacityAmount * (float)(playersOnBridge.Count * playersOnBridge.Count));
                foreach (PlayerControllerB player in playersOnBridge)
                {
                    if (player.carryWeight > 1.1f)
                    {
                        bridgeDurability -= Time.deltaTime * (weightCapacityAmount * player.carryWeight);
                    }
                }
            }

            if (cruisersOnBridge.Count > 0)
            {
                bridgeDurability -= Time.deltaTime * (cruiserCapacityAmount * (float)(cruisersOnBridge.Count * cruisersOnBridge.Count));
                foreach (VehicleController vehicle in cruisersOnBridge)
                {
                    float totalWeights = 0;
                    foreach (var item in vehicle.physicsRegion.physicsTransform.GetComponentsInChildren<GrabbableObject>())
                    {
                        totalWeights += item.itemProperties.weight;
                    }
                    bridgeDurability -= Time.deltaTime * (weightCapacityAmount * totalWeights);
                }
            }

            if (playersOnBridge.Count + cruisersOnBridge.Count == 0 && !giantOnBridge && bridgeDurability < bridgeMaxRegen)
            {
                bridgeDurability = Mathf.Min(bridgeDurability + Time.deltaTime * bridgeRegenerationRate, bridgeMaxRegen);
            }
            if (IsServer && bridgeDurability <= 0f && !hasBridgeFallen)
            {
                hasBridgeFallen = true;
                BridgeFallServerRpc();
                Debug.Log("Bridge collapsed! On server");
            }
            if (bridgeMaxRegen > 0) bridgeAnimator.SetFloat("durability", bridgeDurability > bridgeMaxRegen ? 0 : Mathf.Clamp(Mathf.Abs(bridgeDurability - bridgeMaxRegen) / bridgeMaxRegen, 0f, 1f));
        }

        private void LateUpdate()
        {
            if (giantOnBridge)
            {
                if (giantOnBridgeLastFrame)
                {
                    giantOnBridge = false;
                    giantOnBridgeLastFrame = false;
                }
                else
                {
                    giantOnBridgeLastFrame = true;
                }
            }
        }

        [ServerRpc]
        public void BridgeFallServerRpc() => BridgeFallClientRpc();

        [ClientRpc]
        public void BridgeFallClientRpc()
        {
            hasBridgeFallen = true;
            bridgeAnimator.SetTrigger(fallType);
            EnableFallenBridgeColliders();
            bridgeAudioSource.PlayOneShot(bridgeFallSFX);
            float num = Vector3.Distance(GameNetworkManager.Instance.localPlayerController.transform.position, bridgeAudioSource.transform.position);
            if (num < 30f)
            {
                HUDManager.Instance.ShakeCamera(ScreenShakeType.VeryStrong);
            }
            else if (num < 50f)
            {
                HUDManager.Instance.ShakeCamera(ScreenShakeType.Long);
            }
        }

        private void EnableFallenBridgeColliders()
        {
            for (int i = 0; i < fallenBridgeColliders.Length; i++)
            {
                fallenBridgeColliders[i].enabled = true;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (other.TryGetComponent(out PlayerControllerB player))
                {
                    if (!playersOnBridge.Contains(player))
                    {
                        playersOnBridge.Add(player);
                        if (Random.Range(Mathf.Min(playersOnBridge.Count * 25, 90), 100) > 60)
                        {
                            RoundManager.PlayRandomClip(bridgeAudioSource, bridgeCreakSFX);
                        }
                    }
                }
            }
            else if (other.gameObject.CompareTag("Enemy"))
            {
                if (other.TryGetComponent(out EnemyAICollisionDetect collisionDetect) && (collisionDetect.mainScript.enemyType.enemyName == "ForestGiant" || (LLLPresent && LLLHelper.EnemyHasTag(collisionDetect.mainScript.enemyType, "Heavy"))))
                {
                    giantOnBridge = true;
                    giantOnBridgeLastFrame = false;
                }
            }
            else if (other.gameObject.TryGetComponent(out VehicleController vehicleController))
            {
                if (!cruisersOnBridge.Contains(vehicleController))
                {
                    cruisersOnBridge.Add(vehicleController);
                    if (Random.Range(Mathf.Min(cruisersOnBridge.Count * 40, 90), 100) > 60)
                    {
                        RoundManager.PlayRandomClip(bridgeAudioSource, bridgeCreakSFX);
                    }
                }
            }
        }

        public void RemovePlayerFromBridge(PlayerControllerB playerOnBridge)
        {
            if (playerOnBridge != null && playersOnBridge.Contains(playerOnBridge)) playersOnBridge.Remove(playerOnBridge);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && other.TryGetComponent(out PlayerControllerB player))
            {
                RemovePlayerFromBridge(player);
            }
            else if (other.TryGetComponent(out VehicleController vehicle))
            {
                if (cruisersOnBridge.Contains(vehicle)) cruisersOnBridge.Remove(vehicle);
            }
        }
    }
}
