using SegoLib.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using NaughtyAttributes;

namespace Soma
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
#if UNITY_EDITOR
        [ValidateInput(nameof(ValidateInput), message: "m_tooltipDataProviderObject must not be null and must be an ITooltipDataProvider.")] 
#endif
        [SerializeField] protected MonoBehaviour m_tooltipDataProviderObject;

        private ITooltipDataProvider m_tooltipDataProvider;

        private Coroutine m_tooltipDelay = null;

#if UNITY_EDITOR
        private bool ValidateInput()
        {
            return m_tooltipDataProviderObject != null && m_tooltipDataProviderObject is ITooltipDataProvider;
        }
#endif

        private void Awake()
        {
            m_tooltipDataProvider = m_tooltipDataProviderObject as ITooltipDataProvider;
        }

        private void OnDisable()
        {
            if(TooltipManager.CurrentTooltipData == m_tooltipDataProvider) {
                StopDelayTimer();
                HideTooltip();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            StopDelayTimer();

            m_tooltipDelay = StartCoroutine(ToolTipDelay(TooltipManager.Instance.TooltipDelay));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopDelayTimer();
            HideTooltip();
        }

        public void StopDelayTimer()
        {
            if(m_tooltipDelay != null) {
                StopCoroutine(m_tooltipDelay);
            }

            m_tooltipDelay = null;
        }

        public void ShowTooltip()
        {
            TooltipManager.Instance.SetCurrentTooltip(m_tooltipDataProvider);
        }

        public void HideTooltip()
        {
            TooltipManager.Instance.UnsetTooltip(m_tooltipDataProvider);
        }

        private IEnumerator ToolTipDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            ShowTooltip();
            m_tooltipDelay = null;
        }
    }
}
