using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SegoLib.UI.MVVM
{
    public abstract class View_EnumDropdown<T> : View_Base where T : System.Enum
    {
        [SerializeField] protected TMP_Dropdown m_dropdown;

        protected override void Awake()
        {
            base.Awake();
            m_dropdown.options.Clear();
            m_dropdown.AddOptions(new List<string>(System.Enum.GetNames(typeof(T))));
        }

        protected override void UpdateFromTargetProperty()
        {
            string index = GetPropertyValue<T>().ToString();

            for(int i = 0; i < m_dropdown.options.Count; i++) {
                if (m_dropdown.options[i].text == index) {
                    m_dropdown.SetValueWithoutNotify(i);
                    break;
                }
            }
        }

        public void UpdateFromUi(int index)
        {
            SetPropertyValue(System.Enum.GetValues(typeof(T)).GetValue(index));
        }
    }
}
