using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SegoLib.UI
{
    public class FloatField : MonoBehaviour
    {
        [System.Serializable]
        public class OnValueUpdatedEvent : UnityEvent<float> { }
        [SerializeField] private Vector2 m_range = new Vector2(float.MinValue, float.MaxValue);

        [SerializeField] private TMP_InputField m_inputField;
        [field: SerializeField] public OnValueUpdatedEvent OnValueUpdated { get; private set; }

        public void TextChanged(string text)
        {
            float input = GetValue();
            input = ValidateValue(input);
            string newText = input.ToString();
            
            if(text != newText) {
                m_inputField.SetTextWithoutNotify(newText);
            }

            OnValueUpdated?.Invoke(input);
        }

        public void SetValue(float value)
        {
            m_inputField.text = value.ToString();
        }

        public float GetValue()
        {
            if(float.TryParse(m_inputField.text, out float result)) {
                return result;
            }
            return 0f;
        }

        public void SetMinMax(float min, float max)
        {
            if(min >= max) {
                Debug.LogError($"Min value must be less than Max! Min: {min}, Max: {max}");
                return;
            }

            m_range.x = min;
            m_range.y = max;

            m_inputField.text = ValidateValue(GetValue()).ToString();
        }

        private float ValidateValue(float value)
        {
            return Mathf.Clamp(value, m_range.x, m_range.y);
        }
    }
}
