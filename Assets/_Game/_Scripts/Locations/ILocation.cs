using SegoLib;
using System.Collections.Generic;
using UnityEngine;


namespace Game
{
    public interface ILocation : IDisplayName
    {
        public int Id { get; }
        public Vector2 Position { get; }
        public IReadOnlyList<IRoute> Routes { get; }
    }
}
