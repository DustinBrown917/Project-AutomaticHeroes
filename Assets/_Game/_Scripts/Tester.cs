using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using SegoLib;
using SegoLib.Random;
using System;
using Game.UI;

public class Tester : MonoBehaviour
{
    [SerializeField] private Location[] m_locations;

    [SerializeField] private VM_Hero m_vmHeroPrefab;
    [SerializeField] private Transform m_heroUiContent;

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

            VM_Hero heroUi = Instantiate(m_vmHeroPrefab, m_heroUiContent).GetComponent<VM_Hero>();
            heroUi.SetModel(newHero);
        }
    }
}