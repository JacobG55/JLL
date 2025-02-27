using GameNetcodeStuff;
using UnityEngine;

namespace JLL.Components
{
    public class JBillboard : MonoBehaviour
    {
        public bool Active = true;
        public PlayerTarget Target = PlayerTarget.Local;
        public bool LerpRot = false;
        public float LerpSpeed = 1.0f;
        public bool X_Axis = false;
        public bool Y_Axis = true;
        public bool Z_Axis = false;

        public void Update()
        {
            if (!Active) return;

            Transform target = GetPlayer(Target, transform.position).gameplayCamera.transform;

            Vector3 lookEuler = Quaternion.LookRotation((target.transform.position - transform.position).normalized, Vector3.up).eulerAngles;
            Quaternion axisLocked = Quaternion.Euler(X_Axis ? lookEuler.x : 0, Y_Axis ? lookEuler.y : 0, Z_Axis ? lookEuler.z : 0);
            transform.rotation = LerpRot ? Quaternion.Slerp(transform.rotation, axisLocked, Time.deltaTime * LerpSpeed) : axisLocked;
        }

        public static PlayerControllerB GetPlayer(PlayerTarget target, Vector3 refrencePos)
        {
            if (target == PlayerTarget.Local) return StartOfRound.Instance.localPlayerController;

            PlayerControllerB refrencePlayer = RoundManager.Instance.playersManager.allPlayerScripts[0];
            float refrenceDistance = Vector3.Distance(RoundManager.Instance.playersManager.allPlayerScripts[0].transform.position, refrencePos);

            switch (target)
            {
                case PlayerTarget.Closest:
                    for (int i = 1; i < RoundManager.Instance.playersManager.allPlayerScripts.Length; i++)
                    {
                        PlayerControllerB player = RoundManager.Instance.playersManager.allPlayerScripts[i];
                        if (player.isPlayerDead || player.disconnectedMidGame) continue;
                        float distance = Vector3.Distance(player.transform.position, refrencePos);
                        if (distance < refrenceDistance)
                        {
                            refrenceDistance = distance;
                            refrencePlayer = player;
                        }
                    }
                    return refrencePlayer;
                case PlayerTarget.Farthest:
                    for (int i = 1; i < RoundManager.Instance.playersManager.allPlayerScripts.Length; i++)
                    {
                        PlayerControllerB player = RoundManager.Instance.playersManager.allPlayerScripts[i];
                        if (player.isPlayerDead || player.disconnectedMidGame) continue;
                        float distance = Vector3.Distance(player.transform.position, refrencePos);
                        if (distance > refrenceDistance)
                        {
                            refrenceDistance = distance;
                            refrencePlayer = player;
                        }
                    }
                    return refrencePlayer;
                default: return StartOfRound.Instance.localPlayerController;
            }
        }
    }
}
