using SegoLib;

namespace Game.Monsters.Combat
{
    public enum ECombatResult : byte
    {
        Victorious,
        HeroYield,
        HeroDowned
    }

    public interface IMonsterCombatState : SegoLib.IDisposable
    {
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
        public string Format(string str);

        public MonsterCombatSummary GetSummary();
    }

    public class MonsterCombatSummary
    {
        public readonly Hero hero;
        public readonly Monster monster;
        public readonly float combatDuration;
        public readonly ECombatResult result;

        public MonsterCombatSummary(Hero hero, Monster monster, float combatDuration, ECombatResult result)
        {
            this.hero = hero;
            this.monster = monster;
            this.combatDuration = combatDuration;
            this.result = result;
        }
    }
}
