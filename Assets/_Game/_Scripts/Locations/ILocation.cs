using Game.Causes;
using SegoLib;
using System.Collections.Generic;
using UnityEngine;


namespace Game
{
    public interface ILocation : IDisplayName
    {
        public Vector2 Position { get; }
        public IReadOnlyList<IRoute> Routes { get; }
        public IReadOnlyList<Hero> Visitors { get; }
        public event ValueUpdatedWithSourceEvent<ILocation, IReadOnlyList<Hero>> visitorsUpdated;
        public CauseFavourTracker CauseFavourTracker { get; }

        /// <summary>
        /// Whether a hero visiting this location would be offered a behaviour tagged with <paramref name="tag"/>.
        /// </summary>
        public bool OffersBehaviour(Tag tag);

        public void OnLocationEnter(Hero hero);
        public void OnLocationExit(Hero hero);
    }
}
