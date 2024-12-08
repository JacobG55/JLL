using System;
using System.Collections.Generic;
using UnityEngine;

namespace JLL.API.LevelProperties
{
    [Serializable]
    public class LevelPOI
    {
        private readonly List<Transform> Points = new List<Transform>();

        private LevelPOI() { }

        public static LevelPOI Create()
        {
            return new LevelPOI();
        }

        public int Count()
        {
            return Points.Count;
        }

        public Vector3 GetPoint(int index)
        {
            return Points[Math.Clamp(index, 0, Points.Count-1)].position;
        }

        public Transform GetTransform(int index)
        {
            return Points[Math.Clamp(index, 0, Points.Count - 1)];
        }

        public void Add(Transform t)
        {
            Points.Add(t);
        }
    }
}
