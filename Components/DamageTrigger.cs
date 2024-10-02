using GameNetcodeStuff;
using JLL.API;
using JLL.Patches;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static JLL.Components.EnemySpawner;

namespace JLL.Components
{
    public class DamageTrigger : MonoBehaviour
    {
        [SerializeField]
        public CauseOfDeath damageSource = CauseOfDeath.Unknown;
        public Vector3 hitDir = Vector3.zero;
        public RotationType hitRotation = RotationType.ObjectRotation;
        [Tooltip("Check RoundManager in the Ship Scene for corpse IDs\nA negative copseType will result in no corpse spawning")]
        public int corpseType = 0;

        [Header("Conditions")]
        [Tooltip("Damage when something enters a trigger collider")]
        public bool damageOnEnter = true;
        [Tooltip("Damage when something exits a trigger collider")]
        public bool damageOnExit = false;
        [Tooltip("Damage when something collides with a non trigger collider")]
        public bool damageOnCollision = true;

        [Header("Continuous Damage")]
        [Tooltip("Continuously damages anything inside of it's trigger collider")]
        public bool continuousDamage = true;
        [Tooltip("Continuously calculates a raycast and attempts to damage the whatever the ray hits")]
        public bool continuousRaycastDamage = false;
        public float hitInterval = 0.5f;
        private float timer = 0;

        [Header("Raycast")]
        public float raycastLength = 6f;
        public Transform[] raycastDirections = new Transform[0];
        public LayerMask raycastMask = 1202194760;

        [Header("Targets")]
        public float damageMultiplier = 1f;
        [Tooltip("Players have 100 HP")]
        public DamageTarget<PlayerControllerB> playerTargets = new DamageTarget<PlayerControllerB>();
        [Tooltip("Any instance of EnemyAICollisionDetect converted to EnemyAI")]
        public DamageTarget<EnemyAI> enemyTargets = new DamageTarget<EnemyAI>();
        [Tooltip("The Company Cruiser has 30 HP")]
        public DamageTarget<VehicleController> vehicleTargets = new DamageTarget<VehicleController>();
        [Tooltip("Anything that is damageable by shovels but not one of the things above")]
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
            public bool playCustomSounds = true;
            public UnityEvent<T> hitEvent = new UnityEvent<T>();
            public UnityEvent<T> killEvent = new UnityEvent<T>();

            public int GetTotalDamage(float multiplier = 1)
            {
                if (applyDamageMultiplier)
                {
                    return Mathf.RoundToInt(damage * multiplier);
                }
                return damage;
            }
        }

        private readonly Dictionary<GameObject, int> collidersInside = new Dictionary<GameObject, int>();
        private readonly List<GameObject> markedForRemoval = new List<GameObject>();
        private readonly List<GameObject> foundInside = new List<GameObject>();

        public enum ColliderType
        {
            Unknown = -1,
            Player = 0,
            Enemy = 1,
            Vehicle = 2,
            Object = 3
        }

        public void OnTriggerStay(Collider collider)
        {
            foundInside.Add(collider.gameObject);
            if (!collidersInside.ContainsKey(collider.gameObject))
            {
                int type = IdentifyCollider(collider.gameObject);
                collidersInside.Add(collider.gameObject, type);
                if (damageOnEnter) DamageType(collider.gameObject, type);
            }
        }

        public void FixedUpdate()
        {
            foreach (var pair in collidersInside)
            {
                if (!foundInside.Contains(pair.Key))
                {
                    if (damageOnExit) DamageType(pair.Key, pair.Value);
                    markedForRemoval.Add(pair.Key);
                }
            }
            foundInside.Clear();
        }

        /*
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
                markedForRemoval.Add(collider.gameObject);
            }
        }
        */

