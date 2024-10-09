using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SegoLib.UI
{
    public class GenericTooltipContent1 : TooltipContent
    {
        [SerializeField] private TextMeshProUGUI m_titleText;
        [SerializeField] private TextMeshProUGUI m_descriptionText;

        public override void ClearData()
        {
            m_titleText.text = "";
            m_descriptionText.text = "";
        }

        public override void SetData(Dictionary<string, object> data)
        {
            if(data.TryGetValue(Fields.TITLE, out object titleObj) && titleObj is string title) {
                m_titleText.text = title;
            }

            if(data.TryGetValue(Fields.DESCRIPTION, out object descriptionObj) && descriptionObj is string description) {
                m_descriptionText.text = description;
            }
        }
    }
}
