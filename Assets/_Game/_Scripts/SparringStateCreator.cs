using SegoLib;
using UnityEngine;

namespace Game
{
    public class SparringStateCreator : HeroBehaviourStateCreator
    {
        [System.Serializable]
        private class MonsterBankIdPair
        {
            [field: SerializeField] public string id { get; private set; } = "default";
            [field: SerializeField] public MonsterBank Bank { get; private set; }
        }

        [SerializeField] private MonsterBankIdPair[] m_monsterBanks;

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
            return hero.Skills.GetFavour(hero.Equipment.GetMainWeaponSkill().Def);
        }
    }

}