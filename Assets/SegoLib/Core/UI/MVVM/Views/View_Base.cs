using NaughtyAttributes;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SegoLib.UI.MVVM
{
    public abstract class View_Base : MonoBehaviour
    {
        [SerializeField] protected VM_Base m_observedViewModel;
#if UNITY_EDITOR
#pragma warning disable CS0414
        [field: SerializeField] private bool m_useDropdown = true;
#pragma warning restore CS0414

        [field: HideIf(nameof(m_useDropdown))]
        [field: OnValueChanged(nameof(OnPropertyNameValueChanged))]
        [field: SerializeField] private string m_propertyName;

        [field: Dropdown(nameof(GetVMProps))]
        [field: ShowIf(nameof(m_useDropdown))]
        [field: OnValueChanged(nameof(OnTargetPropertyNameChanged))]
#endif
        [field: SerializeField] protected string m_targetPropertyName { get; private set; }

        protected PropertyInfo m_targetProperty { get; private set; }
        protected MethodInfo m_backSetMethod { get; private set; }

        protected virtual void Awake()
        {
            GetTargetProperty();
            GetBacksetMethod();
        }

        protected virtual void Start()
        {
            HandleObservedViewModelSet();
        }

#if UNITY_EDITOR
        private string[] GetVMProps()
        {
            if (m_observedViewModel == null) {
                return new string[] { string.Empty };
            }

            PropertyInfo[] propInfo = m_observedViewModel.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            List<string> propNames = new List<string>();

            for (int i = 0; i < propInfo.Length; i++) {
                propNames.Add(propInfo[i].Name);
            }

            return propNames.ToArray();
        }

        [Button]
        private void FindViewModelInParent()
        {
            m_observedViewModel = GetComponentInParent<VM_Base>();
        }

        private void OnPropertyNameValueChanged()
        {
            m_targetPropertyName = m_propertyName;
        }

        private void OnTargetPropertyNameChanged()
        {
            m_propertyName = m_targetPropertyName;
        }
#endif

        private void HandleObservedViewModelSet()
        {
            if (m_observedViewModel == null)
                return;

            m_observedViewModel.PropertyChanged += ViewModelPropertyChanged;
            UpdateFromTargetProperty();
        }

        private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == m_targetPropertyName) {
                UpdateFromTargetProperty();
            }
        }

        private void GetTargetProperty()
        {
            if(m_observedViewModel != null)
                m_targetProperty = m_observedViewModel.GetType().GetProperty(m_targetPropertyName);
        }

        private void GetBacksetMethod()
        {
            if (m_observedViewModel == null || m_targetProperty == null)
                return;

            BackSetAttribute attribute = System.Attribute.GetCustomAttribute(m_targetProperty, typeof(BackSetAttribute)) as BackSetAttribute;

            if(attribute != null) {
                m_backSetMethod = m_observedViewModel.GetType().GetMethod(attribute.backsetMethodName);
            }
        }

        protected T GetPropertyValue<T>()
        {
            return (T)GetPropertyValue();
        }

        protected object GetPropertyValue()
        {
            if (m_targetProperty != null)
                return m_targetProperty.GetValue(m_observedViewModel);
            else
                return m_observedViewModel.GetPropertyValue(m_targetPropertyName);
        }

        protected void SetPropertyValue(object value)
        {
            if (m_backSetMethod != null) {
                m_backSetMethod.Invoke(m_observedViewModel, new object[] { value });
            } else {
                m_observedViewModel.RaiseBackset(m_targetPropertyName, value);
            }
        }

        protected abstract void UpdateFromTargetProperty();
    }
}
