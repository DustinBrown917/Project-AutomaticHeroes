using Game.Flow;
using SegoLib;
using SegoLib.Random;
using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace Game.AI.General
{
    [CreateAssetMenu(fileName = "New Hero Behaviour Graph", menuName = "Game/AI/Hero Behaviour Graph")]
    public class HeroBehaviourGraph : HeroBehaviourGraph_Base
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Boundary ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        protected class State : IHeroBehaviour.State
        {
            public State(IHeroBehaviour behaviour, WorldState worldState, Hero hero, IHeroBehaviour.INode entryNode, params (Identity key, object value)[] injectedData) 
                : base(behaviour, worldState, hero, entryNode, injectedData)
            {
            }
        }

        protected class StateManager : IHeroBehaviour.StateManager<HeroBehaviourGraph, State>
        {
            public StateManager(HeroBehaviourGraph parentBehaviour) : base(parentBehaviour)
            {
            }

            protected override State CreateNewState(WorldState worldState, Hero hero, params (Identity key, object value)[] injectedData)
            {
                return new State(ParentBehaviour, worldState, hero, ParentBehaviour.GetEntryNode(worldState, hero));
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Boundary ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        public override bool CanHeroDoBehaviour(Hero hero)
        {
            throw new NotImplementedException();
        }

        public override float GetPriority(Hero hero)
        {
            throw new NotImplementedException();
        }

        protected override IHeroBehaviour.IStateManager CreateStateManager()
        {
            return new StateManager(this);
        }
    }















    [RequireNode(typeof(HeroBehaviourNode_Entry), typeof(HeroBehaviourNode_Exit))]
    public abstract class HeroBehaviourGraph_Base : FlowGraph, IHeroBehaviour
    {
        [SerializeField] private string m_defaultStateText;
        [SerializeField] private Sprite m_defaultForegroundSprite;
        [SerializeField] private Sprite m_defaultBackgroundSprite;
        [SerializeField] private Tag[] m_tagArray;

        [NonSerialized] private HeroBehaviourNode_Entry[] m_entries;

        [NonSerialized] private IHeroBehaviour.IStateManager m_stateManager;

        [NonSerialized] private WorldState m_subscribedWorldState;

        [NonSerialized] private HashSet<Tag> m_tags = new HashSet<Tag>();

        protected override void OnInitialize()
        {
            GatherEntryNodes();
            m_stateManager = CreateStateManager();

            m_tags.UnionWith(m_tagArray);
        }

        protected override void OnDisposed()
        {
            if (m_subscribedWorldState != null && m_subscribedWorldState.Time != null) {
                m_subscribedWorldState.Time.RemoveListener(this);
            }

            m_subscribedWorldState = null;
            m_stateManager = null;
            m_entries = null;
        }

        private void GatherEntryNodes()
        {
            List<HeroBehaviourNode_Entry> entryNodes = UnityEngine.Pool.ListPool<HeroBehaviourNode_Entry>.Get();

            foreach (Node n in nodes) {
                if (n is HeroBehaviourNode_Entry entryNode) {
                    entryNodes.Add(entryNode);
                }
            }

            m_entries = entryNodes.ToArray();

            UnityEngine.Pool.ListPool<HeroBehaviourNode_Entry>.Release(entryNodes);
        }

        public IHeroBehaviour.IState GetNewState(WorldState worldState, Hero hero, params (Identity key, object value)[] injectedData)
        {
            if (!IsInitialized)
                Initialize();

            IHeroBehaviour.IState state = m_stateManager.GetNewState(worldState, hero, injectedData);

            state.StateText = m_defaultStateText;
            state.ForegroundSprite = m_defaultForegroundSprite;
            state.BackgroundSprite = m_defaultBackgroundSprite;

            if(m_subscribedWorldState == null) {
                worldState.Time.AddListener(this);
                m_subscribedWorldState = worldState;
            }

            return state;
        }

        public void OnWorldTimeUpdate(float deltaSeconds)
        {
            m_stateManager.OnWorldTimeUpdate(deltaSeconds);
        }

        public virtual IHeroBehaviour.Node GetEntryNode(WorldState worldState, Hero hero)
        {
            return worldState.Rng.GetRandom(m_entries);
        }

        public void CancelAllStates() => m_stateManager.CancelAllStates();
        public void CancelState(int stateIndex) => m_stateManager.CancelState(stateIndex);

        public bool HasTag(Tag tag)
        {
            return m_tags.Contains(tag);
        }

        protected abstract IHeroBehaviour.IStateManager CreateStateManager();
        public abstract bool CanHeroDoBehaviour(Hero hero);
        public abstract float GetPriority(Hero hero);
    }

}
