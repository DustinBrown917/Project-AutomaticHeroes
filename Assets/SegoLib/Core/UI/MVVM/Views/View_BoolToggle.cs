using UnityEngine;
using UnityEngine.Events;

namespace SegoLib.UI.MVVM
{
    public class View_BoolToggle : View_Base
    {
        [System.Serializable]
        public class PropertyChangedEvent : UnityEvent<bool> { }

        [field: SerializeField] public PropertyChangedEvent ValueChanged { get; private set; }
        [field: SerializeField] public PropertyChangedEvent ValueChangedInverse { get; private set; }

        protected override void UpdateFromTargetProperty()
        {
            bool value = (bool)m_targetProperty.GetValue(m_observedViewModel);
            ValueChanged?.Invoke(value);
            ValueChangedInverse?.Invoke(!value);
        }
    }

}