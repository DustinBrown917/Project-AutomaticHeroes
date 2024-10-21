using UnityEngine;
using UnityEngine.UI;

namespace SegoLib.UI.MVVM
{
    public class View_Image : View_Base
    {
        [SerializeField] private Image m_image;

        [SerializeField] private Sprite m_fallbackSpriteIfValueIsNull;
        protected override void UpdateFromTargetProperty()
        {
            Sprite sprite = GetPropertyValue<Sprite>();

            if(sprite == null) {
                m_image.sprite = m_fallbackSpriteIfValueIsNull;
            } else {
                m_image.sprite = sprite;
            }
        }
    }
}
