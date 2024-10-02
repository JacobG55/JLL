using GameNetcodeStuff;
using JLL.API.Events;
using UnityEngine;

namespace JLL.Components.Helpers
{
    public class ObjectConverter : MonoBehaviour
    {
        public ObjectEvent OutputObject = new ObjectEvent();

        public void AsGameObject(MonoBehaviour behaviour)
        {
            OutputObject.Invoke(behaviour.gameObject);
        }

        public PlayerEvent OutputPlayer = new PlayerEvent();

        public void AsPlayer(GameObject obj)
        {
            if (obj.TryGetComponent(out PlayerControllerB player))
            {
                OutputPlayer.Invoke(player);
            }
        }

        public EnemyEvent OutputEnemyAI = new EnemyEvent();

        public void AsEnemy(GameObject obj)
        {
            if (obj.TryGetComponent(out EnemyAI enemy))
            {
                OutputEnemyAI.Invoke(enemy);
            }
            else if (obj.TryGetComponent(out EnemyAICollisionDetect enemyCollider))
            {
                OutputEnemyAI.Invoke(enemyCollider.mainScript);
            }
        }

        public VehicleEvent OutputVehicle = new VehicleEvent();

        public void AsVehicle(GameObject obj)
        {
            if (obj.TryGetComponent(out VehicleController cruiser))
            {
                OutputVehicle.Invoke(cruiser);
            }
        }
    }
}
