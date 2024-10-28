using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Monsters.Combat
{
    public class Node_Defeat : Node
    {
        public class State : NodeState
        {
            public float stateEndTime;

            public State(Node node) : base(node)
            {
            }
        }

        [Input] public PortData entry;
        [Output] public PortData exit;

        [SerializeField] private string[] m_flavourTexts;

        public override INodeState Enter(IState bb)
        {
            State newState = new State(this);
            bb.Result = IState.ECombatResult.Defeated;
            newState.stateEndTime = bb.ElapsedTime + bb.WorldState.Rng.Range(3600f, 7200f);
            bb.FlavourText = string.Format(bb.WorldState.Rng.GetRandom(m_flavourTexts), bb.Hero, bb.Monster);
            return newState;
        }

        public override void OnWorldTimeUpdate(IState bb, float deltaSeconds)
        {
            if (bb.NodeState is not State state)
                return;

            if(bb.ElapsedTime >= state.stateEndTime) {
                state.Conclude(exit.ConnectedPort?.Node);
            }
        }

        protected override PortData GetPortData(XNode.NodePort port)
        {
            switch (port.fieldName) {
                case nameof(entry): return entry;
                case nameof(exit): return exit;
                default: return null;
            }
        }
    } 
}
