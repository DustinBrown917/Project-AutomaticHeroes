using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class ProportionBar : MonoBehaviour
    {
        private enum EDirection : byte
        {
            Horizontal,
            Vertical
        }

        [SerializeField] private Image m_imageTemplate;
        [SerializeField] private EDirection m_direction;
        [SerializeField, HideInInspector] private List<Image> m_images = new();
        [SerializeField] private Color[] m_colors = System.Array.Empty<Color>();
        [SerializeField] private float[] m_proportions = System.Array.Empty<float>();

        [SerializeField] private bool m_validateInEditor = false;

        private void Start()
        {
            SetColors(m_colors);
        }

        public void SetColors(Color[] colors)
        {
            if (colors == null)
                m_colors = System.Array.Empty<Color>();
            else
                m_colors = colors;


            int imageIndex = 0;
            for(; imageIndex < m_colors.Length; imageIndex++) {
                Image image = GetOrCreateImage(imageIndex);

                image.gameObject.SetActive(true);
                image.color = m_colors[imageIndex];
            }

            for(; imageIndex < m_images.Count; imageIndex++) {
                m_images[imageIndex].gameObject.SetActive(false);
            }

            float[] oldProportions = m_proportions;
            m_proportions = new float[m_colors.Length];

            SetProportions(oldProportions);
        }

        public void SetProportions(float[] proportions)
        {
            for(int i = 0; i < proportions.Length && i < m_proportions.Length; i++) {
                m_proportions[i] = proportions[i];

                if (m_proportions[i] < 0)
                    m_proportions[i] = 0;
            }

            UpdateProportions();
        }

        private void OnValidate()
        {
            if (!m_validateInEditor || m_imageTemplate == null)
                return;
            ValidateImages();
            SetColors(m_colors);
        }

        private void ValidateImages()
        {
            for(int i = m_images.Count -1;  i >= 0; i--) {
                if (m_images[i] == null) {
                    m_images.RemoveAt(i);
                }
            }
        }

        private void UpdateProportions()
        {
            float sumOfProportions = GetSumOfProportions();

            if (sumOfProportions <= 0)
                sumOfProportions = 1;

            float lastProportionEnd = 0;
            for(int i = 0; i < m_proportions.Length; i++) {
                float currentProportion = m_proportions[i] / sumOfProportions;
                float currentProportionEnd = lastProportionEnd + currentProportion;

                SetProportionSize(i, lastProportionEnd, currentProportionEnd);

                lastProportionEnd = currentProportionEnd;
            }
        }

        private float GetSumOfProportions()
        {
            float sum = 0;

            for(int i = 0; i < m_proportions.Length; i++) {
                if (m_proportions[i] < 0)
                    m_proportions[i] = 0;

                sum += m_proportions[i];
            }

            return sum;
        }

        private Image GetOrCreateImage(int index)
        {
            if(m_images.Count > index)
                return m_images[index];

            Image newImage = Instantiate(m_imageTemplate, transform).GetComponent<Image>();
            m_images.Add(newImage);
            return newImage;
        }

        private void SetProportionSize(int index, float min, float max)
        {
            RectTransform rt = m_images[index].GetComponent<RectTransform>();

            if (m_direction == EDirection.Horizontal) { 
                rt.anchorMin = new Vector2(min, 0);
                rt.anchorMax = new Vector2(max, 1); 
            } else if (m_direction == EDirection.Vertical) {
                rt.anchorMin = new Vector2(0, min);
                rt.anchorMax = new Vector2(1, max);
            }

            rt.sizeDelta = Vector2.zero;
        }
    } 
}
