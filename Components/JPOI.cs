using JLL.API.LevelProperties;
using UnityEngine;

namespace JLL.Components
{
    public class JPOI : MonoBehaviour
    {
        public string POI;

        void Start ()
        {
            JLevelPropertyRegistry.CreatePOI(POI, transform);
        }
    }
}
