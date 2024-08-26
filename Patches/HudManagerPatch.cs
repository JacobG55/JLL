using GameNetcodeStuff;
using HarmonyLib;
using JLL.API;
using JLL.Components;
using System.Collections;
using UnityEngine;

namespace JLL.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    public class HudManagerPatch
    {
        public static JWaterFilter? customFilter = null;

        [HarmonyPatch("UnderwaterScreenFilters")]
        [HarmonyPrefix]
        public static bool patchUnderwaterScreenFilters(HUDManager __instance)
        {
            PlayerControllerB playerScript = __instance.localPlayer;

            if (__instance.localPlayer != null)
            {
                bool flag = false;

                PlayerControllerB spectatedPlayer = playerScript.spectatedPlayerScript;
                if (playerScript.isPlayerDead && spectatedPlayer != null && spectatedPlayer.underwaterCollider != null && spectatedPlayer.underwaterCollider.bounds.Contains(StartOfRound.Instance.spectateCamera.transform.position))
                {
                    flag = true;
                    spectatedPlayer.underwaterCollider.TryGetComponent(out customFilter);
                }

                if (customFilter == null && !playerScript.isPlayerDead)
                {
                    if (playerScript.underwaterCollider != null)
                    {
                        playerScript.underwaterCollider.TryGetComponent(out customFilter);
                    }
                }

                if (customFilter != null)
                {
                    if (__instance.setUnderwaterFilter || flag)
                    {
                        customFilter.UnderwaterFilters(__instance, flag);
                        __instance.breathingUnderwaterAudio.volume = Mathf.Lerp(__instance.breathingUnderwaterAudio.volume, 1f, 10f * Time.deltaTime);
                        if (customFilter.customUnderwaterSounds != null)
                        {
                            customFilter.customUnderwaterSounds.volume = __instance.breathingUnderwaterAudio.volume;
                        }
                    }
                    else
                    {
                        if (__instance.audioListenerLowPass.cutoffFrequency >= 19000f)
                        {
                            __instance.audioListenerLowPass.enabled = false;
                        }
                        else
                        {
                            __instance.audioListenerLowPass.cutoffFrequency = Mathf.Lerp(__instance.audioListenerLowPass.cutoffFrequency, 20000f, 10f * Time.deltaTime);
                        }
                        if (customFilter.underwaterFilter.weight < 0.05f)
                        {
                            customFilter.underwaterFilter.weight = 0f;
                            __instance.breathingUnderwaterAudio.Stop();
                            if (customFilter.customUnderwaterSounds != null)
                            {
                                customFilter.customUnderwaterSounds.Stop();
                            }
                            customFilter = null;
                        }
                        else
                        {
                            __instance.breathingUnderwaterAudio.volume = Mathf.Lerp(__instance.breathingUnderwaterAudio.volume, 0f, 10f * Time.deltaTime);
                            if (customFilter.customUnderwaterSounds != null)
                            {
                                customFilter.customUnderwaterSounds.volume = __instance.breathingUnderwaterAudio.volume;
                            }
                            customFilter.underwaterFilter.weight = Mathf.Lerp(customFilter.underwaterFilter.weight, 0f, 10f * Time.deltaTime);
                        }
                    }

                    return false;
                }
            }

            return true;
        }

        [HarmonyPatch("DisplayTip")]
        [HarmonyPrefix]
        public static void patchDisplayTip(HUDManager __instance)
        {
            JHudHelper.isTipActive = true;
            __instance.StartCoroutine(TipQueueCheck());
        }

        private static IEnumerator TipQueueCheck()
        {
            yield return new WaitForSeconds(5f);
            JHudHelper.isTipActive = false;
            JHudHelper.DisplayNextTip();
        }
    }
}
