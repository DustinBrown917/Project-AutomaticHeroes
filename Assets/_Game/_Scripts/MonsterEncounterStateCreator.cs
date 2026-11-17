using SegoLib;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Offers a behaviour that fights monsters drawn from this location's monster banks.
    /// <para>
    /// Whether the fight is practice or deadly is up to the graph: sparring at a fort and hunting werewolves on the plains are
    /// the same creator, with different graphs, banks and weights.
    /// </para>
    /// </summary>
    public class MonsterEncounterStateCreator : HeroBehaviourStateCreator
    {
        [System.Serializable]
        private class MonsterBankIdPair
        {
            [field: SerializeField] public string id { get; private set; } = "default";
            [field: SerializeField] public MonsterBank Bank { get; private set; }
        }

        [SerializeField] private MonsterBankIdPair[] m_monsterBanks;

        [Tooltip("Scales how appealing this encounter is. The hero's favour for their main weapon's skill is multiplied by this.")]
        [SerializeField, Min(0)] private float m_utilityWeight = 1.0f;

        protected override (Identity key, object value)[] GetInjectedData(WorldState worldState, Hero hero)
        {
            MonsterProviderPool monsterProviders = new();

            for(int i = 0; i < m_monsterBanks.Length; i++) {
                monsterProviders.SetProvider(m_monsterBanks[i].id, m_monsterBanks[i].Bank);
            }

            return new (Identity, object)[] { (InjectedDataIdentities.MonsterProviderPool, monsterProviders) };
        }

        public override bool CanDo(WorldState worldState, Hero hero)
        {
            return hero.Heuristics.IsCurrentHealthHigh() && hero.Heuristics.IsExhaustionLow();
        }

        public override float Utility(WorldState worldState, Hero hero)
        {
            return hero.Skills.GetFavour(hero.Equipment.GetMainWeaponSkill().Def) * m_utilityWeight;
        }
    }

}
