using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SegoLib.UI
{
    public class TooltipContainer : MonoBehaviour
    {
        /// <summary>
        /// The transform of this object's parent.
        /// </summary>
        [SerializeField] private RectTransform m_parentTransform;
        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private float m_fadeSpeed = 0.2f;
        /// <summary>
        /// The offset the tooltip will maintain from the cursor.
        /// </summary>
        [SerializeField] private Vector2 m_offset;
        /// <summary>
        /// This object's transform as a RectTransform.
        /// </summary>
        private RectTransform m_rectTransform;
        /// <summary>
        /// The tooltip content that is currently attached to this container.
        /// </summary>
        private TooltipContent m_tooltipContent;

        private Coroutine m_fadeCoroutine;
        private bool fadingIn;
        private bool fadingOut;

        private bool m_needsDimensionUpdate = false;

        private void Awake()
        {
            m_rectTransform = (RectTransform)transform;
            m_parentTransform = (RectTransform)transform.parent;
        }

        private void OnEnable()
        {
            m_fadeCoroutine = StartCoroutine(FadeIn());
        }

        private void OnDisable()
        {
            StopFade();
        }

        private void Update()
        {
            Vector2 mousePos = Input.mousePosition;
            m_rectTransform.anchoredPosition = mousePos;

            if (m_needsDimensionUpdate) {
                UpdateDimensions();
            }

            AdjustPivot();
        }

        public void SetTooltipContent(TooltipContent content)
        {
            if(m_tooltipContent == content) return;
            if(m_tooltipContent != null) {
                Debug.LogError("Attempting to set new tooltip content, but existing content is not null.");
                return;
            }

            m_tooltipContent = content;
            m_tooltipContent.RectTransform.SetParent(m_rectTransform);
            m_tooltipContent.RectTransform.anchoredPosition = Vector2.zero;

            m_needsDimensionUpdate = true;
        }

        private void UpdateDimensions()
        {
            if(m_tooltipContent == null) {
                return;
            }

            m_rectTransform.sizeDelta = m_tooltipContent.RectTransform.sizeDelta;
            m_needsDimensionUpdate = false;
        }

        private void AdjustPivot()
        {
            Vector2 offsetPos = m_rectTransform.anchoredPosition;

            if(m_rectTransform.anchoredPosition.x + m_rectTransform.rect.width + m_offset.x > m_parentTransform.rect.width) {
                m_rectTransform.pivot = new Vector2(1, m_rectTransform.pivot.y);
                offsetPos.x -= m_offset.x;
            } else {
                m_rectTransform.pivot = new Vector2(0, m_rectTransform.pivot.y);
                offsetPos.x += m_offset.x;
            }

            if (m_rectTransform.anchoredPosition.y + m_rectTransform.rect.height + m_offset.y > m_parentTransform.rect.height) {
                m_rectTransform.pivot = new Vector2(m_rectTransform.pivot.x, 1);
                offsetPos.y -= m_offset.y;
            } else {
                m_rectTransform.pivot = new Vector2(m_rectTransform.pivot.x, 0);
                offsetPos.y += m_offset.y;
            }

            m_rectTransform.anchoredPosition = offsetPos;
        }

        public bool ExtractCurrentTooltipContent(out TooltipContent content)
        {
            if(m_tooltipContent == null) {
                content = null;
                return false;
            }

            m_tooltipContent.transform.SetParent(null);
            content = m_tooltipContent;
            m_tooltipContent = null;
            return true;
        }

        private void StopFade()
        {
            if(m_fadeCoroutine != null) {
                StopCoroutine(m_fadeCoroutine);
                m_fadeCoroutine = null;
            }


            if (fadingIn) {
                fadingIn = false;
                m_canvasGroup.alpha = 1;
            }

            if (fadingOut) {
                fadingOut = false;
                m_canvasGroup.alpha = 0;
            }
        }

        private IEnumerator FadeIn()
        {
            fadingIn = true;
            m_canvasGroup.alpha = 0.0f;

            yield return null;

            while(m_canvasGroup.alpha < 1.0f) {
                m_canvasGroup.alpha += m_fadeSpeed * Time.deltaTime;
                yield return null;
            }

            m_canvasGroup.alpha = 1.0f;
            fadingIn = false;
            m_fadeCoroutine = null;
        }

        private IEnumerator FadeOut()
        {
            fadingOut = true;
            m_canvasGroup.alpha = 1.0f;

            yield return null;

            while (m_canvasGroup.alpha > 0.0f) {
                m_canvasGroup.alpha += m_fadeSpeed * Time.deltaTime;
                yield return null;
            }

            m_canvasGroup.alpha = 0.0f;
            fadingOut = false;
            m_fadeCoroutine = null;
        }
    }
}
