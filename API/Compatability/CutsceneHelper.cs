using LCCutscene;
using System;
using UnityEngine;

namespace JLL.API.Compatability
{
    public class CutsceneHelper
    {
        public static void StartCutscene(Transform cameraTrans, float length, bool allowDeath = true, bool allowMovement = false, float startTransition = 1, float endTransition = 1)
        {
            if (JLL.disableCutscenes.Value)
            {
                return;
            }
            try
            {
                CutsceneManager.Instance.PlayScene(new Cutscene(cameraTrans, length)
                {
                    AllowPlayerDeath = allowDeath,
                    AllowPlayerMovement = allowMovement,
                    TransitionInSpeed = startTransition,
                    TransitionOutSpeed = endTransition
                });
            }
            catch (Exception e)
            {
                JLogHelper.LogWarning($"LCCutscene failed to start.\n{e}", JLogLevel.Debuging);
            }
        }
    }
}
