using Game.Flow;
using SegoLib;
using SegoLib.Random;
using UnityEngine;
using XNode;

namespace Game.Monsters.Combat
{
    public class MonsterCombatNode_Flavour : MonsterCombatNode
    {
        [Input] public FlowPort entry;

        [SerializeField] private FloatRange m_nodeDuration = new FloatRange(3000, 4200);
        [SerializeField] private string[] m_flavourTexts = System.Array.Empty<string>();

        [Output] public FlowPort exit;

        public override void Enter(IMonsterCombatState bb)
        {
            bb.FlavourText = bb.Format(bb.WorldState.Rng.GetRandom(m_flavourTexts));
            bb.NodeEndTime = bb.ElapsedTime + bb.WorldState.Rng.Range(m_nodeDuration);
        }

        public override void OnWorldTimeUpdate(IMonsterCombatState bb, float deltaSeconds)
        {
            if (bb.ElapsedTime >= bb.NodeEndTime) {
                bb.ConcludeCurrentNode(exit.ConnectedPortData?.Node as MonsterCombatNode);
            }
        }

        public override FlowPort GetFlowPort(NodePort port)
        {
            switch (port.fieldName) {
                case nameof(entry): return entry;
                case nameof(exit): return exit;
                default: return null;
            }
        }
    }

}