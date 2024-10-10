using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SegoLib.UI.MVVM
{
    public abstract class VM_Base : View_Base, INotifyPropertyChanged
    {
        public abstract Type ModelType { get; }
        public bool SettingNewModel { get; protected set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public abstract bool SetModel(object model);
        public abstract object GetModel();

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Fallback method that views will use if they fail to find their target property within the VM.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual object GetPropertyValue(string propertyName) { return null; }
        /// <summary>
        /// Fallback backset method that views will use if they fail to find an appropriate backset method within the VM.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="parameter"></param>
        public virtual void RaiseBackset(string propertyName, object parameter) { }
    }

    public abstract class VM_Base<T> : VM_Base
    {
        public override Type ModelType => typeof(T);
        public T Model { get; private set; }
        public event Action<VM_Base<T>, T> ModelChanged;

        /// <summary>
        /// base.Awake must be called if this is overridden. <br/>
        /// base.Awake ensures all values are properly set to a default value.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            SetValuesToDefault();
        }

        /// <summary>
        /// base.Destroy must be called if this is overridden. <br/>
        /// base.Destroy ensures that, if Model is not null, it is properly cleared.
        /// </summary>
        protected virtual void OnDestroy()
        {
            SetModel(null);
        }

        protected override void UpdateFromTargetProperty()
        {
            SetModel(GetPropertyValue<T>());
        }

        public override bool SetModel(object model)
        {
            if(model == null) {
                SetModel((T)model);
                return true;
            }

            if(model is T typedModel) {
                SetModel(typedModel);
                return true;
            }

            return false;
        }

        public void SetModel(T model)
        {
            SettingNewModel = true;

            if (IsEqual(model, Model)) {
                SettingNewModel = false;
                return;
            } else if (Model != null) {
                OnModelChangingFromNotNull();
            } else {
                OnModelChangingFromNull();
            }

            Model = model;

            if (Model != null) {
                OnModelChangedToNotNull();
                UpdateValues();
            } else {
                OnModelChangedToNull();
                SetValuesToDefault();
            }

            ModelChanged?.Invoke(this, Model);
            OnPropertyChanged(nameof(Model));

            SettingNewModel = false;
        }

        protected abstract bool IsEqual(T a, T b);

        protected virtual void OnModelChangingFromNotNull() { }
        protected virtual void OnModelChangingFromNull() { }
        protected virtual void OnModelChangedToNull() { }
        protected virtual void OnModelChangedToNotNull() { }
        protected virtual void SetValuesToDefault() { }
        protected virtual void UpdateValues() { }

        public override object GetModel() => Model;
    }
}
