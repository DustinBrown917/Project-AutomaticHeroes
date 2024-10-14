using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New Sprite Library", menuName = "Game/Sprite Library")]
    public class SpriteLibrary : ScriptableObject
    {
        private const string RESOURCE_PATH = "Sprite Libraries";
        private static readonly Dictionary<string, Sprite> m_loadedSprites = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            SpriteLibrary[] libraries = Resources.LoadAll<SpriteLibrary>(RESOURCE_PATH);

            foreach(SpriteLibrary library in libraries) {
                foreach(Sprite sprite in library.m_sprites) {
                    m_loadedSprites[sprite.name] = sprite;
                }
            }

            foreach(SpriteLibrary library in libraries) {
                Resources.UnloadAsset(library);
            }
        }

        public static Sprite GetSprite(string name)
        {
            if(m_loadedSprites.TryGetValue(name, out Sprite sprite)) {
                return sprite;
            }

            return null;
        }

        [SerializeField] private Sprite[] m_sprites;
    } 
}
