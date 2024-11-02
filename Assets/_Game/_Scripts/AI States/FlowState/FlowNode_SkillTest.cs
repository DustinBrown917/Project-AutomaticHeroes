using SegoLib;
using SegoLib.Random;
using System;
using UnityEngine;
using XNode;

namespace Game.Flow
{
    [NodeWidth(300)]
    public class FlowNode_SkillTest : FlowNode
    {
        public class State : FlowNodeState
        {
            public float exitTime;

            public State(FlowNode node) : base(node)
            {
            }
        }


        [Input] public FlowPort entry;

        [field: SerializeField] public HeroSkillDef Skill { get; private set; }
        [field: SerializeField] public int Level { get; private set; }
        [field: SerializeField] public FloatRange TimeToSpendInNode { get; private set; } = new FloatRange(3000, 4200);
        [field: SerializeField] public string[] Flavour { get; private set; } = Array.Empty<string>();

        [Output] public FlowPort success;
        [Output] public FlowPort failure;

        public override IFlowNodeState Enter(IFlowState bb)
        {
            State newState = new(this);
            newState.exitTime = bb.ElapsedTime + bb.WorldState.Rng.Range(TimeToSpendInNode);
            bb.FlavourText = bb.WorldState.Rng.GetRandom(Flavour);
            return newState;
        }

        public override FlowPort GetFlowPort(NodePort port)
        {
            switch (port.fieldName) {
                case nameof(entry): return entry;
                case nameof(success): return success;
                case nameof(failure): return failure;
                default: return null;
            }
        }

        public override void OnWorldTimeUpdate(IFlowState bb, float deltaSeconds)
        {
            if (bb.NodeState is not State state)
                return;

            if(bb.ElapsedTime >= state.exitTime) {
                if (HeroPassesSkillTest(bb)) {
                    OnHeroSucceedsSkillTest(bb, state);
                } else {
                    OnHeroFailsSkillTest(bb, state);
                }
            }
        }

        private bool HeroPassesSkillTest(IFlowState bb)
        {
            return bb.Hero.Skills.Roll(Skill, bb.WorldState) > Level;
        }

        private void OnHeroSucceedsSkillTest(IFlowState bb, State state)
        {
            bb.Hero.Skills[Skill].GainExperience(Level);
            state.Conclude(success.ConnectedPortData.Node);
        }

        private void OnHeroFailsSkillTest(IFlowState bb, State state)
        {
            bb.Hero.Skills[Skill].GainExperience(Level * 0.5f);
            state.Conclude(failure.ConnectedPortData.Node);
        }
    } 
}
