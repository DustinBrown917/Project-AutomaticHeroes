using SegoLib;
using SegoLib.Random;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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
        public class Flavour
        {
            public string flavorText;
        }

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
            public float nextCombatStep;
            public float nextFightTime;

            public Monster monster;

            public Flavour CurrentFlavour
            {
                get => m_flavour;
            }
            private Flavour m_flavour = DefaultFlavour;

            public State(IHeroBehaviour behaviour, WorldState worldState, Hero hero) : base(behaviour, worldState, hero) { }

            public void SetFlavour(Flavour flavourState)
            {
                if (flavourState == null)
                    return;

                m_flavour = flavourState;
                FlavourText = m_flavour.flavorText;
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
            private const float COMBAT_STEP_TIME = 900;
            private const float NEXT_FIGHT_STEP = 2400;

            public Sprite foregroundSprite;
            public string stateString = "Hunting Monsters.";
            public IntRange monsterLevelRange = new IntRange(1, 10);
            public List<MonsterPopulation> monsters = new();

            public Flavour searchingFlavour = new Flavour() { flavorText = "Searching for monsters." };

            public Behaviour(IHeroBehaviour parentBehaviour) : base(parentBehaviour) { }

            protected override State CreateNewState(Hero hero)
            {
                State state = new State(ParentBehaviour, WorldState.Instance, hero);
                state.StateString = stateString;
                state.ForegroundSprite = foregroundSprite;
                state.nextFightTime = NEXT_FIGHT_STEP;
                state.SetFlavour(searchingFlavour);
                return state;
            }


            protected override void OnWorldTimeUpdate(State bb, float deltaSeconds)
            {
                if(bb.monster == null && bb.ElapsedTime > bb.nextFightTime) {
                    bb.monster = GetNewMonster(bb);
                    bb.nextCombatStep = bb.ElapsedTime + COMBAT_STEP_TIME;
                    bb.SetFlavour(new Flavour() { flavorText = $"Fighting {bb.monster}." });
                }

                while(bb.monster != null && bb.ElapsedTime > bb.nextCombatStep && !ShouldQuit(bb)) {
                    Combat.DoCombat(bb.Hero, bb.monster, bb.WorldState);
                    bb.nextCombatStep += COMBAT_STEP_TIME;

                    if(bb.monster.CurrentWoundsRemaining <= 0) {
                        bb.monster = null;
                        bb.nextFightTime = bb.ElapsedTime + NEXT_FIGHT_STEP;
                        bb.SetFlavour(searchingFlavour);
                    }
                }

                if (ShouldQuit(bb)) {
                    bb.Succeed();
                }
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

            private bool ShouldQuit(State bb)
            {
                return bb.Hero.Health.Percent < 0.2f ||
                    bb.Hero.Stamina.ExhaustionPercent > 0.5f;
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Behaviour ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        private static Flavour DefaultFlavour { get; } = new() { flavorText = string.Empty };

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

