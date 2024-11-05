using Game.Flow;

namespace Game.Monsters.Combat
{
    [NodeWidth(150)]
    public class MonsterCombatNode_Enter : MonsterCombatNode
    {


        [Output] public FlowPort exit;


        public override void Enter(IMonsterCombatState state)
        {
            state.ConcludeCurrentNode(exit.ConnectedPortData?.Node as MonsterCombatNode);
        }

        public override void OnWorldTimeUpdate(IMonsterCombatState bb, float deltaSeconds)
        {
            
        }

        public override FlowPort GetFlowPort(XNode.NodePort port)
        {
            switch (port.fieldName) {
                case nameof(exit): return exit;
                default: return null;
            }
        }

    }
}
