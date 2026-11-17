using UnityEngine;
using Game;

public class Tester : MonoBehaviour
{
    [SerializeField] private Location[] m_locations;
    [SerializeField] private WorldState m_worldState;

    [SerializeField] private HeroCore[] m_heroCores;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < m_heroCores.Length; i++) {
            if (m_heroCores[i] == null) {
                Debug.LogWarning($"Hero core at index {i} is null.", this);
                continue;
            }

            ILocation location = m_locations[m_worldState.Rng.Range(0, m_locations.Length)];

            if (!m_worldState.HeroFactory.TryCreateHero(m_heroCores[i], location, out _)) {
                Debug.LogError($"Failed to create hero for core {m_heroCores[i].name}.", this);
            }
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
