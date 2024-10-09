using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SegoLib.UI.MVVM
{
    public class View_Image : View_Base
    {
        [SerializeField] private Image m_image;

        protected override void UpdateFromTargetProperty()
        {
            Sprite sprite = GetPropertyValue<Sprite>();
            m_image.sprite = sprite;
        }
    }
}
