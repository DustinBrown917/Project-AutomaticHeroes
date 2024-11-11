using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New Monster Bank", menuName = "Game/Monster/Monster Population Bank")]
    public class MonsterPopulationBank : MonsterBank, IEnumerable<MonsterPopulation>
    {
        [SerializeField] private MonsterPopulationBank[] m_subBanks;

        [SerializeField] private MonsterPopulation[] m_monsters;

        [NonSerialized] private MonsterPopulation[] m_monstersInternal;

        private float m_sumOfWeights;

        private void OnEnable()
        {
            Dictionary<MonsterDef, float> monsterPopulationPrecursor = UnityEngine.Pool.DictionaryPool<MonsterDef, float>.Get();

            foreach (MonsterPopulation monsterPopulation in this) {

                if (monsterPopulation.MonsterDef == null || monsterPopulation.Density <= 0)
                    continue;

                if (!monsterPopulationPrecursor.TryGetValue(monsterPopulation.MonsterDef, out float weight)) {
                    monsterPopulationPrecursor.Add(monsterPopulation.MonsterDef, monsterPopulation.Density);
                } else {
                    monsterPopulationPrecursor[monsterPopulation.MonsterDef] = weight + monsterPopulation.Density;
                }
            }

            m_sumOfWeights = 0;
            m_monstersInternal = new MonsterPopulation[monsterPopulationPrecursor.Count];
            int index = 0;

            foreach ((MonsterDef monsterDef, float density) in monsterPopulationPrecursor) {
                m_monstersInternal[index] = new MonsterPopulation(monsterDef, density);
                m_sumOfWeights += density;
            }

            UnityEngine.Pool.DictionaryPool<MonsterDef, float>.Release(monsterPopulationPrecursor);
        }


        public override Monster GetMonster(WorldState worldState, Hero hero)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<MonsterPopulation> GetEnumerator()
        {
            if (m_monsters != null) {
                for (int i = 0; i < m_monsters.Length; i++) {
                    yield return m_monsters[i];
                }
            }

            if (m_subBanks != null) {
                for (int i = 0; i < m_subBanks.Length; i++) {

                    if (m_subBanks[i] == null)
                        continue;

                    foreach (MonsterPopulation monsterPopulation in m_subBanks[i]) {
                        yield return monsterPopulation;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
