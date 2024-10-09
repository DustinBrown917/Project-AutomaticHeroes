using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SegoLib.UI;

namespace Soma
{
    public class PromptManager : MonoBehaviour
    {
        public static PromptManager Instance { get; private set; }


        [SerializeField] private GameObject m_promptCanvas;
        [SerializeField] private Prompt_FloatField m_floatFieldView;

        private Prompt m_currentPrompt;

        private void Awake()
        {
            if(Instance == null) {
                Instance = this;
            } else {
                Destroy(gameObject);
                return;
            }
        }

        public Prompt_FloatField ShowFloatField(string promptTitle, float initialValue, Vector2? range)
        {
            if(SetCurrentPrompt(m_floatFieldView)) {
                m_floatFieldView.Init(promptTitle, initialValue, range);
                return m_floatFieldView;
            }

            return null;
        }

        private bool SetCurrentPrompt(Prompt prompt)
        {
            if(m_currentPrompt != null) {
                Debug.LogError("Attempting to open new prompt but a prompt is already open. Close current prompt first.");
                return false;
            }

            m_currentPrompt = prompt;
            m_currentPrompt.gameObject.SetActive(true);
            m_currentPrompt.onClosed.AddListener(OnCurrentPromptClosed);
            m_promptCanvas.SetActive(true);

            return true;
        }

        private void OnCurrentPromptClosed()
        {
            m_currentPrompt.gameObject.SetActive(false);
            m_currentPrompt.onClosed.RemoveListener(OnCurrentPromptClosed);
            m_currentPrompt = null;
            m_promptCanvas.SetActive(false);
        }
        
        public void CancelCurrentPrompt()
        {
            if(m_currentPrompt != null) {
                m_currentPrompt.Cancel();
            }
        }
    }
}
