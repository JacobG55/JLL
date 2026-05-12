using GameNetcodeStuff;
using UnityEngine;

namespace JLL.Components
{
    public class JStaminaController : MonoBehaviour
    {
        public float passiveChange = -0.2f;
        public float changeMult = 1f;

        private float lastStamina = 0f;

        public void OnEnable()
        {
            lastStamina = StartOfRound.Instance.localPlayerController.sprintMeter;
        }

        public void Update()
        {
            PlayerControllerB local = StartOfRound.Instance.localPlayerController;
            if (local.isPlayerDead) return;

            foreach (Collider collider in GetComponents<Collider>())
            {
                if (Vector3.Distance(collider.ClosestPoint(local.transform.position), local.transform.position) < 0.1f)
                {
                    float change = local.sprintMeter - lastStamina;
                    local.sprintMeter = Mathf.Clamp(lastStamina + (change * changeMult) + (passiveChange * Time.deltaTime), 0f, 1f);

                    break;
                }
            }

            lastStamina = local.sprintMeter;
        }
    }
}
