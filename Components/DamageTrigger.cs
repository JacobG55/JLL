using GameNetcodeStuff;
using JLL.Patches;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JLL.Components
{
    public class DamageTrigger : MonoBehaviour
    {
        [SerializeField]
        public CauseOfDeath damageSource = CauseOfDeath.Unknown;
        public Vector3 hitDir = Vector3.zero;
        public int corpseType = 0;

        [Header("Conditions")]
        public bool damageOnEnter = true;
        public bool damageOnExit = false;
        public bool damageOnCollision = true;

        [Header("Continuous Damage")]
        public bool continuousDamage = true;
        public float hitInterval = 0.5f;
        private float timer = 0;

        [Header("Targets")]
        public float damageMultiplier = 1f;
        public DamageTarget<PlayerControllerB> playerTargets = new DamageTarget<PlayerControllerB>();
        public DamageTarget<EnemyAI> enemyTargets = new DamageTarget<EnemyAI>();
        public DamageTarget<VehicleController> vehicleTargets = new DamageTarget<VehicleController>();
        public DamageTarget<IHittable> objectTargets = new DamageTarget<IHittable>();

        [Header("SFX")]
        public bool playNormalDamageSFX = true;
        public AudioClip[] clips = new AudioClip[0];
        public AudioSource[] sources = new AudioSource[0];

        [Serializable]
        public class DamageTarget<T>
        {
            public bool enabled = false;
            public int damage = 1;
            public bool applyDamageMultiplier = true;
            public UnityEvent<T> hitEvent = new UnityEvent<T>();
        }

        private readonly Dictionary<GameObject, int> collidersInside = new Dictionary<GameObject, int>();

        public enum ColliderType
        {
            Player = 0,
            Enemy = 1,
            Vehicle = 2,
            Object = 3
        }

        public void OnTriggerEnter(Collider collider)
        {
            int type = IdentifyCollider(collider.gameObject);
            if (type != -1)
            {
                if (damageOnEnter) DamageType(collider.gameObject, type);
                collidersInside.Add(collider.gameObject, type);
            }
        }

        public void OnTriggerExit(Collider collider)
        {
            if (collidersInside.TryGetValue(collider.gameObject, out int type)) {
                if (damageOnExit) DamageType(collider.gameObject, type);
                collidersInside.Remove(collider.gameObject);
            }
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (damageOnCollision)
            {
                int type = IdentifyCollider(collision.gameObject);
                if (type != -1)
                {
                    DamageType(collision.gameObject, type);
                }
            }
        }

        public void Update()
        {
            if (continuousDamage)
            {
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    timer = hitInterval;
                    DamageAllInside();
                }
            }
        }

        public void DamageAllInside()
        {
            foreach (var hit in collidersInside)
            {
                DamageType(hit.Key, hit.Value);
            }
        }

        private void DamageType(GameObject target, int type)
        {
            switch (type)
            {
                case (int)ColliderType.Player:
                    DamagePlayer(target.GetComponent<PlayerControllerB>());
                    break;
                case (int)ColliderType.Enemy:
                    DamageEnemy(target.GetComponent<EnemyAICollisionDetect>().mainScript);
                    break;
                case (int)ColliderType.Vehicle:
                    DamageVehicle(target.GetComponent<VehicleController>());
                    break;
                case (int)ColliderType.Object:
                    DamageObject(target.GetComponent<IHittable>());
                    break;
                default:
                    return;
            }

            PlayCustomAudio();
        }

        public static int IdentifyCollider(GameObject target)
        {
            if (target.CompareTag("Player"))
            {
                if (target.TryGetComponent(out PlayerControllerB player))
                {
                    return (int)ColliderType.Player;
                }
            }
            else if (target.CompareTag("Enemy"))
            {
                if (target.TryGetComponent(out EnemyAICollisionDetect enemy))
                {
                    return (int)ColliderType.Enemy;
                }
            }
            else if (target.TryGetComponent(out VehicleController vehicle))
            {
                return (int)ColliderType.Vehicle;
            }
            else if (target.TryGetComponent(out IHittable hittable))
            {
                return (int)ColliderType.Object;
            }

            return -1;
        }

        public void SetDamageMultiplier(float multiplier)
        {
            damageMultiplier = multiplier;
        }

        public void DamagePlayer(PlayerControllerB player)
        {
            int damage = playerTargets.damage;
            if (playerTargets.applyDamageMultiplier)
            {
                damage = Mathf.RoundToInt(damage * damageMultiplier);
            }
            player.DamagePlayer(damage, causeOfDeath: damageSource, force: hitDir, hasDamageSFX: playNormalDamageSFX, deathAnimation: Mathf.Clamp(corpseType, 0, StartOfRound.Instance.playerRagdolls.Count));
            playerTargets.hitEvent.Invoke(player);
        }
        public void SetPlayerDamage(int damage)
        {
            playerTargets.damage = damage;
        }

        public void DamageEnemy(EnemyAI enemy)
        {
            int damage = enemyTargets.damage;
            if (enemyTargets.applyDamageMultiplier)
            {
                damage = Mathf.RoundToInt(damage * damageMultiplier);
            }
            enemy.HitEnemyOnLocalClient(damage, hitDir, playHitSFX: playNormalDamageSFX);
            enemyTargets.hitEvent.Invoke(enemy);
        }
        public void SetEnemyDamage(int damage)
        {
            enemyTargets.damage = damage;
        }

        public void DamageVehicle(VehicleController vehicle)
        {
            int damage = vehicleTargets.damage;
            if (vehicleTargets.applyDamageMultiplier)
            {
                damage = Mathf.RoundToInt(damage * damageMultiplier);
            }
            VehicleControllerPatch.DealPermanentDamage(vehicle, damage, hitDir);
            vehicleTargets.hitEvent.Invoke(vehicle);
        }
        public void SetVehicleDamage(int damage)
        {
            vehicleTargets.damage = damage;
        }

        public void DamageObject(IHittable obj)
        {
            int damage = objectTargets.damage;
            if (objectTargets.applyDamageMultiplier)
            {
                damage = Mathf.RoundToInt(damage * damageMultiplier);
            }
            obj.Hit(damage, hitDir, playHitSFX: playNormalDamageSFX);
            objectTargets.hitEvent.Invoke(obj);
        }
        public void SetObjectDamage(int damage)
        {
            objectTargets.damage = damage;
        }


        private void PlayCustomAudio()
        {
            if (clips.Length > 0 && sources.Length > 0)
            {
                AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];
                foreach (AudioSource source in sources)
                {
                    source.clip = clip;
                    source.Play();
                }
            }
        }
    }
}
