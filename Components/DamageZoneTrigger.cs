using UnityEngine;
using GameNetcodeStuff;
using JLL.Patches;
using System.Collections.Generic;

namespace JLL.Components
{
    public class DamageZoneTrigger : MonoBehaviour
    {
        [Header("-=-DEPRICATED-=-")]
        [SerializeField]
        public CauseOfDeath damageSource = CauseOfDeath.Unknown;
        public Vector3 hitDir = Vector3.zero;
        public int corpseType = 0;

        [Header("Conditions")]
        public bool damageOnEnter = true;
        public bool damageOnExit = false;
        public bool damageOnCollision = true;

        [Header("Damage")]
        public bool damagePlayers = true;
        public int damageForPlayers = 1;
        public bool damageEnemies = true;
        public int damageForEnemies = 1;
        public bool damageVehicles = false;
        public int damageForVehicles = 1;
        public bool damageObjects = false;
        public int damageForObjects = 1;

        [Header("Continuous Damage")]
        public bool continuousDamage = true;
        public float hitInterval = 0.5f;
        private float timer = 0;

        [Header("SFX")]
        public bool playNormalDamageSFX = true;
        public AudioClip[] clips = new AudioClip[0];
        public AudioSource[] sources = new AudioSource[0];

        [Header("Player")]
        public InteractEvent OnPlayerDamaged = new InteractEvent();

        private List<PlayerControllerB> playersInside = new List<PlayerControllerB>();
        private List<EnemyAI> creaturesInside = new List<EnemyAI>();
        private List<VehicleController> vehiclesInside = new List<VehicleController>();
        private List<IHittable> objectsInside = new List<IHittable>();

        public void OnTriggerEnter(Collider collider)
        {
            int type = DamageTarget(collider.gameObject, damageOnEnter);
            switch (type)
            {
                case 0:
                    playersInside.Add(collider.GetComponent<PlayerControllerB>());
                    break;
                case 1:
                    creaturesInside.Add(collider.GetComponent<EnemyAICollisionDetect>().mainScript);
                    break;
                case 2:
                    vehiclesInside.Add(collider.GetComponent<VehicleController>());
                    break;
                case 3:
                    objectsInside.Add(collider.GetComponent<IHittable>());
                    break;
            }
        }

        public void OnTriggerExit(Collider collider)
        {
            int type = DamageTarget(collider.gameObject, damageOnExit);
            switch (type)
            {
                case 0:
                    playersInside.Remove(collider.GetComponent<PlayerControllerB>());
                    break;
                case 1:
                    creaturesInside.Remove(collider.GetComponent<EnemyAICollisionDetect>().mainScript);
                    break;
                case 2:
                    vehiclesInside.Remove(collider.GetComponent<VehicleController>());
                    break;
                case 3:
                    objectsInside.Remove(collider.GetComponent<IHittable>());
                    break;
            }
        }

        public void OnCollisionEnter(Collision collision)
        {
            DamageTarget(collision.gameObject, damageOnCollision);
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
            if (damagePlayers)
            {
                for (int i = 0; i < playersInside.Count; i++)
                {
                    if (!playersInside[i].isPlayerDead)
                    {
                        DamagePlayer(playersInside[i]);
                    }
                }
            }
            if (damageEnemies)
            {
                for (int i = 0; i < creaturesInside.Count; i++)
                {
                    if (!creaturesInside[i].isEnemyDead)
                    {
                        DamageEnemy(creaturesInside[i]);
                    }
                }
            }
            if (damageVehicles)
            {
                for (int i = 0; i < vehiclesInside.Count; i++)
                {
                    DamageVehicle(vehiclesInside[i]);
                }
            }
            if (damageObjects)
            {
                for (int i = 0; i < objectsInside.Count; i++)
                {
                    DamageObject(objectsInside[i]);
                }
            }
        }

        private int DamageTarget(GameObject target, bool condition = true)
        {
            if (target.CompareTag("Player"))
            {
                if (target.TryGetComponent(out PlayerControllerB player))
                {
                    if (condition && damagePlayers && !player.isPlayerDead)
                    {
                        DamagePlayer(player);
                    }
                    return 0;
                }
            }
            else if (target.CompareTag("Enemy"))
            {
                if (target.TryGetComponent(out EnemyAICollisionDetect enemy))
                {
                    if (condition && damageEnemies && !enemy.mainScript.isEnemyDead)
                    {
                        DamageEnemy(enemy.mainScript);
                    }
                    return 1;
                }
            }
            else if (target.TryGetComponent(out VehicleController vehicle))
            {
                if (condition && damageVehicles && !vehicle.carDestroyed)
                {
                    DamageVehicle(vehicle);
                }
                return 2;
            }
            else if (target.TryGetComponent(out IHittable hittable))
            {
                if (condition && damageObjects)
                {
                    DamageObject(hittable);
                }
                return 3;
            }
            return -1;
        }

        public void DamagePlayer(PlayerControllerB player, bool playCustomAudio = true)
        {
            player.DamagePlayer(damageForPlayers, causeOfDeath: damageSource, force: hitDir, hasDamageSFX: playNormalDamageSFX, deathAnimation: Mathf.Clamp(corpseType, 0, StartOfRound.Instance.playerRagdolls.Count));
            if (playCustomAudio) PlayCustomAudio();
            OnPlayerDamaged.Invoke(player);
        }
        public void DamageEnemy(EnemyAI enemy, bool playCustomAudio = true)
        {
            enemy.HitEnemyOnLocalClient(damageForEnemies, hitDir, playHitSFX: playNormalDamageSFX);
            if (playCustomAudio) PlayCustomAudio();
        }
        public void DamageVehicle(VehicleController vehicle, bool playCustomAudio = true)
        {
            VehicleControllerPatch.DealPermanentDamage(vehicle, damageForVehicles, hitDir);
            if (playCustomAudio) PlayCustomAudio();
        }
        public void DamageObject(IHittable obj, bool playCustomAudio = true)
        {
            obj.Hit(damageForObjects, hitDir, playHitSFX: playNormalDamageSFX);
            if (playCustomAudio) PlayCustomAudio();
        }

        private void PlayCustomAudio()
        {
            if (clips.Length > 0 && sources.Length > 0)
            {
                AudioClip clip = clips[Random.Range(0, clips.Length)];
                foreach (AudioSource source in sources)
                {
                    source.clip = clip;
                    source.Play();
                }
            }
        }
    }
}
