using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IRoute
    {
        public ILocation A { get; }
        public ILocation B { get; }

        public float Length { get; }

        public IReadOnlyList<Vector2> Points { get; }

        public Vector2 TraverseAToB(float t);
        public Vector2 TraverseBToA(float t);
    }
}
