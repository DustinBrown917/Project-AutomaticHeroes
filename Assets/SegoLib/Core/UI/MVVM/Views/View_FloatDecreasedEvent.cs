using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SegoLib.UI.MVVM
{
    public class View_FloatDecreasedEvent : View_Base
    {
        [System.Serializable]
        public class ValueDecreasedEvent : UnityEvent<float, float> { }

        private float m_value = float.MinValue;

        public ValueDecreasedEvent ValueDecreased;

        protected override void UpdateFromTargetProperty()
        {
            float newValue = GetPropertyValue<float>();
            float oldValue = m_value;
            m_value = newValue;
            if(newValue < oldValue) {
                ValueDecreased?.Invoke(newValue, oldValue - newValue);
            }
        }
    }
}
