using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.UI
{
    public abstract class TooltipContent : MonoBehaviour, IIdentifiable<string>
    {
        public static class Fields
        {
            public const string TITLE = "title";
            public const string DESCRIPTION = "description";
        }

        private RectTransform m_rectTransform = null;
        public RectTransform RectTransform
        {
            get {
                if (m_rectTransform == null) {
                    m_rectTransform = (RectTransform)transform;
                }
                return m_rectTransform;
            }
        }

        [field: SerializeField] public CanvasGroup CanvasGroup;
        [field:SerializeField] public string Id { get; private set; }

        public abstract void SetData(Dictionary<string, object> data);
        public abstract void ClearData();
    }
}
