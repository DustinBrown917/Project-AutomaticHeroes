using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New Hero Color", menuName = "Game/Hero Color")]
    public class HeroColor : ScriptableObject
    {
        [field: SerializeField] public Sprite CharacterSprite { get; private set; }
        [field: SerializeField] public Color Base { get; private set; }
        [field: SerializeField] public Color Primary { get; private set; } = Color.white;
        [field: SerializeField] public Color Secondary { get; private set; } = Color.black;

        [field: SerializeField] public Texture2D m_bannerBase;
        [field: SerializeField] public Texture2D m_primaryDecal;
        [field: SerializeField] public Texture2D m_secondaryDecal;

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

    } 
}
