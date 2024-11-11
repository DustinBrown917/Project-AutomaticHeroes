using Game.Flow;
using UnityEngine;
using XNode;

namespace Game.AI.General
{
    [NodeWidth(300)]
    public class HeroBehaviourNode_SkillTest : IHeroBehaviour.Node
    {
        [Input] public FlowPort entry;

        [field: SerializeField] public SkillCheck SkillCheck { get; private set; }
        [field: SerializeField] public float ExpGainMultiplierOnSuccess { get; private set; } = 1.0f;
        [field: SerializeField] public float ExpGainMultiplierOnFailure { get; private set; } = 0.5f;

        [Output] public FlowPort success;
        [Output] public FlowPort failure;


        public override IHeroBehaviour.INode.IState Enter(IHeroBehaviour.IState bb)
        {
            IHeroBehaviour.INode.State state = new();

            if (HeroPassesSkillTest(bb))
                OnHeroSucceedsSkillTest(bb, state);
            else
                OnHeroFailsSkillTest(bb, state);

            return state;
        }

        private bool HeroPassesSkillTest(IHeroBehaviour.IState bb)
        {
            return SkillCheck.HeroPassesCheck(bb.Hero, bb.WorldState, out _, out _);
        }

        private void OnHeroSucceedsSkillTest(IHeroBehaviour.IState bb, IHeroBehaviour.INode.IState state)
        {
            bb.Hero.Skills[SkillCheck.Skill].GainExperience(SkillCheck.Level * ExpGainMultiplierOnSuccess);
            state.Conclude(success.ConnectedPortData.Node as IHeroBehaviour.Node);
        }

        private void OnHeroFailsSkillTest(IHeroBehaviour.IState bb, IHeroBehaviour.INode.IState state)
        {
            bb.Hero.Skills[SkillCheck.Skill].GainExperience(SkillCheck.Level * ExpGainMultiplierOnFailure);
            state.Conclude(failure.ConnectedPortData.Node as IHeroBehaviour.Node);
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

        public override void OnWorldTimeUpdate(IHeroBehaviour.IState bb, IHeroBehaviour.INode.IState node_bb, float deltaSeconds) { }
    } 
}
