using GameNetcodeStuff;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Rendering;

namespace WesleyMoonScripts.Components
{
    public class StoneRaycast : MonoBehaviour
    {
        public float Range = 5f;
        public bool ActiveBeam = true;

        public LayerMask mask = 1084754248;
        public Material Replacement;

        void FixedUpdate()
        {
            if (ActiveBeam)
            {
                KillCast();
            }
        }
        public void KillCast()
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, Range, mask))
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    if (hit.collider.gameObject.TryGetComponent(out PlayerControllerB player))
                    {
                        try
                        {
                            Transform scavengerModel = Instantiate(player.transform.Find("ScavengerModel"));
                            scavengerModel.transform.position = player.transform.position;
                            scavengerModel.transform.rotation = player.transform.rotation;

                            if (scavengerModel.TryGetComponent(out LODGroup lodGroup))
                            {
                                WesleyScripts.Instance.mls.LogInfo(lodGroup.GetLODs()[0].renderers.Count());
                            }

                            Transform rig = scavengerModel.Find("metarig");

                            player.thisPlayerModel.shadowCastingMode = ShadowCastingMode.On;

                            if (rig.TryGetComponent(out RigBuilder rigBuilder))
                            {
                                Destroy(rigBuilder);
                            }
                            if (rig.TryGetComponent(out PlayerAnimationEvents animEvents))
                            {
                                Destroy(animEvents);
                            }
                            if (rig.TryGetComponent(out PlayAudioAnimationEvent audioAnim))
                            {
                                Destroy(audioAnim);
                            }
                            if (rig.TryGetComponent(out Animator anim))
                            {
                                Destroy(anim);
                            }

                            BoxCollider boxCollider = scavengerModel.gameObject.AddComponent<BoxCollider>();
                            boxCollider.center = new Vector3(0, 1.2f, 0);
                            boxCollider.size = new Vector3(0.75f, 2.3f, 0.4f);

                            Rigidbody body = scavengerModel.gameObject.AddComponent<Rigidbody>();
                            body.drag = 5;
                            body.angularDrag = 12;

                            for (int i = 0; i < scavengerModel.childCount; i++)
                            {
                                if (scavengerModel.GetChild(i).TryGetComponent(out SkinnedMeshRenderer smr))
                                {
                                    smr.materials = new Material[]
                                    {
                                        Replacement
                                    };
                                    smr.allowOcclusionWhenDynamic = false;
                                }
                            }

                            Destroy(rig.Find("ScavengerModelArmsOnly").gameObject);
                            Destroy(rig.Find("CameraContainer").gameObject);

                        }
                        catch (Exception e)
                        {
                            WesleyScripts.Instance.mls.LogWarning($"Something went wrong destroying parts of the Player Rig!\n{e}");
                        }

                        player.KillPlayer(Vector3.zero, spawnBody: false, CauseOfDeath.Suffocation);
                    }
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            if (Range > 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.TransformDirection(Vector3.forward * Range) + transform.position);
            }
        }
    }
}
