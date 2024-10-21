using UnityEngine;
using NaughtyAttributes;
using Unity.VisualScripting;
using System;

namespace Game
{
    [CreateAssetMenu(fileName = "New Hero Core", menuName = "Game/Hero Core")]
    public class HeroCore : ScriptableObject
    {
        public string Name => name;

        public string ColoredName
        {
            get {
                if (string.IsNullOrEmpty(m_coloredName))
                    UpdateColoredName();
                return m_coloredName;
            }
        }
        [NonSerialized] private string m_coloredName;

        [field: SerializeField, ShowAssetPreview] public Sprite CharacterSprite { get; private set; }
        [field: SerializeField] public Color Base { get; private set; }
        [field: SerializeField] public Color Primary { get; private set; } = Color.white;
        [field: SerializeField] public Color Secondary { get; private set; } = Color.black;

        [field: SerializeField] public Texture2D m_bannerBase;
        [field: SerializeField] public Texture2D m_primaryDecal;
        [field: SerializeField] public Texture2D m_secondaryDecal;

        public IWeapon UnnarmedWeapon => m_unnarmedWeapon;
        [SerializeField] private ScriptableWeapon m_unnarmedWeapon;

        public Sprite BannerSprite
        {
            get {
                if(m_bannerSprite == null) {
                    CreateBannerSprite();
                }
                return m_bannerSprite;
            }
        }
        private Sprite m_bannerSprite;

        private void CreateBannerSprite()
        {
            Texture2D bannerTex = TextureCombiner.Combine(m_bannerBase, Base, m_primaryDecal, Primary, m_secondaryDecal, Secondary);
            m_bannerSprite = Sprite.Create(bannerTex, new Rect(0, 0, bannerTex.width, bannerTex.height), new Vector2(0.5f, 0), 256);
        }

        private void UpdateColoredName()
        {
            m_coloredName = $"<color=#{Primary.ToHexString()}>{Name}</color>";
        }
    } 
}
