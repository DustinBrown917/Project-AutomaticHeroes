using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SegoLib.UI
{
    [System.Obsolete("Use SegoLib.UI.MVVM library instead.")]
    public interface IViewModel
    {
        Type ModelType { get; }
        bool SetModel(object model);
        object GetModel();
    }

    [System.Obsolete("Use SegoLib.UI.MVVM library instead.")]
    public abstract class ViewModel : MonoBehaviour, IViewModel
    {
        public abstract Type ModelType { get; }

        protected virtual void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Raised in Start()
        /// </summary>
        protected virtual void Initialize() { }

        public abstract object GetModel();

        public abstract bool SetModel(object model);
    }

    [System.Obsolete("Use SegoLib.UI.MVVM library instead.")]
    public interface IViewModel<T> : IViewModel
    {
        public struct ModelChangedArgs
        {
            public readonly T oldModel;
            public readonly T newModel;

            public ModelChangedArgs(T oldModel, T newModel)
            {
                this.oldModel = oldModel;
                this.newModel = newModel;
            }
        }

        T Model { get; }

        /// <summary>
        /// Raised when the model changes.
        /// </summary>
        event Action<ViewModel<T>, ModelChangedArgs> onModelChanged;

        /// <summary>
        /// Set the model being displayed by this view model.
        /// </summary>
        /// <param name="newModel"></param>
        void SetModel(T newModel);
    }

    [System.Obsolete("Use SegoLib.UI.MVVM library instead.")]
    public abstract class ViewModel<T> : ViewModel, IViewModel<T>
    {
        public T Model { get; private set; }
        public override Type ModelType => typeof(T);

        /// <summary>
        /// Raised when the model changes.
        /// </summary>
        public event Action<ViewModel<T>, IViewModel<T>.ModelChangedArgs> onModelChanged;

        public bool IsSelected { get; private set; }

        public UnityEvent onSelected;
        public UnityEvent onDeselected;

        /// <summary>
        /// Set the model being displayed by this view model.
        /// </summary>
        /// <param name="newModel"></param>
        public void SetModel(T newModel)
        {
            if (!ObjectIsModel(newModel)) {
                IViewModel<T>.ModelChangedArgs args = new IViewModel<T>.ModelChangedArgs(Model, newModel);

                if (Model != null)
                    DetatchOldModel();

                Model = newModel;

                if (Model != null) {
                    OnNewModelSet();
                    OnUpdateView();
                } else
                    ResetViewModel();

                onModelChanged?.Invoke(this, args);
            }
        }

        /// <summary>
        /// Returns true if <paramref name="obj"/> is Model.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract bool ObjectIsModel(T obj);
        /// <summary>
        /// Unsubscribe from events here. Model will not be null.
        /// </summary>
        protected virtual void DetatchOldModel() { }
        /// <summary>
        /// Subscribe to events here. Model will not be null.
        /// </summary>
        protected virtual void OnNewModelSet() { }
        /// <summary>
        /// Reset all view fields to default as though the model is null.
        /// </summary>
        protected virtual void ResetViewModel() { }
        /// <summary>
        /// Updates all view fields with the current Model's values.
        /// </summary>
        protected virtual void OnUpdateView() { }

        /// <summary>
        /// Toggle on the selected state of this ViewModel.
        /// </summary>
        public virtual void Select()
        {
            if(IsSelected) {
                return;
            }

            IsSelected = true;
            onSelected?.Invoke();
        }

        /// <summary>
        /// Toggle off the selected state of this ViewModel.
        /// </summary>
        public virtual void Deselect()
        {
            if (!IsSelected) {
                return;
            }

            IsSelected = false;
            onDeselected?.Invoke();
        }

        public override bool SetModel(object model)
        {
            if(model is T validModel) {
                SetModel(validModel);
                return true;
            }

            if(model == null) {
                SetModel((T)model);
                return true;
            }
            return false;
        }

        public override object GetModel()
        {
            return Model;
        }
    }

    
}
