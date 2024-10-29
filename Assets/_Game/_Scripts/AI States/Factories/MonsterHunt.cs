using SegoLib;
using SegoLib.Random;
using System.Collections.Generic;
using Game.Flow;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class MonsterHunt : HeroBehaviour
    {
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        [System.Serializable]
        public class MonsterPopulation
        {
            [field: SerializeField] public MonsterDef MonsterDef { get; private set; }
            [field: SerializeField] public float Density { get; private set; }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== State ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        protected class State : IHeroBehaviour.State
        {
            public float nextFightTime;

            public Monsters.Combat.IMonsterCombatState CombatState
            {
                get => m_combatState;
                set => SetCombatState(value);
            }
            private Monsters.Combat.IMonsterCombatState m_combatState;

            public State(IHeroBehaviour behaviour, WorldState worldState, Hero hero) : base(behaviour, worldState, hero) { }

            public void SetCombatState(Monsters.Combat.IMonsterCombatState newState)
            {
                if(m_combatState != null) {
                    m_combatState.flavourTextUpdated -= CombatState_FlavourTextUpdated;
                    m_combatState.Dispose();
                }

                m_combatState = newState;

                if(m_combatState != null) {
                    m_combatState.flavourTextUpdated += CombatState_FlavourTextUpdated;
                }
            }

            public void SetFlavourText(string flavourText)
            {
                FlavourText = flavourText;
            }

            private void CombatState_FlavourTextUpdated(IFlowState source, string newValue)
            {
                SetFlavourText(newValue);
            }

            protected override void OnDisposed()
            {
                CombatState?.Dispose();
                CombatState = null;
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End State ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/



        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Behaviour ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        [System.Serializable]
        protected class Behaviour : IHeroBehaviour.Behaviour<State>
        {
            private const float NEXT_FIGHT_STEP = 2400;

            public Game.Monsters.Combat.MonsterCombatGraph graph;

            public Sprite foregroundSprite;
            public string stateString = "Hunting Monsters.";
            public IntRange monsterLevelRange = new IntRange(1, 10);
            public List<MonsterPopulation> monsters = new();

            public string searchingFlavour = "Searching for monsters.";

            public Behaviour(IHeroBehaviour parentBehaviour) : base(parentBehaviour) { }

            protected override State CreateNewState(Hero hero)
            {
                if (!graph.Initialized)
                    graph.Initialize();

                State state = new State(ParentBehaviour, WorldState.Instance, hero);
                state.StateString = stateString;
                state.ForegroundSprite = foregroundSprite;
                SearchForMonsters(state);
                return state;
            }


            protected override void OnWorldTimeUpdate(State bb, float deltaSeconds)
            {
                // If we're not fighting and it's time to start another fight
                if(bb.CombatState == null && bb.ElapsedTime > bb.nextFightTime) {
                    Monster monster = GetNewMonster(bb);
                    bb.CombatState = graph.Enter(bb.WorldState, bb.Hero, monster);
                }

                if (bb.CombatState != null) { // If we have an active combat, then play it out.

                    bb.CombatState.OnWorldTimeUpdate(deltaSeconds);

                    if (bb.CombatState.Status == IFlowState.EStatus.Concluded) {
                        bb.CombatState = null;

                        if (bb.WorldState.Rng.Bool(GetQuitChance(bb))) {
                            bb.Succeed();
                        } else {
                            SearchForMonsters(bb);
                        }
                    }
                }
            }

            private void SearchForMonsters(State bb)
            {
                bb.nextFightTime = bb.ElapsedTime + NEXT_FIGHT_STEP;
                bb.SetFlavourText(searchingFlavour);
            }

            private Monster GetNewMonster(State bb)
            {
                float sumOfDensities = 0;

                for(int i = 0; i < monsters.Count; i++) {
                    sumOfDensities += monsters[i].Density;
                }

                int index = WeightedRandom.Naive(monsters, sumOfDensities, bb.WorldState.Rng, x => x.Density);

                return new Monster(monsters[index].MonsterDef, bb.WorldState.Rng.Range(monsterLevelRange));
            }

            private float GetQuitChance(State bb)
            {
                return Mathf.Max(1.0f - bb.Hero.Health.Percent, bb.Hero.Stamina.ExhaustionPercent);
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Behaviour ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        [SerializeField] private Behaviour m_behaviour;
        public string StateString { get => m_behaviour.stateString; set => m_behaviour.stateString = value; }
        public Sprite ForegroundSprite { get => m_behaviour.foregroundSprite; set => m_behaviour.foregroundSprite = value; }

        public MonsterHunt()
        {
            m_behaviour = new Behaviour(this);
        }

        protected override IHeroBehaviour.IBehaviour InitializeBehaviour()
        {
            return m_behaviour;
        }

        public override bool CanHeroDoBehaviour(Hero hero)
        {
            return hero.Health.Percent > 0.15f && hero.Stamina.ExhaustionPercent < 0.5f;
        }

        public override float GetPriority(Hero hero)
        {
            return hero.Health.Percent * (1.0f - hero.Stamina.ExhaustionPercent);
        }
    }
}

