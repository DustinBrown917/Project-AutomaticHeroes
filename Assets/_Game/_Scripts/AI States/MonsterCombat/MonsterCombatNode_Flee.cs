using Game.Flow;
using UnityEngine;

namespace Game.Monsters.Combat
{
    public class MonsterCombatNode_Flee : MonsterCombatNode
    {
        public class State : FlowNodeState
        {
            public int currentStage = 1;
            public float nextStageTime;

            public State(MonsterCombatNode node) : base(node)
            {
            }
        }

        [Input] public FlowPort entry;
        [Output] public FlowPort exit;

        [SerializeField] private string[] m_stage1Flavour;
        [SerializeField] private string[] m_stage2Flavour;
        [SerializeField] private string[] m_stage3Flavour;

        public override IFlowNodeState Enter(IFlowState bb)
        {
            if (bb is not IMonsterCombatState state)
                throw new System.ArgumentException($"Must be of type {nameof(IMonsterCombatState)}.", nameof(bb));

            State newState = new State(this);
            state.Result = IMonsterCombatState.ECombatResult.Fled;
            newState.nextStageTime = GetNextStageTime(state);
            UpdateFlavourForStage(state, newState.currentStage);
            bb.Hero.Equipment.GetMainWeaponSkill().GainExperience(state.Monster.CalculateExperienceReward(bb.Hero) * 0.5f);
            return newState;
        }

        public override void OnWorldTimeUpdate(IFlowState bb, float deltaSeconds)
        {
            if (bb.NodeState is not State state)
                return;

            if(bb.ElapsedTime >= state.nextStageTime) {
                state.currentStage++;
                state.nextStageTime = GetNextStageTime(bb);
                UpdateFlavourForStage((IMonsterCombatState)bb, state.currentStage);
            }

            if(state.currentStage >= 4) {
                state.Conclude(exit.ConnectedPortData?.Node);
            }
        }

        public override FlowPort GetFlowPort(XNode.NodePort port)
        {
            switch (port.fieldName) {
                case nameof(entry): return entry;
                case nameof(exit): return exit;
                default: return null;
            }
        }

        private float GetNextStageTime(IFlowState bb)
        {
            return bb.ElapsedTime + bb.WorldState.Rng.Range(1200f, 3600f);
        }

        private void UpdateFlavourForStage(IMonsterCombatState bb, int stage)
        {
            string[] flavours = GetFlavoursForStage(stage);
            bb.FlavourText = string.Format(bb.WorldState.HeroFormatProvider, bb.WorldState.Rng.GetRandom(flavours), bb.Hero, bb.Monster);
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
