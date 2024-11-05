using Game.Flow;
using UnityEngine;

namespace Game.Monsters.Combat
{
    public class MonsterCombatNode_Victory : MonsterCombatNode
    {
        [Input] public FlowPort entry;
        [SerializeField] private float m_experienceGainFactor = 1.0f;
        [Tooltip("Exhausted and wounded.")]
        [Output] public FlowPort exit_1;
        [Tooltip("Wounded.")]
        [Output] public FlowPort exit_2;
        [Tooltip("Exhausted.")]
        [Output] public FlowPort exit_3;
        [Tooltip("Somewhat wounded.")]
        [Output] public FlowPort exit_4;
        [Tooltip("Somewhat exhausted.")]
        [Output] public FlowPort exit_5;
        [Tooltip("High health & low exhaustion")]
        [Output] public FlowPort exit_6;


        public override void Enter(IMonsterCombatState bb)
        {
            bb.Result = IMonsterCombatState.ECombatResult.Victorious;
            bb.Hero.Equipment.GetMainWeaponSkill().GainExperience(bb.Combat.Monster.ExpReward * m_experienceGainFactor);
            bb.ConcludeCurrentNode(GetConclusionNode(bb));
        }

        public override void OnWorldTimeUpdate(IMonsterCombatState bb, float deltaSeconds) { }

        public override FlowPort GetFlowPort(XNode.NodePort port)
        {
            switch (port.fieldName) {
                case nameof(entry): return entry;
                case nameof(exit_1): return exit_1;
                case nameof(exit_2): return exit_2;
                case nameof(exit_3): return exit_3;
                case nameof(exit_4): return exit_4;
                case nameof(exit_5): return exit_5;
                case nameof(exit_6): return exit_6;
                default: return null;
            }
        }


        private MonsterCombatNode GetConclusionNode(IMonsterCombatState bb)
        {
            FlowPort port;
            if(bb.Hero.Heuristics.IsCurrentHealthLow() && bb.Hero.Heuristics.IsExhaustionHigh()) {
                port = exit_1;
            } else if (bb.Hero.Heuristics.IsCurrentHealthLow()) {
                port = exit_2;
            } else if (bb.Hero.Heuristics.IsExhaustionHigh()) {
                port = exit_3;
            } else if (!bb.Hero.Heuristics.IsCurrentHealthHigh()) {
                port = exit_4;
            } else if (!bb.Hero.Heuristics.IsExhaustionLow()) {
                port = exit_5;
            } else {
                port = exit_6;
            }

            return port.ConnectedPortData?.Node as MonsterCombatNode;
        }
    } 
}
