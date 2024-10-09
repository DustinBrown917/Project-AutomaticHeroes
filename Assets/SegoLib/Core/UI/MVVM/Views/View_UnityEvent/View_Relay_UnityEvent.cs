using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SegoLib.UI.MVVM
{
    public class View_Relay_UnityEvent<T> : View_Base
    {
        [System.Serializable]
        public class UnityEventProxy : UnityEvent<T> { }

        [field: SerializeField] public UnityEventProxy OnValueChanged { get; private set; }

        protected override void UpdateFromTargetProperty()
        {
            T value = GetPropertyValue<T>();

            OnValueChanged?.Invoke(value);
        }
    } 
}