        public void OnDisable()
        {
            collidersInside.Clear();
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

        void OnDrawGizmos()
        {
            if (raycastLength > 0)
            {
                for (int i = 0; i < raycastDirections.Length; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawRay(transform.position, GetRayDirection(raycastDirections[i]) * raycastLength);
                }
            }
        }

        public void Update()
        {
            for (int i = 0; i < markedForRemoval.Count; i++)
            {
                collidersInside.Remove(markedForRemoval[i]);
            }
            markedForRemoval.Clear();

            if (continuousDamage || continuousRaycastDamage)
            {
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    timer = hitInterval;

                    if (continuousDamage)
                    {
                        DamageAllInside();
                    }
                    if (continuousRaycastDamage)
                    {
                        DamageRaycast();
                    }
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
                    if (playerTargets.enabled) DamagePlayer(target.GetComponent<PlayerControllerB>());
                    break;
                case (int)ColliderType.Enemy:
                    if (enemyTargets.enabled) DamageEnemy(target.GetComponent<EnemyAICollisionDetect>().mainScript);
                    break;
                case (int)ColliderType.Vehicle:
                    if (vehicleTargets.enabled) DamageVehicle(target.GetComponent<VehicleController>());
                    break;
                case (int)ColliderType.Object:
                    if (objectTargets.enabled) DamageObject(target.GetComponent<IHittable>());
                    break;
                default:
                    return;
            }
        }

        public void DamageRaycast()
        {
            if (raycastLength > 0)
            {
                for (int i = 0; i < raycastDirections.Length; i++)
                {
                    if (Physics.Raycast(transform.position, GetRayDirection(raycastDirections[i]), out RaycastHit hit, raycastLength, raycastMask))
                    {
                        int type = IdentifyCollider(hit.collider.gameObject);
                        if (type != -1)
                        {
                            DamageType(hit.collider.gameObject, type);
                        }
                    }
                }
            }
        }

        private Vector3 GetRayDirection(Transform target)
        {
            return (target.position - transform.position).normalized;
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

        private Vector3 CalcHitDir(Transform target)
        {
            switch (hitRotation)
            {
                case RotationType.ObjectRotation:
                    return Quaternion.AngleAxis(transform.rotation.y, Vector3.up) * hitDir;
                case RotationType.RandomRotation:
                    return Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), Vector3.up) * hitDir;
                default:
                    return hitDir;
            }
        }

        public void SetDamageMultiplier(float multiplier)
        {
            damageMultiplier = multiplier;
        }

        public void DamagePlayer(PlayerControllerB player)
        {
            if (player.isPlayerDead)
            {
                markedForRemoval.Add(player.gameObject);
                return;
            }

            player.DamagePlayer(playerTargets.GetTotalDamage(damageMultiplier), causeOfDeath: damageSource, force: CalcHitDir(player.transform), hasDamageSFX: playNormalDamageSFX, deathAnimation: Mathf.Clamp(corpseType, 0, StartOfRound.Instance.playerRagdolls.Count-1));
            playerTargets.hitEvent.Invoke(player);

            if (player.isPlayerDead)
            {
                if (corpseType < 0)
                {
                    JLLNetworkManager.Instance.DestroyPlayerCorpse(player);
                }
                playerTargets.killEvent.Invoke(player);
            }

            if (playerTargets.playCustomSounds)
            {
                PlayCustomAudio();
            }
        }
        public void SetPlayerDamage(int damage)
        {
            playerTargets.damage = damage;
        }

        public void DamageEnemy(EnemyAI enemy)
        {
            if (enemy.isEnemyDead)
            {
                markedForRemoval.Add(enemy.gameObject);
                return;
            }

            enemy.HitEnemyOnLocalClient(enemyTargets.GetTotalDamage(damageMultiplier), CalcHitDir(enemy.transform), playHitSFX: playNormalDamageSFX);
            enemyTargets.hitEvent.Invoke(enemy);

            if (enemy.isEnemyDead)
            {
                enemyTargets.killEvent.Invoke(enemy);
            }

            if (enemyTargets.playCustomSounds)
            {
                PlayCustomAudio();
            }
        }
        public void SetEnemyDamage(int damage)
        {
            enemyTargets.damage = damage;
        }

        public void DamageVehicle(VehicleController vehicle)
        {
            if (vehicle.carDestroyed)
            {
                markedForRemoval.Add(vehicle.gameObject);
                return;
            }

            VehicleControllerPatch.DealPermanentDamage(vehicle, vehicleTargets.GetTotalDamage(damageMultiplier), CalcHitDir(vehicle.transform));
            vehicleTargets.hitEvent.Invoke(vehicle);

            if (vehicle.carDestroyed)
            {
                vehicleTargets.killEvent.Invoke(vehicle);
            }

            if (vehicleTargets.playCustomSounds)
            {
                PlayCustomAudio();
            }
        }
        public void SetVehicleDamage(int damage)
        {
            vehicleTargets.damage = damage;
        }

        public void DamageObject(IHittable obj)
        {
            obj.Hit(objectTargets.GetTotalDamage(damageMultiplier), CalcHitDir(obj is MonoBehaviour behaviour ? behaviour.transform : transform), playHitSFX: playNormalDamageSFX);
            objectTargets.hitEvent.Invoke(obj);

            if (objectTargets.playCustomSounds)
            {
                PlayCustomAudio();
            }
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
