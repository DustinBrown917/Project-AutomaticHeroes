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
        [NonSerialized] private HeroBehaviourNode_Entry[] m_entries;

        private IHeroBehaviour.IStateManager m_stateManager;

        public event IHeroBehaviour.CreateStateDataInjectionDelegate CreateStateDataInjector;

        protected override void OnInitialize()
        {
            GatherEntryNodes();
            m_stateManager = CreateStateManager();
            WorldState.Instance.Time.AddListener(this);
        }

        protected override void OnDisposed()
        {
            if (WorldState.Instance != null && WorldState.Instance.Time != null) {
                WorldState.Instance.Time.RemoveListener(this);
            }

            m_stateManager = null;
            m_entries = null;
            CreateStateDataInjector = null;
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

        public IHeroBehaviour.IState GetNewState(WorldState worldState, Hero hero)
        {
            if (!IsInitialized)
                Initialize();

            List<(Identity, object)> injectedDataContainer = UnityEngine.Pool.ListPool<(Identity, object)>.Get();

            CreateStateDataInjector?.Invoke(this, injectedDataContainer);

            (Identity key, object value)[] injectedData = injectedDataContainer.ToArray();

            UnityEngine.Pool.ListPool<(Identity, object)>.Release(injectedDataContainer);

            return m_stateManager.GetNewState(worldState, hero, injectedData);
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



        protected abstract IHeroBehaviour.IStateManager CreateStateManager();
        public abstract bool CanHeroDoBehaviour(Hero hero);
        public abstract float GetPriority(Hero hero);
    }

}
