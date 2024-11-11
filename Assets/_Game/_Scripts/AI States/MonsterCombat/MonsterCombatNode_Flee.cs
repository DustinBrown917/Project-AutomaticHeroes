using Game.Flow;
using SegoLib.Random;
using UnityEngine;

namespace Game.Monsters.Combat
{
    public class MonsterCombatNode_HeroYield : MonsterCombatNode
    {
        [Input] public FlowPort entry;
        [SerializeField] private float m_expedrienceGainScale = 1.0f;
        [Output] public FlowPort exit;

        public override void Enter(IMonsterCombatState bb)
        {
            bb.Result = ECombatResult.HeroYield;
            bb.Hero.Equipment.GetMainWeaponSkill().GainExperience(bb.Combat.Monster.GetExpRewardModifiedByHealthLost() * m_expedrienceGainScale);
            bb.ConcludeCurrentNode(exit.ConnectedPortData?.Node as MonsterCombatNode);
        }

        public override void OnWorldTimeUpdate(IMonsterCombatState bb, float deltaSeconds) { }

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
