using System;
using UnityEngine;
using UnityEngine.UI;

namespace SegoLib.UI.MVVM
{
    public class View_FloatToImageSorted : View_Base
    {
        [System.Serializable]
        public class FloatSpritePair
        {
            public float value;
            public Sprite sprite;
        }

        [SerializeField] private Image[] m_images;
        [SerializeField] private FloatSpritePair[] m_sprites;

        protected override void UpdateFromTargetProperty()
        {
            int value = GetPropertyValue<int>();

            if (m_sprites == null || m_sprites.Length == 0)
                return;

            Sprite sprite = m_sprites[^1].sprite;
            for(int i = 0; i < m_sprites.Length; i++) {
                if(value <= m_sprites[i].value) {
                    sprite = m_sprites[i].sprite;
                    break;
                }
            }

            for(int i = 0; i < m_images.Length; i++) {
                m_images[i].sprite = sprite;
            }
        }

        private void OnValidate()
        {
            if(m_sprites != null && m_sprites.Length > 0) {
                Array.Sort(m_sprites, (x, y) => x.value.CompareTo(y.value));
            }

        }

    } 
}
