using Game.Flow;
using UnityEngine;

namespace Game.Monsters.Combat
{
    public class MonsterCombatNode_Combat : MonsterCombatNode
    {
        [Input] public FlowPort entry;
        [Output] public FlowPort victory;
        [Output] public FlowPort flee;
        [Output] public FlowPort defeat;

        public override void Enter(IMonsterCombatState bb)
        {
            bb.FlavourText = $"Fighting {bb.Combat.Monster}.";
        }

        public override void OnWorldTimeUpdate(IMonsterCombatState bb, float deltaSeconds)
        {
            bb.Combat.Simulate(deltaSeconds);

            bool monsterDefeated = bb.Combat.Monster.CurrentHealth <= 0;
            bool heroDefeated = bb.Combat.Hero.Health.Current <= 0;

            if (monsterDefeated) {
                bb.ConcludeCurrentNode(victory.ConnectedPortData?.Node as MonsterCombatNode);
            } else if (heroDefeated) {
                bb.ConcludeCurrentNode(defeat.ConnectedPortData?.Node as MonsterCombatNode);
            } else if (bb.Combat.HeroIsAttemptingToFlee) {
                bb.ConcludeCurrentNode(flee.ConnectedPortData?.Node as MonsterCombatNode);
            }
        }

        public override FlowPort GetFlowPort(XNode.NodePort port)
        {
            switch (port.fieldName) {
                case nameof(entry): return entry;
                case nameof(defeat): return defeat;
                case nameof(flee): return flee;
                case nameof(victory): return victory;
                default: return null;
            }
        }


    }
}
