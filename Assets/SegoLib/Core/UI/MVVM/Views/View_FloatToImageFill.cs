using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SegoLib.UI.MVVM
{
    public class View_FloatToImageFill : View_Base
    {
        [SerializeField] private Image m_image;

        protected override void UpdateFromTargetProperty()
        {
            float fill = GetPropertyValue<float>();
            m_image.fillAmount = fill;
        }
    }
}
