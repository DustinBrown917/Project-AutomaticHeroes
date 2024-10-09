using UnityEngine;
using TMPro;


namespace SegoLib.UI.MVVM
{
    [RequireComponent(typeof(TMP_Text))]
    public class View_TextMeshPro : View_Base
    {
        [SerializeField] private TMP_Text m_textMeshPro;
        [SerializeField] private string m_format;
        [SerializeField] private Formatter m_formatter;

        protected override void Awake()
        {
            base.Awake();
            if(m_textMeshPro == null) {
                m_textMeshPro = GetComponent<TMP_Text>();
            }
        }

        protected override void UpdateFromTargetProperty()
        {
            object value = m_targetProperty.GetValue(m_observedViewModel);

            if (m_formatter != null) {
                value = m_formatter.Format(value);
            }

            if (!string.IsNullOrEmpty(m_format)) {
                value = string.Format(m_format, value);
            }

            if(value != null) {
                m_textMeshPro.text = value.ToString();
            } else {
                m_textMeshPro.text = string.Empty;
            }
        }
    } 
}
