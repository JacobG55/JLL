using JLL.Components;
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
        public void BreakTerrainObstacleServerRpc(Vector3 pos, int damage, int playerWhoSent)
        {
            BreakTerrainObstacleClientRpc(pos, damage, playerWhoSent);
        }

        [ClientRpc]
        public void BreakTerrainObstacleClientRpc(Vector3 pos, int damage, int playerWhoSent)
        {
            if ((int)GameNetworkManager.Instance.localPlayerController.playerClientId != playerWhoSent)
            {
                DestroyTerrainObstacleAtPosition(pos, damage);
            }
        }
    }
}
