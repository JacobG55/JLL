﻿using JLL.API;
using UnityEngine;

namespace JLL.Components
{
    public class TerrainObstacle : MonoBehaviour
    {
        public int health = 5;
        public float damageThreshold = 5f;

        [Header("FX")]
        public GameObject breakFX;
        private GameObject? oldBreakFX;
        public AudioClip[] clips = new AudioClip[0];

        [Header("Collision Types")]
        public bool detectTriggers = true;
        public bool detectColliders = true;

        public void OnTriggerEnter(Collider other)
        {
            if (detectTriggers) CalcCollision(other);
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (detectColliders) CalcCollision(collision.collider);
        }

        public void CalcCollision(Collider detected)
        {
            if (detected.TryGetComponent(out VehicleController component))
            {
                float damage = component.averageVelocity.magnitude;
                if (!(component == null) && component.IsOwner && damage > damageThreshold && Vector3.Angle(component.averageVelocity, transform.position - component.mainRigidbody.position) < 80f)
                {
                    JLLNetworkManager.Instance.DestroyTerrainObstacleOnLocalClient(transform.position, Mathf.RoundToInt(damage));
                    component.CarReactToObstacle(component.mainRigidbody.position - transform.position, transform.position, Vector3.zero, CarObstacleType.Object);
                }
            }
        }

        public void Damage(int ammount)
        {
            health -= ammount;

            if (breakFX != null)
            {
                if (oldBreakFX != null)
                {
                    Destroy(oldBreakFX);
                }
                if ((oldBreakFX = Instantiate(breakFX, gameObject.transform.position + Vector3.up, Quaternion.identity)).TryGetComponent(out AudioSource source))
                {
                    if (clips.Length > 0)
                    {
                        source.clip = clips[UnityEngine.Random.Range(0, clips.Length - 1)];
                    }
                    source.Play();
                }
            }

            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}