using UnityEngine;

namespace JLL.Components
{
    public class JClientAttatchedObject : MonoBehaviour
    {
        public GameObject target;

        public ActiveCondition enableCondition = ActiveCondition.ActiveOutdoors;

        [Header("Player Attatchment")]
        [Tooltip("When disabled the player will not attach to the target and isntead just enable / disable it's self based on the Enable Condition.")]
        public bool attachToLocalPlayer = false;
        public bool lerpPosition = false;

        public enum ActiveCondition
        {
            None,
            ActiveOutdoors,
            ActiveIndoors,
            ActiveOutdoorsOutsideShip,
        }

        public void Update()
        {
            if (target == null) return;

            switch (enableCondition)
            {
                case ActiveCondition.ActiveOutdoors:
                    target.SetActive(!RoundManager.Instance.playersManager.localPlayerController.isInsideFactory);
                    break; 
                case ActiveCondition.ActiveIndoors:
                    target.SetActive(RoundManager.Instance.playersManager.localPlayerController.isInsideFactory);
                    break;
                case ActiveCondition.ActiveOutdoorsOutsideShip:
                    target.SetActive(!RoundManager.Instance.playersManager.localPlayerController.isInsideFactory && !RoundManager.Instance.playersManager.localPlayerController.isInHangarShipRoom);
                    break;
                default:
                    break;
            }

            if (target.activeSelf && attachToLocalPlayer)
            {
                Vector3 playerPos = RoundManager.Instance.playersManager.localPlayerController.transform.position;

                if (lerpPosition)
                {
                    target.transform.position = Vector3.Lerp(target.transform.position, playerPos, Time.deltaTime);
                }
                else
                {
                    target.transform.position = playerPos;
                }
            }
        }
    }
}
