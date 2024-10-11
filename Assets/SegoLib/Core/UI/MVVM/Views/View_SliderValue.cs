using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SegoLib.UI.MVVM
{
    public class View_SliderValue : View_Base
    {
        [SerializeField] private Slider m_slider;

        protected override void UpdateFromTargetProperty()
        {
            float value = GetPropertyValue<float>();
            if (float.IsNaN(value))
                value = 0f;
            m_slider.value = value;
        }

    }

}