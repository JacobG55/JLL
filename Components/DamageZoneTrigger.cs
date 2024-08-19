using GameNetcodeStuff;
using System.Collections.Generic;
using UnityEngine;

namespace JLL.Components
{
    public class DamageZoneTrigger : MonoBehaviour
    {
        [SerializeField]
        public CauseOfDeath damageSource = CauseOfDeath.Unknown;
        public Vector3 hitDir = Vector3.zero;
        public int corpseType = 0;

        [Header("Conditions")]
        public bool damageOnEnter = true;
        public bool damageOnExit = false;

        [Header("Damage")]
        public bool damagePlayers = true;
        public int damageForPlayers = 1;
        public bool damageEnemies = true;
        public int damageForEnemies = 1;
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

        private List<PlayerControllerB> playersInside = new List<PlayerControllerB>();
        private List<IHittable> creaturesInside = new List<IHittable>();
        private List<IHittable> objectsInside = new List<IHittable>();

        public void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.CompareTag("Player")) 
            {
                if (collider.TryGetComponent(out PlayerControllerB player))
                {
                    playersInside.Add(player);
                    if (damageOnEnter && damagePlayers)
                    {
                        DamagePlayer(player);
                    }
                }
            }
            else if (collider.gameObject.CompareTag("Enemy"))
            {
                if (collider.TryGetComponent(out EnemyAICollisionDetect enemy))
                {
                    creaturesInside.Add(enemy);
                    if (damageOnEnter && damageEnemies)
                    {
                        DamageEnemy(enemy);
                    }
                }
            }
            else if (collider.gameObject.TryGetComponent(out IHittable hittable))
            {
                objectsInside.Add(hittable);
                if (damageOnEnter && damageObjects)
                {
                    DamageObject(hittable);
                }
            }
        }

        public void OnTriggerExit(Collider collider)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                if (collider.TryGetComponent(out PlayerControllerB player))
                {
                    playersInside.Remove(player);
                    if (damageOnExit && damagePlayers)
                    {
                        DamagePlayer(player);
                    }
                }
            }
            else if (collider.gameObject.CompareTag("Enemy"))
            {
                if (collider.TryGetComponent(out EnemyAICollisionDetect enemy))
                {
                    creaturesInside.Remove(enemy);
                    if (damageOnExit && damageEnemies)
                    {
                        DamageEnemy(enemy);
                    }
                }
            }
            else if (collider.gameObject.TryGetComponent(out IHittable hittable))
            {
                objectsInside.Add(hittable);
                if (damageOnExit && damageObjects)
                {
                    DamageObject(hittable);
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

                    if (damagePlayers)
                    {
                        for (int i = 0; i < playersInside.Count; i++)
                        {
                            DamagePlayer(playersInside[i]);
                        }
                    }
                    if (damageEnemies)
                    {
                        for (int i = 0; i < creaturesInside.Count; i++)
                        {
                            DamageEnemy(creaturesInside[i]);
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
            }
        }

        private void DamagePlayer(PlayerControllerB player)
        {
            player.DamagePlayer(damageForPlayers, causeOfDeath: damageSource, force: hitDir, hasDamageSFX: playNormalDamageSFX, deathAnimation: Mathf.Clamp(corpseType, 0, StartOfRound.Instance.playerRagdolls.Count));
            PlayCustomAudio();
        }
        private void DamageEnemy(IHittable enemy)
        {
            enemy.Hit(damageForEnemies, hitDir, playHitSFX: playNormalDamageSFX);
            PlayCustomAudio();
        }
        private void DamageObject(IHittable obj)
        {
            obj.Hit(damageForObjects, hitDir, playHitSFX: playNormalDamageSFX);
            PlayCustomAudio();
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
