using Game.Flow;
using SegoLib;

namespace Game.Monsters.Combat
{
    public interface IMonsterCombatState : SegoLib.IDisposable
    {
        public enum ECombatResult : byte
        {
            Victorious,
            Fled,
            Defeated
        }

        public WorldState WorldState { get; }
        public Hero Hero { get; }
        public Monster Monster { get; }
        public MonsterCombat Combat { get; }
        public string FlavourText { get; set; }
        public event ValueUpdatedWithSourceEvent<IMonsterCombatState, string> FlavourTextUpdated;
        public float ElapsedTime { get; }
        public float NodeEndTime { get; set; }
        public bool CombatIsConcluded { get; }
        public ECombatResult Result { get; set; }

        public void ConcludeCurrentNode(MonsterCombatNode nextNode);
        public void OnWorldTimeUpdate(float deltaSeconds);
    }
}
