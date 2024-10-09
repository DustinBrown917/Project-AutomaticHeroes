using SegoLib.Logging;
using SegoLib.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soma
{
    public class TooltipManager : MonoBehaviour
    {
        public static TooltipManager Instance { get; private set; }

        public static ITooltipDataProvider CurrentTooltipData { get; private set; }

        [SerializeField] private TooltipContainer m_tooltipContainer;
        [SerializeField] private Transform m_tooltipPool;

        [field: SerializeField] public float TooltipDelay { get; private set; }
        [SerializeField] private TooltipContent[] m_tooltipPrefabs;

        private Dictionary<string, TooltipContent> m_tooltips = new Dictionary<string, TooltipContent>();

        private void Awake()
        {
            if(Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            m_tooltipContainer.gameObject.SetActive(false);
            InitTooltips();
        }

        private void InitTooltips()
        {
            for(int i = 0; i < m_tooltipPrefabs.Length; i++) {
                if (m_tooltips.ContainsKey(m_tooltipPrefabs[i].Id)) {
                    Debug.LogWarning($"Duplicate tooltip with Id \"{m_tooltipPrefabs[i].Id}\". Replacing.");
                }

                TooltipContent content = Instantiate(m_tooltipPrefabs[i].gameObject, m_tooltipPool).GetComponent<TooltipContent>();
                m_tooltips[content.Id] = content;
                PoolContent(content);
            }
        }

        public void SetCurrentTooltip(ITooltipDataProvider tooltipData)
        {
            if(tooltipData == null || string.IsNullOrWhiteSpace(tooltipData.TooltipContentType)) {
                return;
            }

            if(!m_tooltips.TryGetValue(tooltipData.TooltipContentType, out TooltipContent content)) {
                Debug.LogError($"Tooltip of type {tooltipData.TooltipContentType} does not exist.");
                return;
            }

            Dictionary<string, object> data = tooltipData.GetTooltipData();

            if (data == null) {
                return;
            }

            if(CurrentTooltipData != null) {
                UnsetCurrentTooltip();
            }

            content.SetData(data);
            CurrentTooltipData = tooltipData;
            m_tooltipContainer.gameObject.SetActive(true);
            m_tooltipContainer.SetTooltipContent(content);
        }

        public void UnsetCurrentTooltip()
        {
            UnsetTooltip(CurrentTooltipData);
        }

        public void UnsetTooltip(ITooltipDataProvider tooltipData)
        {
            if(CurrentTooltipData != tooltipData) {
                return;
            }

            if(m_tooltipContainer.ExtractCurrentTooltipContent(out TooltipContent content)) {
                PoolContent(content);
            }

            CurrentTooltipData = null;
            m_tooltipContainer.gameObject.SetActive(false);
        }

        private void PoolContent(TooltipContent content)
        {
            if (content == null) { return; }
            content.transform.SetParent(m_tooltipPool);
            content.transform.localPosition = Vector3.zero;
            content.ClearData();
        }
    }
}
