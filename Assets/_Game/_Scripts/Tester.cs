using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using SegoLib;
using SegoLib.Random;
using System;

public class Tester : MonoBehaviour
{
    [SerializeField] private Location[] m_locations;

    [SerializeField] private Hero m_heroPrefab;

    private List<Hero> m_heroes = new List<Hero>();

    public IRandomNumberGenerator rng = new UnityRNG((int)DateTime.Now.Ticks);

    // Start is called before the first frame update
    void Start()
    {
        List<Location> locations = new List<Location>(m_locations);

        for(int i = 0; i < 4 && locations.Count > 0; i++) {
            int index = rng.Range(0, locations.Count);

            Location location = locations[index];
            locations.RemoveAt(index);

            Hero newHero = HeroFactory.CreateHero(location);

            m_heroes.Add(newHero);
        }
    }



    public IRouteTraversal GetRandomRouteToNeighbour(ILocation location, IRandomNumberGenerator rng)
    {
        IRoute route = rng.GetRandom(location.Routes);

        IRouteTraversal routeTraversal;
        if(route.A == location) {
            routeTraversal = new RouteTraversalAToB(route);
        } else {
            routeTraversal = new RouteTraversalBToA(route);
        }

        return routeTraversal;
    }
}