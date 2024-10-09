using UnityEngine;
using UnityEngine.UI;

namespace SegoLib.UI.MVVM
{
    public class View_ColorToUIGraphicColor : View_Base
    {
        [SerializeField] private Graphic m_graphic;
        [SerializeField] private bool m_overrideAlpha = false;
        [SerializeField, Range(0f, 1f)] private float m_alphaOverride = 1;

        protected override void UpdateFromTargetProperty()
        {
            Color color = GetPropertyValue<Color>();

            if(m_overrideAlpha)
                color.a = m_alphaOverride;

            m_graphic.color = color;
        }
    }

}