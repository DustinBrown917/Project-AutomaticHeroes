using UnityEngine;

namespace Game.Monsters.Combat
{
    public class Node_Victory : Node
    {
        private class State : NodeState
        {
            public float timeToExitNode;

            public State(Node node) : base(node)
            {
            }
        }

        [Input] public PortData entry;
        [Output] public PortData exit;

        public override INodeState Enter(IState bb)
        {
            State newState = new State(this);
            bb.Result = IState.ECombatResult.Victorious;
            newState.timeToExitNode = bb.ElapsedTime + bb.WorldState.Rng.Range(600, 1800);
            bb.FlavourText = GetFlavourText(bb);
            bb.Hero.Equipment.GetMainWeaponSkill().GainExperience(bb.Monster.CalculateExperienceReward(bb.Hero));

            return newState;
        }

        public override void OnWorldTimeUpdate(IState bb, float deltaSeconds)
        {
            if (bb.NodeState is not State state)
                return;

            if(bb.ElapsedTime >= state.timeToExitNode) {
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

        private string GetFlavourText(IState bb)
        {
            if(bb.Hero.Health.Percent < 0.2f && bb.Hero.Stamina.ExhaustionPercent > 0.7f) {
                return "Collapsed on the ground, wincing victoriously.";
            } else if (bb.Hero.Health.Percent < 0.2f) {
                return "Assessing the severity of their wounds.";
            } else if (bb.Hero.Stamina.ExhaustionPercent > 0.7f) {
                return "Lying on the ground, heaving heavily.";
            } else if(bb.Hero.Health.Percent < 0.5f) {
                return "Taking a moment to examine wounds.";
            } else if (bb.Hero.Stamina.ExhaustionPercent > 0.5f) {
                return "Taking a moment to catch their breath.";
            } else {
                return "Dusting self off.";
            }
        }
    } 
}
