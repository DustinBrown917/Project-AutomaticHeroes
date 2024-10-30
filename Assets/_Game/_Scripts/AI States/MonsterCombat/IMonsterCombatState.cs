using Game.Flow;

namespace Game.Monsters.Combat
{
    public interface IMonsterCombatState : IFlowState
    {
        public enum ECombatResult : byte
        {
            Victorious,
            Fled,
            Defeated
        }

        public Monster Monster { get; }
        public ECombatResult Result { get; set; }
    }
}
