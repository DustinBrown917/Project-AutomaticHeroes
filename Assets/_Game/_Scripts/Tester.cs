using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using SegoLib;
using SegoLib.Random;
using System;
using Game.UI;
using Cinemachine;

public class Tester : MonoBehaviour
{
    [SerializeField] private Location[] m_locations;
    [SerializeField] private CinemachineTargetGroup m_targetGroup;
    [SerializeField] private VM_Hero m_vmHeroPrefab;
    [SerializeField] private Transform m_heroUiContent;
    [SerializeField] private WorldState m_worldState;

    [SerializeField] private HeroCore[] m_heroCores;

    private List<Hero> m_heroes = new List<Hero>();


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < m_heroCores.Length; i++) {
            if (m_heroCores[i] == null) {
                Debug.LogWarning($"Hero core at index {i} is null.", this);
                continue;
            }

            ILocation location = m_locations[m_worldState.Rng.Range(0, m_locations.Length)];

            if (!m_worldState.HeroFactory.TryCreateHero(m_heroCores[i], location, out Hero newHero)) {
                Debug.LogError($"Failed to create hero for core {m_heroCores[i].name}.", this);
                continue;
            }

            m_heroes.Add(newHero);
            m_targetGroup.AddMember(newHero.transform, 1.0f, 2.0f);
            VM_Hero heroUi = Instantiate(m_vmHeroPrefab, m_heroUiContent).GetComponent<VM_Hero>();
            heroUi.SetModel(newHero);
        }
    }
}


public static class TextureCombiner
{
    public static Texture2D Combine(Texture2D baseTexture, Color baseColor, Texture2D primaryTexture, Color primaryColor, Texture2D secondaryTexture, Color secondaryColor)
    {
        Color[] pixels = baseTexture.GetPixels();

        for(int i = 0; i < pixels.Length; i++) {
            Color col = baseColor;
            col.a = pixels[i].a;
            pixels[i] = col;
        }

        Color[] secondaryPixels = secondaryTexture.GetPixels();

        Overlay(pixels, secondaryPixels, secondaryColor);

        Color[] primaryPixels = primaryTexture.GetPixels();

        Overlay(pixels, primaryPixels, primaryColor);

        Texture2D newTex = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.RGBA32, false);
        newTex.SetPixels(pixels);
        newTex.Apply();
        return newTex;
    }

    private static void Overlay(Color[] baseTex, Color[] overlayTex, Color overlayColor)
    {
        for (int i = 0; i < baseTex.Length && i < overlayTex.Length; i++) {
            Color newCol = Color.Lerp(baseTex[i], overlayColor, overlayTex[i].a);
            newCol.a = baseTex[i].a;
            baseTex[i] = newCol;
        }
    }
}
