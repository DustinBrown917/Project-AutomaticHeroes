using DG.Tweening.Plugins;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Monsters.Combat
{
    public class Node_Flee : Node
    {
        public class State : NodeState
        {
            public int currentStage = 1;
            public float nextStageTime;

            public State(Node node) : base(node)
            {
            }
        }

        [Input] public PortData entry;
        [Output] public PortData exit;

        [SerializeField] private string[] m_stage1Flavour;
        [SerializeField] private string[] m_stage2Flavour;
        [SerializeField] private string[] m_stage3Flavour;

        public override INodeState Enter(IState bb)
        {
            State newState = new State(this);
            bb.Result = IState.ECombatResult.Fled;
            newState.nextStageTime = GetNextStageTime(bb);
            UpdateFlavourForStage(bb, newState.currentStage);
            return newState;
        }

        public override void OnWorldTimeUpdate(IState bb, float deltaSeconds)
        {
            if (bb.NodeState is not State state)
                return;

            if(bb.ElapsedTime >= state.nextStageTime) {
                state.currentStage++;
                state.nextStageTime = GetNextStageTime(bb);
                UpdateFlavourForStage(bb, state.currentStage);
            }

            if(state.currentStage >= 4) {
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

        private float GetNextStageTime(IState bb)
        {
            return bb.ElapsedTime + bb.WorldState.Rng.Range(1200f, 3600f);
        }

        private void UpdateFlavourForStage(IState bb, int stage)
        {
            string[] flavours = GetFlavoursForStage(stage);
            bb.FlavourText = string.Format(bb.WorldState.Rng.GetRandom(flavours), bb.Hero, bb.Monster);
        }

        private string[] GetFlavoursForStage(int stage)
        {
            switch(stage) {
                case 0:
                case 1:
                    return m_stage1Flavour;
                case 2:
                    return m_stage2Flavour;
                default:
                    return m_stage3Flavour;
            }
        }
    }
}
