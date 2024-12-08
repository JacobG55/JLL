using JLL.API;
using JLL.API.LevelProperties;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace JLL.Components
{
    public interface IDungeonLoadListener
    {
        public abstract void PostDungeonGeneration();
    }

    public class JRandomPropPlacer : MonoBehaviour, IDungeonLoadListener
    {
        public SpawnNodes spawnNodeSelection = SpawnNodes.Children;
        [Tooltip("When left empty will default to this gameobject.")]
        public Transform PropContainer;

        public SpawnableProp[] spawnableProps = new SpawnableProp[1] { new SpawnableProp() };

        public NavMeshToRebake rebakeNavMesh = NavMeshToRebake.None;
        public NavMeshSurface[] rebakeSurfaces;

        [Serializable]
        public class SpawnableProp
        {
            public GameObject prefabToSpawn;
            public AnimationCurve randomAmount = new AnimationCurve();
            public float prefabWidth = 2f;
            public SpawnRotation spawnRotation = SpawnRotation.Random;
            public bool spawnFlushAgainstWall = false;
            public float distanceFromEntrances = 10f;
            public float distanceBetweenSpawns = 5f;
            public float randomSpawnRange = 10f;

            public Vector3 PositionEdgeCheck(Vector3 position)
            {
                if (NavMesh.FindClosestEdge(position, out NavMeshHit navHit, -1) && navHit.distance < prefabWidth)
                {
                    Vector3 position2 = navHit.position;
                    if (NavMesh.SamplePosition(new Ray(position2, position - position2).GetPoint(prefabWidth + 0.5f), out navHit, 10f, -1))
                    {
                        position = navHit.position;
                        return position;
                    }
                    return Vector3.zero;
                }
                return position;
            }
        }

        public enum SpawnRotation
        {
            Random,
            FacingWall,
            FacingAwayFromWall,
            BackToWall
        }

        public enum SpawnNodes
        {
            Children,
            OutsideAINodes,
            InsideAINodes,
            AINodes,
        }

        public enum NavMeshToRebake
        {
            None = 0,
            Exterior,
            Custom = -1,
        }

        void Start()
        {
            if (PropContainer == null)
            {
                PropContainer = transform;
            }
        }

        public void PostDungeonGeneration()
        {
            StartCoroutine(SpawnNextFrame());
        }

        private IEnumerator SpawnNextFrame()
        {
            yield return null;
            SpawnProps();
        }

        private void SpawnProps()
        {
            JLogHelper.LogInfo($"{name} Spawning map props. {PropContainer == null} {spawnableProps.Length == 0}");
            if (PropContainer == null || spawnableProps.Length == 0) return;

            System.Random random = new System.Random(StartOfRound.Instance.randomMapSeed + 666);

            // Sets up posible spawn nodes.

            List<Vector3> spawnNodes = new List<Vector3>();
            switch (spawnNodeSelection)
            {
                case SpawnNodes.Children:
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        spawnNodes.Add(transform.GetChild(i).position);
                    }
                    break;
                case SpawnNodes.OutsideAINodes:
                    for (int i = 0; i < RoundManager.Instance.outsideAINodes.Length; i++)
                    {
                        spawnNodes.Add(RoundManager.Instance.outsideAINodes[i].transform.position);
                    }
                    break;
                case SpawnNodes.InsideAINodes:
                    for (int i = 0; i < RoundManager.Instance.insideAINodes.Length; i++)
                    {
                        spawnNodes.Add(RoundManager.Instance.insideAINodes[i].transform.position);
                    }
                    break;
                case SpawnNodes.AINodes:
                    for (int i = 0; i < RoundManager.Instance.outsideAINodes.Length; i++)
                    {
                        spawnNodes.Add(RoundManager.Instance.outsideAINodes[i].transform.position);
                    }
                    for (int i = 0; i < RoundManager.Instance.insideAINodes.Length; i++)
                    {
                        spawnNodes.Add(RoundManager.Instance.insideAINodes[i].transform.position);
                    }
                    break;
                default: break;
            }

            JLogHelper.LogInfo($"{name} Found {spawnNodes.Count} nodes available for random selection.");
            if (spawnNodes.Count == 0) return;

            // Spawn Props at Nodes.

            List<Vector3> spawnedPositions = new List<Vector3>();
            int spawnedSuccessfully = 0;

            for (int i = 0; i < spawnableProps.Length; i++)
            {
                int num = Mathf.RoundToInt(spawnableProps[i].randomAmount.Evaluate((float)random.NextDouble()));
                if (num <= 0)
                {
                    continue;
                }
                SpawnableProp spawnableProp = spawnableProps[i];

                bool skipOnClient = spawnableProp.prefabToSpawn.GetComponent<NetworkObject>() != null && !(RoundManager.Instance.IsServer || RoundManager.Instance.IsHost);

                for (int k = 0; k < num; k++)
                {
                    Vector3 position = RoundManager.Instance.GetRandomNavMeshPositionInBoxPredictable(spawnNodes[random.Next(0, spawnNodes.Count)], spawnableProp.randomSpawnRange, default(NavMeshHit), random);

                    // Check if position is invalid.
                    bool invalidLocation = false;

                    if (spawnableProp.distanceFromEntrances > 0)
                    {
                        for (int E = 0; E < JLevelPropertyRegistry.EntranceTeleports.Count; E++)
                        {
                            if (Vector3.Distance(JLevelPropertyRegistry.EntranceTeleports[E].transform.position, position) < spawnableProp.distanceFromEntrances)
                            {
                                invalidLocation = true;
                                break;
                            }
                        }
                    }
                    if (invalidLocation)
                    {
                        JLogHelper.LogInfo($"{name} Skiped spawning prop. (Too close to an entrance teleport) {position}", JLogLevel.Wesley);
                        continue;
                    }
                    if (spawnableProp.distanceBetweenSpawns > 0)
                    {
                        for (int S = 0; S < spawnedPositions.Count; S++)
                        {
                            if (Vector3.Distance(position, spawnedPositions[S]) < spawnableProp.distanceBetweenSpawns)
                            {
                                invalidLocation = true;
                                break;
                            }
                        }
                    }
                    if (invalidLocation)
                    {
                        JLogHelper.LogInfo($"{name} Skiped spawning prop. (Too close to another prop) {position}", JLogLevel.Wesley);
                        continue;
                    }

                    Vector3 eulerAngles = spawnableProp.spawnRotation switch
                    {
                        SpawnRotation.FacingAwayFromWall => new Vector3(0f, RoundManager.Instance.YRotationThatFacesTheFarthestFromPosition(position + Vector3.up * 0.2f), 0f),
                        SpawnRotation.FacingWall => new Vector3(0f, RoundManager.Instance.YRotationThatFacesTheNearestFromPosition(position + Vector3.up * 0.2f), 0f),
                        SpawnRotation.BackToWall => RandomEulers(spawnableProp.prefabToSpawn.transform.eulerAngles, random),
                        SpawnRotation.Random => RandomEulers(spawnableProp.prefabToSpawn.transform.eulerAngles, random),
                        _ => new Vector3(spawnableProp.prefabToSpawn.transform.eulerAngles.x, 0, spawnableProp.prefabToSpawn.transform.eulerAngles.z),
                    };

                    Vector3 spawnPos = spawnableProp.PositionEdgeCheck(position);
                    spawnedPositions.Add(spawnPos);

                    if (skipOnClient) continue;

                    // Spawn Prop in world. 

                    GameObject spawned = Instantiate(spawnableProp.prefabToSpawn, spawnPos, Quaternion.identity, PropContainer);

                    // Rotate Prop
                    spawned.transform.eulerAngles = eulerAngles;

                    if (spawnableProp.spawnRotation == SpawnRotation.BackToWall && Physics.Raycast(spawned.transform.position, -spawned.transform.forward, out var hitInfo, 100f, StartOfRound.Instance.collidersAndRoomMaskAndDefault, QueryTriggerInteraction.Ignore))
                    {
                        spawned.transform.position = hitInfo.point;
                        if (spawnableProp.spawnFlushAgainstWall)
                        {
                            spawned.transform.forward = hitInfo.normal;
                            spawned.transform.eulerAngles = new Vector3(0f, spawned.transform.eulerAngles.y, 0f);
                        }
                    }

                    if (spawned.TryGetComponent(out NetworkObject netObj))
                    {
                        netObj.Spawn(destroyWithScene: true);
                    }
                    JLogHelper.LogInfo($"{name} spawned {spawned.name} at {spawned.transform.position}", JLogLevel.Wesley);
                    spawnedSuccessfully++;
                }
            }

            JLogHelper.LogInfo($"{name} spawned {spawnedSuccessfully} props!");
            if (spawnedSuccessfully > 0)
            {
                switch (rebakeNavMesh)
                {
                    case NavMeshToRebake.Exterior:
                        GameObject.FindGameObjectWithTag("OutsideLevelNavMesh")?.GetComponent<NavMeshSurface>()?.BuildNavMesh();
                        break;
                    case NavMeshToRebake.Custom:
                        for (int i = 0; i < rebakeSurfaces.Length; i++)
                        {
                            rebakeSurfaces[i]?.BuildNavMesh();
                        }
                        break;
                    default: break;
                }
            }
        }

        private Vector3 RandomEulers(Vector3 original, System.Random random)
        {
            return new Vector3(original.x, random.Next(0, 360), original.z);
        }
    }
}
