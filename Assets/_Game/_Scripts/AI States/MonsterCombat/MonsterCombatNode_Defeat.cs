using Game.Flow;
using SegoLib.Random;
using UnityEngine;

namespace Game.Monsters.Combat
{
    public class MonsterCombatNode_Defeat : MonsterCombatNode
    {
        public class State : FlowNodeState
        {
            public float stateEndTime;

            public State(MonsterCombatNode node) : base(node)
            {
            }
        }

        [Input] public FlowPort entry;
        [Output] public FlowPort exit;

        [SerializeField] private string[] m_flavourTexts;

        public override IFlowNodeState Enter(IFlowState bb)
        {
            if (bb is not IMonsterCombatState state)
                throw new System.ArgumentException($"Must be of type {nameof(IMonsterCombatState)}", nameof(bb));

            State newState = new State(this);
            state.Result = IMonsterCombatState.ECombatResult.Defeated;
            newState.stateEndTime = bb.ElapsedTime + bb.WorldState.Rng.Range(3600f, 7200f);
            bb.FlavourText = string.Format(bb.WorldState.Rng.GetRandom(m_flavourTexts), bb.Hero, state.Monster);
            bb.Hero.Equipment.GetMainWeaponSkill().GainExperience(state.Monster.GetExpRewardModifiedByHealthLost());
            return newState;
        }

        public override void OnWorldTimeUpdate(IFlowState bb, float deltaSeconds)
        {
            if (bb.NodeState is not State state)
                return;

            if(bb.ElapsedTime >= state.stateEndTime) {
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
    } 
}
