using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SegoLib.UI.MVVM
{
    public abstract class View_UnityEvent<T> : View_Base
    {
        [System.Serializable]
        public class ForwardedUnityEvent : UnityEvent<T> { }

        [field: SerializeField] public ForwardedUnityEvent OnValueChanged { get; private set; }

        protected override void UpdateFromTargetProperty()
        {
            T value = (T)m_targetProperty.GetValue(m_observedViewModel);
            OnValueChanged?.Invoke(value);
        }
    } 
}
