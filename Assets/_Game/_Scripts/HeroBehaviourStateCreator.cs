using System.Collections.Generic;
using UnityEngine;
using Game.AI.General;
using SegoLib;
using System;
using XNode;

namespace Game
{
    public class HeroBehaviourStateCreator : SceneGraph<HeroBehaviourGraph>, IHeroBehaviourStateCreator, IHeroBehaviourProvider
    {
        [SerializeField] private string m_name;

        public IHeroBehaviour Behaviour => graph;

        [SerializeField] private string m_overrideStateText;
        [SerializeField] private Sprite m_overrideForegroundSprite;
        [SerializeField] private Sprite m_overrideBackgroundSprite;

        public IHeroBehaviour.IState CreateState(WorldState worldState, Hero hero)
        {
            IHeroBehaviour.IState state = graph.GetNewState(worldState, hero, GetInjectedData(worldState, hero));

            if(!string.IsNullOrEmpty(m_overrideStateText)) 
                state.StateText = m_overrideStateText;

            if(m_overrideForegroundSprite != null)
                state.ForegroundSprite = m_overrideForegroundSprite;

            if(m_overrideBackgroundSprite != null)
                state.BackgroundSprite = m_overrideBackgroundSprite;

            OnStateCreated(worldState, hero, state);
            return state;
        }

        public void GetStates(List<IHeroBehaviourStateCreator> container)
        {
            container.Add(this);
        }

        public virtual float Utility(WorldState worldState, Hero hero)
        {
            return 1.0f;
        }
        public virtual bool CanDo(WorldState worldState, Hero hero)
        {
            return true;
        }

        protected virtual (Identity key, object value)[] GetInjectedData(WorldState worldState, Hero hero)
        {
            return Array.Empty<(Identity key, object value)>();
        }

        protected virtual void OnStateCreated(WorldState worldState, Hero hero, IHeroBehaviour.IState createdState)
        {

        }
    }
}
