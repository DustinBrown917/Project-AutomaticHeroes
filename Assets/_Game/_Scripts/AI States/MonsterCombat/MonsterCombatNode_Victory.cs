using Game.Flow;
using UnityEngine;

namespace Game.Monsters.Combat
{
    public class MonsterCombatNode_Victory : MonsterCombatNode
    {
        private class State : FlowNodeState
        {
            public float timeToExitNode;

            public State(MonsterCombatNode node) : base(node)
            {
            }
        }

        [Input] public FlowPort entry;
        [Output] public FlowPort exit;

        public override IFlowNodeState Enter(IFlowState bb)
        {
            if (bb is not IMonsterCombatState state)
                throw new System.ArgumentException($"Must be of type {nameof(IMonsterCombatState)}", nameof(bb));

            State newState = new State(this);
            state.Result = IMonsterCombatState.ECombatResult.Victorious;
            newState.timeToExitNode = bb.ElapsedTime + bb.WorldState.Rng.Range(600, 1800);
            bb.FlavourText = GetFlavourText(state);
            bb.Hero.Equipment.GetMainWeaponSkill().GainExperience(state.Monster.CalculateExperienceReward(bb.Hero));

            return newState;
        }

        public override void OnWorldTimeUpdate(IFlowState bb, float deltaSeconds)
        {
            if (bb.NodeState is not State state)
                return;

            if(bb.ElapsedTime >= state.timeToExitNode) {
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

        private string GetFlavourText(IMonsterCombatState bb)
        {
            if(bb.Hero.Health.Percent < 0.2f && bb.Hero.Stamina.ExhaustionPercent > 0.7f) {
                return "Collapsed on the ground, wincing victoriously.";
            } else if (bb.Hero.Health.Percent < 0.2f) {
                return "Assessing the severity of {0:his} wounds.";
            } else if (bb.Hero.Stamina.ExhaustionPercent > 0.7f) {
                return "Lying on the ground, heaving heavily.";
            } else if(bb.Hero.Health.Percent < 0.5f) {
                return "Taking a moment to examine wounds.";
            } else if (bb.Hero.Stamina.ExhaustionPercent > 0.5f) {
                return "Taking a moment to catch {0:his} breath.";
            } else {
                return "Dusting {0:himself} off.";
            }
        }
    } 
}
