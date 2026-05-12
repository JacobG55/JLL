using System.Collections;
using UnityEngine;

namespace JLL.Components
{
    public class RigidBodyCuller : MonoBehaviour
    {
        public float threshold = 20f;
        public Rigidbody[] bodies = new Rigidbody[0];
        public bool beStupid = false;

        private bool active = false;
        [HideInInspector]
        public bool Culled { get; private set; } = false;

        public void Start()
        {
            foreach (Rigidbody body in bodies)
            {
                if (body != null) body.Sleep();
            }
            StartCoroutine(AwaitDungeonLoad());
        }

        private IEnumerator AwaitDungeonLoad()
        {
            yield return new WaitUntil(() => RoundManager.Instance.dungeonCompletedGenerating);
            active = true;
        }

        public void FixedUpdate()
        {
            if (!active) return;

            if (beStupid)
            {
                Culled = Vector3.Distance(StartOfRound.Instance.localPlayerController.transform.position, transform.position) > threshold;

                foreach (Rigidbody body in bodies)
                {
                    body.isKinematic = Culled;
                }
            }
            else
            {
                if (Vector3.Distance(StartOfRound.Instance.localPlayerController.transform.position, transform.position) > threshold)
                {
                    foreach (Rigidbody body in bodies)
                    {
                        if (body.IsSleeping()) body.Sleep();
                    }
                    Culled = true;
                }
                else
                {
                    if (Culled)
                    {
                        foreach (Rigidbody body in bodies)
                        {
                            body.AddForce(Vector3.zero);
                        }
                    }
                    Culled = false;
                }
            }
        }
    }
}
