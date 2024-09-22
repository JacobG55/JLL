using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace JLL.Components
{
    public class ExplosiveEmitter : MonoBehaviour
    {
        [FormerlySerializedAs("explodeOnAwake")]
        public bool explodeOnEnabled = false;
        [Header("Explosion Size")]
        public float killDistance = 5.7f;
        public float damageRange = 6f;

        [Header("Damage Properties")]
        public int nonLethalDamage = 50;
        public float pushForce = 0;
        public bool goThroughCar = false;

        public GameObject? overrideEffect = null;
        public bool spawnParticles = true;

        [Header("Screen Shake Emmision")]
        [FormerlySerializedAs("shakeOnAwake")]
        public bool shakeOnEnabled = false;
        [Tooltip("Bridge Destroyed Default: 30")]
        public float strongDistance = -1f;
        [Tooltip("Bridge Destroyed Default: 50")]
        public float longDistance = -1f;
        [Tooltip("Landmine Default: 14")]
        public float bigDistance = 14f;
        [Tooltip("Landmine Default = 25")]
        public float smallDistance = 25f;

        [Header("Stun Explosion")]
        [FormerlySerializedAs("stunOnAwake")]
        public bool stunOnEnabled = false;
        public bool affectAudio = true;
        public float flashSeverityMultiplier = 1f;
        public float enemyStunTime = 7.5f;
        public float flashSeverityDistanceRolloff = 1f;

        public void OnEnabled()
        {
            if (explodeOnEnabled)
            {
                Explode();
            }
            if (shakeOnEnabled)
            {
                DistanceShakeScreen();
            }
            if (stunOnEnabled)
            {
                StunFlash();
            }
        }

        public void Explode()
        {
            Explode(transform.position);
        }

        public void Explode(MonoBehaviour target)
        {
            Explode(target.gameObject);
        }

        public void Explode(GameObject target)
        {
            Explode(target.transform.position);
        }

        public void Explode(Vector3 position)
        {
            Landmine.SpawnExplosion(position, spawnParticles, killDistance, damageRange, nonLethalDamage, pushForce, overrideEffect, goThroughCar);
        }

        void OnDrawGizmosSelected()
        {
            if (killDistance > 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, killDistance);
            }
            if (damageRange > 0)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, damageRange);
            }

            if (strongDistance > 0)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawWireSphere(transform.position, strongDistance);
            }
            if (longDistance > 0)
            {
                Gizmos.color = Color.Lerp(Color.black, Color.gray, 0.5f);
                Gizmos.DrawWireSphere(transform.position, longDistance);
            }
            if (bigDistance > 0)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawWireSphere(transform.position, bigDistance);
            }
            if (smallDistance > 0)
            {
                Gizmos.color = Color.Lerp(Color.gray, Color.white, 0.5f);
                Gizmos.DrawWireSphere(transform.position, smallDistance);
            }
        }

        private float GetLocalPlayerDistance()
        {
            return Vector3.Distance(GameNetworkManager.Instance.localPlayerController.transform.position, transform.position);
        }

        public void DistanceShakeScreen()
        {
            float num = GetLocalPlayerDistance();
            if (strongDistance > 0 && num < strongDistance)
            {
                HUDManager.Instance.ShakeCamera(ScreenShakeType.VeryStrong);
            }
            else if (longDistance > 0 && num < longDistance)
            {
                HUDManager.Instance.ShakeCamera(ScreenShakeType.Long);
            }
            else if (bigDistance > 0 && num < bigDistance)
            {
                HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
            }
            else if (smallDistance > 0 && num < smallDistance)
            {
                HUDManager.Instance.ShakeCamera(ScreenShakeType.Small);
            }
        }

        public void ShakeLocalClient(int intensity)
        {
            if (Enum.IsDefined(typeof(ScreenShakeType), intensity))
            {
                HUDManager.Instance.ShakeCamera((ScreenShakeType)intensity);
            }
        }

        public void StunFlash()
        {
            StunFlash(transform.position);
        }

        public void StunFlash(MonoBehaviour target)
        {
            StunFlash(target.gameObject);
        }

        public void StunFlash(GameObject target)
        {
            StunFlash(target.transform.position);
        }

        public void StunFlash(Vector3 position)
        {
            StunGrenadeItem.StunExplosion(position, affectAudio, flashSeverityMultiplier, enemyStunTime, flashSeverityDistanceRolloff);
        }
    }
}
