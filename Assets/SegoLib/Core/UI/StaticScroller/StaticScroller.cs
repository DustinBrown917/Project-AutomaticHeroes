using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SegoLib.UI.MVVM;

namespace SegoLib.UI
{
    public class StaticScroller : MonoBehaviour
    {
        public enum EButtonDisableMode : byte
        {
            Uninteractable,
            Disabled
        }

        protected readonly List<VM_Base> m_cells = new List<VM_Base>();
        protected readonly List<object> m_shownList = new List<object>();

        [SerializeField] protected Transform m_content;
        [SerializeField] protected Button m_scrollUpButton;
        [SerializeField] protected Button m_scrollDownButton;
        [SerializeField] protected Scrollbar m_scrollBar;
        [SerializeField] protected EButtonDisableMode m_buttonDisableMode;
        [SerializeField] protected EButtonDisableMode m_scrollbarDisableMode;

        private int m_currentScrollIndex = 0;
        private int m_scrollIndexMax;



        #region Initialization

        protected virtual void Awake()
        {
            StaticScrollerControls controls = GetComponentInChildren<StaticScrollerControls>();

            if (controls != null) {
                if (m_scrollUpButton == null && controls.DecreaseScrollIndexButton != null) {
                    m_scrollUpButton = controls.DecreaseScrollIndexButton;
                    m_scrollUpButton.onClick.AddListener(() => DecrementScrollIndex(controls.ScrollStep));
                }

                if (m_scrollDownButton == null && controls.IncreaseScrollIndexButton != null) {
                    m_scrollDownButton = controls.IncreaseScrollIndexButton;
                    m_scrollDownButton.onClick.AddListener(() => IncrementScrollIndex(controls.ScrollStep));
                }

                if (m_scrollBar == null && controls.Scrollbar != null) {
                    m_scrollBar = controls.Scrollbar;
                    m_scrollBar.onValueChanged.AddListener(SetScrollIndexNormalized);
                }
            }

            GatherCells();
        }

        public void GatherCells()
        {
            m_cells.Clear();

            foreach (Transform child in m_content) {
                if (TryGetVMFromChild(child, out VM_Base viewModel)) {
                    m_cells.Add(viewModel);
                }
            }
        }

        protected virtual bool TryGetVMFromChild(Transform child, out VM_Base viewModel)
        {
            return child.TryGetComponent(out viewModel);
        }

        #endregion /Initialization





        #region Operation

        public virtual void Show(System.Collections.IEnumerable items)
        {
            m_currentScrollIndex = 0;

            UpdateShownList(items);
            UpdateMaxScrollIndex();

            UpdateView();
        }

        public void UpdateView()
        {

            for (int i = 0; i < m_cells.Count; i++) {
                int itemIndex = m_currentScrollIndex + i;
                if (itemIndex < m_shownList.Count) {
                    m_cells[i].gameObject.SetActive(true);
                    m_cells[i].SetModel(m_shownList[m_currentScrollIndex + i]);
                } else {
                    m_cells[i].SetModel(default);
                    m_cells[i].gameObject.SetActive(false);
                }
            }

            UpdateScrollUpButton();
            UpdateScrollDownButton();
            UpdateScrollBar();
            OnUpdateView();
        }

        protected virtual void UpdateShownList(System.Collections.IEnumerable items)
        {
            m_shownList.Clear();

            if (m_shownList != null) {
                foreach (object item in items) {
                    m_shownList.Add(item);
                }
            }
        }

        protected virtual void OnUpdateView() { }

        public void ClearView()
        {
            for (int i = 0; i < m_cells.Count; i++) {
                m_cells[i].SetModel(default);
            }
        }
        #endregion /Operation




        #region Index Management
        private void UpdateMaxScrollIndex()
        {
            m_scrollIndexMax = m_shownList.Count - m_cells.Count;
            if (m_scrollIndexMax < 0) {
                m_scrollIndexMax = 0;
            }
        }

        public void IncrementScrollIndex(int amount)
        {
            SetScrollIndex(m_currentScrollIndex + amount);
        }

        public void DecrementScrollIndex(int amount)
        {
            SetScrollIndex(m_currentScrollIndex - amount);
        }

        public void SetScrollIndexNormalized(float value)
        {
            SetScrollIndex(Mathf.RoundToInt(value * m_scrollIndexMax));
        }

        public void SetScrollIndex(int index)
        {
            index = Mathf.Clamp(index, 0, m_scrollIndexMax);

            if (index != m_currentScrollIndex) {
                m_currentScrollIndex = index;
                UpdateView();
            }
        }

        #endregion /Index Management



        #region Controls

        private void UpdateScrollUpButton()
        {
            if (m_scrollUpButton == null) {
                return;
            }

            if (m_currentScrollIndex <= 0) {
                switch (m_buttonDisableMode) {
                    case EButtonDisableMode.Disabled:
                        m_scrollUpButton.gameObject.SetActive(false);
                        break;
                    default:
                        m_scrollUpButton.interactable = false;
                        break;
                }

            } else {
                switch (m_buttonDisableMode) {
                    case EButtonDisableMode.Disabled:
                        m_scrollUpButton.gameObject.SetActive(true);
                        break;
                    default:
                        m_scrollUpButton.interactable = true;
                        break;
                }
            }
        }

        private void UpdateScrollDownButton()
        {
            if (m_scrollDownButton == null) {
                return;
            }

            if (m_currentScrollIndex >= m_scrollIndexMax) {
                switch (m_buttonDisableMode) {
                    case EButtonDisableMode.Disabled:
                        m_scrollDownButton.gameObject.SetActive(false);
                        break;
                    default:
                        m_scrollDownButton.interactable = false;
                        break;
                }
            } else {
                switch (m_buttonDisableMode) {
                    case EButtonDisableMode.Disabled:
                        m_scrollDownButton.gameObject.SetActive(true);
                        break;
                    default:
                        m_scrollDownButton.interactable = true;
                        break;
                }
            }
        }

        private void UpdateScrollBar()
        {
            if (m_scrollBar == null) {
                return;
            }

            if (m_scrollIndexMax > 0) {
                m_scrollBar.numberOfSteps = m_scrollIndexMax + 1;
                m_scrollBar.SetValueWithoutNotify((float)m_currentScrollIndex / m_scrollIndexMax);
                m_scrollBar.size = Mathf.Clamp(1.0f / m_scrollBar.numberOfSteps, 0.05f, 1.0f);

                switch (m_scrollbarDisableMode) {
                    case EButtonDisableMode.Disabled:
                        m_scrollBar.gameObject.SetActive(true);
                        break;
                    default:
                        m_scrollBar.interactable = true;
                        break;
                }

            } else {
                m_scrollBar.numberOfSteps = 1;
                m_scrollBar.size = 1.0f;

                switch (m_scrollbarDisableMode) {
                    case EButtonDisableMode.Disabled:
                        m_scrollBar.gameObject.SetActive(false);
                        break;
                    default:
                        m_scrollBar.interactable = false;
                        break;
                }
            }
        } 

        #endregion /Controls
    }

    public abstract class StaticScroller<T> : StaticScroller
    {
        protected override bool TryGetVMFromChild(Transform child, out VM_Base viewModel)
        {
            if(child.TryGetComponent(out VM_Base<T> vm)) {
                viewModel = vm;
                return true;
            }

            viewModel = null;
            return false;
        }
    }
}
