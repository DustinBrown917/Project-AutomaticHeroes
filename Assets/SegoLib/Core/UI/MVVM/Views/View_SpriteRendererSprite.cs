using UnityEngine;

namespace SegoLib.UI.MVVM
{
    public class View_SpriteRendererSprite : View_Base
    {
        [SerializeField] private SpriteRenderer m_spriteRenderer;

        protected override void UpdateFromTargetProperty()
        {
            Sprite sprite = GetPropertyValue<Sprite>();
            m_spriteRenderer.sprite = sprite;
        }
    } 
}
