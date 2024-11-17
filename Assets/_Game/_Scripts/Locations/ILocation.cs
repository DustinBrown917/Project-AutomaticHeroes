using Game.Causes;
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
        public IReadOnlyList<Hero> Visitors { get; }
        public event ValueUpdatedWithSourceEvent<ILocation, IReadOnlyList<Hero>> visitorsUpdated;
        public CauseFavourTracker CauseFavourTracker { get; }

        public void OnLocationEnter(Hero hero);
        public void OnLocationExit(Hero hero);
    }
}
