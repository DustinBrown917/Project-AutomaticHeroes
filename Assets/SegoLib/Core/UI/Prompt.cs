using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace SegoLib.UI
{
    public abstract class Prompt : MonoBehaviour
    {
        [System.Serializable]
        public class CancelEvent : UnityEvent { }

        [SerializeField] private TextMeshProUGUI m_titleText;
        [SerializeField] private Button m_acceptButton;
        [SerializeField] private Button m_cancelButton;

        public CancelEvent onCancel;
        public UnityEvent onClosed;
        public UnityEvent onShown;

        protected void Init(string title)
        {
            if (m_titleText != null) {
                m_titleText.text = title;
            }
        }

        public void Accept()
        {
            OnAccept();
            Close();
        }

        protected abstract void OnAccept();

        public void Cancel()
        {
            OnCancelled();
            onCancel?.Invoke();
            Close();
        }

        private void Close()
        {
            ClearListeners();
            OnClose();
            onClosed?.Invoke();
        }

        protected abstract void OnClose();

        protected virtual void ClearListeners()
        {
            onCancel?.RemoveAllListeners();
        }

        protected abstract void OnCancelled();

        public void SetAcceptButtonInteractable(bool interactable)
        {
            m_acceptButton.interactable = interactable;
        }

        public void SetCancelButtonInteractable(bool interactable)
        {
            m_cancelButton.interactable = interactable;
        }
    }

    public abstract class Prompt<T> : Prompt
    {
        [System.Serializable]
        public class AcceptEvent : UnityEvent<T> { }

        public AcceptEvent onAccept;

        protected override void OnAccept()
        {
            onAccept?.Invoke(OnValueAccepted());
        }

        protected abstract T OnValueAccepted();

        protected override void ClearListeners()
        {
            base.ClearListeners();
            onAccept?.RemoveAllListeners();
        }
    }
}
