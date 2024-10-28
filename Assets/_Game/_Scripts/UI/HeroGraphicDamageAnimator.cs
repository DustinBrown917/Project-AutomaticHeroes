using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class HeroGraphicDamageAnimator : MonoBehaviour
    {
        [SerializeField] private VM_Hero m_heroVM;
        [SerializeField] private Graphic m_targetGraphic;
        [SerializeField] private Gradient m_damageGradient;
        [SerializeField] private float m_animDuration;
        [SerializeField] private Vector3 m_punchPosition;

        private Hero m_currentlyMonitoredHero;

        private Tween m_positionTween;
        private Tween m_colorTween;

        private float m_currentDamageGradientT = 1.0f;

        public void Start()
        {
            m_heroVM.ModelChanged += VMModelChanged;
            m_currentlyMonitoredHero = m_heroVM.Model;
            if (m_currentlyMonitoredHero != null)
                SubscribeToEvents();


            m_positionTween = m_targetGraphic.transform.DOPunchPosition(m_punchPosition, m_animDuration).SetAutoKill(false).Pause();
            m_colorTween = DOTween.To(GetCurrentDamageGradientT, SetCurrentDamageGradientT, 1.0f, m_animDuration).SetAutoKill(false).Pause();

        }

        private void OnDestroy()
        {
            if (m_currentlyMonitoredHero != null)
                UnsubscribeFromEvents();

            m_currentlyMonitoredHero = null;

            if (m_heroVM != null)
                m_heroVM.ModelChanged -= VMModelChanged;

            m_heroVM = null;
        }

        private float GetCurrentDamageGradientT()
        {
            return m_currentDamageGradientT;
        }

        private void SetCurrentDamageGradientT(float t)
        {
            m_currentDamageGradientT = t;
            m_targetGraphic.color = m_damageGradient.Evaluate(t);
        }

        private void VMModelChanged(SegoLib.UI.MVVM.VM_Base<Hero> vm, Hero newHero)
        {
            if(m_currentlyMonitoredHero != null) {
                UnsubscribeFromEvents();
            }

            m_currentlyMonitoredHero = newHero;

            if(m_currentlyMonitoredHero != null) {
                SubscribeToEvents();
            }
        }

        private void SubscribeToEvents()
        {
            m_currentlyMonitoredHero.Health.currentChanged += OnHeroHealthChanged;
        }

        private void UnsubscribeFromEvents()
        {
            m_currentlyMonitoredHero.Health.currentChanged -= OnHeroHealthChanged;
        }

        private void OnHeroHealthChanged(HeroHealth source, float oldValue, float newValue)
        {
            if(newValue < oldValue) {
                SetCurrentDamageGradientT(0.0f);
                m_positionTween.Restart();
                m_colorTween.Restart();
            }
        }
    } 
}
