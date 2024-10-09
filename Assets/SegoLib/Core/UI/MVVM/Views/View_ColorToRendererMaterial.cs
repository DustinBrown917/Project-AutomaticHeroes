using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.UI.MVVM
{
    public class View_ColorToRendererMaterial : View_Base
    {
        [SerializeField] private Renderer m_renderer;
        [SerializeField] private int m_materialIndex = 0;
        [SerializeField] private bool m_overrideAlpha = false;
        [SerializeField, Range(0f, 1f)] private float m_alphaOverride = 1;

        protected override void UpdateFromTargetProperty()
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();

            m_renderer.GetPropertyBlock(mpb, m_materialIndex);

            Color color = GetPropertyValue<Color>();

            if(m_overrideAlpha)
                color.a = m_alphaOverride;

            mpb.SetColor("_Color", color);

            m_renderer.SetPropertyBlock(mpb, m_materialIndex);
        }
    }

}