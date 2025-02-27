using GameNetcodeStuff;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace WesleyMoonScripts.Components
{
    public class Mortor : NetworkBehaviour
    {
        public AnimationCurve arc;
        public float projAirTime = 5f;

        [Range(0f, 100f)]
        public float targetPlayerChance = 60f;

        [Header("Firing")]
        public int shellCount = 4;
        private int firedShells = 0;
        private int playerShell = -1;
        private bool isFiring = false;
        public float shellSeperation = 0.5f;
        private float timeSinceLastShell = 0;
        public float timeBetweenFires = 20f;
        private float cooldownTimer = 0;
        [HideInInspector] public bool OnCooldown => cooldownTimer > 0;

        [Header("Prefabs")]
        public GameObject ProjectilePrefab;
        public float destroyAfter = 5f;
        public GameObject ExplosionPrefab;

        public float killDistance = 8f;
        public float damageRange = 18f;
        public int nonLethalDMG = 50;
        public float pushForce = 60;

        [Header("Targetting Region")]
        public bool drawDebug = true;
        public MortorRange targetRange = MortorRange.Surface;
        public enum MortorRange
        {
            Surface,
            MortorProximity
        }
        public float proximityDistance = 35;

        public bool fireOnLoop = true;
        public AnimationCurve fireLoopCooldown;
        private float fireCooldown = 0;

        private List<Shell> launchedProjectiles = new List<Shell>();
        private class Shell
        {
            public GameObject obj;
            public bool inAir { get; private set; } = true;
            public float destroyTimer = 0;
            public float airTime = 0;
            public Vector3 targetPos = Vector3.zero;

            public Vector3 CalcPos(Vector3 origin, AnimationCurve arc, float progress)
            {
                Vector3 lerpPos = Vector3.Lerp(origin, targetPos, progress);
                return new Vector3(lerpPos.x, lerpPos.y + arc.Evaluate(progress), lerpPos.z);
            }

            public void Explode(float killRange, float damageRange, int nonLethalDamage, float pushForce, GameObject prefab)
            {
                Landmine.SpawnExplosion(targetPos + Vector3.up, true, killRange, damageRange, nonLethalDamage, pushForce, prefab, true);
                inAir = false;
                foreach(MeshRenderer renderer in obj.GetComponentsInChildren<MeshRenderer>())
                {
                    renderer.enabled = false;
                }
            }
        }

        [Header("FX")]
        public ParticleSystem shootParticle;
        public AudioSource nearSource;
        public AudioClip nearClip;
        public AudioSource farSource;
        public AudioClip farClip;
        public Vector2 pitchMinMax = new Vector2(1, 1);

        void OnEnable()
        {
            cooldownTimer = timeBetweenFires * Random.Range(0.8f, 1.35f);
        }

        void Update()
        {
            if (IsHost || IsServer)
            {
                if (OnCooldown)
                {
                    cooldownTimer -= Time.deltaTime;
                }
                else if (isFiring)
                {
                    timeSinceLastShell += Time.deltaTime;
                    if (timeSinceLastShell > shellSeperation)
                    {
                        FireShellServerRpc();
                        timeSinceLastShell = 0;
                        firedShells++;
                    }
                    if (firedShells >= shellCount)
                    {
                        isFiring = false;
                        firedShells = 0;
                        cooldownTimer = timeBetweenFires;
                        if (fireOnLoop)
                        {
                            fireCooldown = fireLoopCooldown.Evaluate(TimeOfDay.Instance.normalizedTimeOfDay);
                        }
                    }
                }
                else if (fireOnLoop)
                {
                    if (fireCooldown < 0)
                    {
                        FireShellSequence();
                    }
                    else fireCooldown -= Time.deltaTime;
                }
            }

            foreach (var shell in new List<Shell>(launchedProjectiles))
            {
                if (shell.inAir)
                {
                    shell.airTime += Time.deltaTime;
                    if (shell.airTime > projAirTime)
                    {
                        shell.Explode(killDistance, damageRange, nonLethalDMG, pushForce, ExplosionPrefab);
                    }
                    else
                    {
                        shell.obj.transform.position = shell.CalcPos(transform.position, arc, shell.airTime / projAirTime);
                        shell.obj.transform.LookAt(shell.CalcPos(transform.position, arc, (shell.airTime + 0.01f) / projAirTime));
                    }
                }
                else
                {
                    shell.destroyTimer -= Time.deltaTime;
                    if (shell.destroyTimer < 0)
                    {
                        Destroy(shell.obj);
                        launchedProjectiles.Remove(shell);
                    }
                }
            }
        }

        public void EnableLooping() => ToggleLoopFireServerRpc(true);
        public void DisableFire() => ToggleLoopFireServerRpc(false);
        public void ToggleLoop(bool toggle) => ToggleLoopFireServerRpc(toggle);

        [ServerRpc(RequireOwnership = false)]
        private void ToggleLoopFireServerRpc(bool enabled)
        {
            ToggleLoopFireClientRpc(enabled);
        }

        [ClientRpc]
        private void ToggleLoopFireClientRpc(bool enabled)
        {
            fireOnLoop = enabled;
            if (enabled)
            {
                FireShellSequence();
            }
            else
            {
                isFiring = false;
                cooldownTimer = timeBetweenFires;
            }
        }

        public void FireShellSequence(bool respectCooldown = true) => FireShellSequence(targetPlayerChance, respectCooldown);
        public void FireShellSequence(float playerChance, bool respectCooldown = true)
        {
            if (respectCooldown && OnCooldown) return;
            firedShells = 0;
            timeSinceLastShell = 0;
            cooldownTimer = 0;
            isFiring = true;
            playerShell = Random.Range(0, 100f) < playerChance ? Random.Range(0, firedShells) : -1;
        }

        [ServerRpc]
        private void FireShellServerRpc()
        {
            List<Vector3> validPos = new List<Vector3>();
            if (firedShells == playerShell)
            {
                switch (targetRange)
                {
                    case MortorRange.MortorProximity:
                        foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
                        {
                            if (!player.isPlayerDead && Vector3.Distance(player.transform.position, transform.position) < proximityDistance 
                                && NavMesh.SamplePosition(player.transform.position, out NavMeshHit hit, 15, 655))
                            {
                                validPos.Add(hit.position);
                            }
                        }
                        break;
                    default:
                        foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
                        {
                            if (!player.isPlayerDead && !player.isInsideFactory && !player.isInElevator
                                && NavMesh.SamplePosition(player.transform.position, out NavMeshHit hit, 15, 655))
                            {
                                validPos.Add(hit.position);
                            }
                        }
                        break;
                }
            }
            if (validPos.Count == 0)
            {
                Vector3 result;
                switch (targetRange)
                {
                    case MortorRange.Surface:
                        foreach (var node in RoundManager.Instance.outsideAINodes)
                        {
                            if (RandomNavPos(node.transform.position, proximityDistance, out result))
                            {
                                validPos.Add(result);
                            }
                        }
                        break;
                    case MortorRange.MortorProximity:
                        if (RandomNavPos(transform.position, proximityDistance, out result))
                        {
                            validPos.Add(result);
                        }
                        break;
                    default: break;
                }
            }
            if (validPos.Count > 0)
            {
                FireShellClientRpc(validPos[Random.Range(0, validPos.Count)]);
            }
        }

        [ClientRpc]
        private void FireShellClientRpc(Vector3 targetPos)
        {
            FireShellLocal(targetPos);
        }

        private void FireShellLocal(Vector3 targetPos)
        {
            float pitch = pitchMinMax.x >= pitchMinMax.y ? pitchMinMax.y : Random.Range(pitchMinMax.x, pitchMinMax.y);
            if (nearSource != null && nearClip != null)
            {
                nearSource.pitch = pitch;
                nearSource.PlayOneShot(nearClip);
            }
            if (farSource != null && farClip != null)
            {
                farSource.pitch = pitch;
                farSource.PlayOneShot(farClip);
            }
            shootParticle?.Play();
            Shell shell = new Shell
            {
                obj = Instantiate(ProjectilePrefab, transform.position, Quaternion.identity),
                destroyTimer = destroyAfter,
                airTime = 0,
                targetPos = targetPos
            };
            launchedProjectiles.Add(shell);
            transform.LookAt(shell.CalcPos(transform.position, arc, 0.01f));
        }

        public bool RandomNavPos(Vector3 pos, float radius, out Vector3 result)
        {
            pos = Random.insideUnitSphere * radius + pos;
            if (NavMesh.SamplePosition(pos, out NavMeshHit navHit, radius, 655))
            {
                result = navHit.position;
                return true;
            }
            result = Vector3.zero;
            return false;
        }

        void OnDrawGizmos()
        {
            if (!drawDebug) return;

            switch (targetRange)
            {
                case MortorRange.Surface:
                    foreach (var node in GameObject.FindGameObjectsWithTag("OutsideAINode"))
                    {
                        if (proximityDistance > 0)
                        {
                            Gizmos.color = Color.black;
                            Gizmos.DrawWireSphere(node.transform.position, proximityDistance);
                        }
                        if (killDistance > 0)
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawWireSphere(node.transform.position, killDistance);
                        }
                        if (damageRange > 0)
                        {
                            Gizmos.color = Color.yellow;
                            Gizmos.DrawWireSphere(node.transform.position, damageRange);
                        }
                    }
                    break;
                case MortorRange.MortorProximity:
                    if (proximityDistance > 0)
                    {
                        Gizmos.color = Color.black;
                        Gizmos.DrawWireSphere(transform.position, proximityDistance);
                    }
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
                    break;
                default: break;
            }
        }

        public void OnDisable()
        {
            foreach (var projectile in launchedProjectiles)
            {
                Destroy(projectile.obj);
            }
        }
    }

    public class MortorSensor : MonoBehaviour
    {
        public Mortor mortor;
        public bool respectCooldown = true;

        [Range(0f, 100f)]
        public float triggerChance = 20f;

        void OnTriggerEnter(Collider other)
        {
            if (!(mortor.IsHost || mortor.IsServer)) return;

            if (Random.Range(0, 100f) < triggerChance && other.gameObject.TryGetComponent(out PlayerControllerB player))
            {
                mortor.FireShellSequence(100, respectCooldown);
            }
        }
    }
}
