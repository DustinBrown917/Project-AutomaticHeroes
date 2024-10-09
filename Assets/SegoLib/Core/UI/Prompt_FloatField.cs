using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SegoLib.UI
{
    public class Prompt_FloatField : Prompt<float>
    {
        [SerializeField] private FloatField m_floatField;
        private float m_initialValue;

        public void Init(string title, float initialValue, Vector2? range)
        {
            base.Init(title);
            m_initialValue = initialValue;

            if (range.HasValue) {
                m_floatField.SetMinMax(range.Value.x, range.Value.y);
            }

            m_floatField.SetValue(m_initialValue);
        }

        protected override void OnCancelled()
        {
        }

        protected override void OnClose()
        {
            m_initialValue = 0;
            m_floatField.SetMinMax(float.MinValue, float.MaxValue);
        }

        protected override float OnValueAccepted()
        {
            return m_floatField.GetValue();
        }
    }
}
