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
            public float nextFightTime;

            public MonsterCombat combat;

            public Flavour CurrentFlavour
            {
                get => m_flavour;
            }
            private Flavour m_flavour = DefaultFlavour;

            public bool isFleeing;

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
                // If we're not fighting and it's time to start another fight
                if(bb.combat == null && bb.ElapsedTime > bb.nextFightTime) {
                    if (bb.isFleeing) { // If we're fleeing, stop fleeing and start searching.
                        SearchForMonsters(bb);
                        bb.isFleeing = false;
                    } else { // Otherwise, get a new monster and start fighting it.
                        Monster monster = GetNewMonster(bb);
                        bb.combat = new MonsterCombat(WorldState.Instance, bb.Hero, monster);
                        bb.SetFlavour(new Flavour() { flavorText = $"Fighting {monster}." });
                    }
                }

                if (bb.combat != null) { // If we have an active combat, then play it out.

                    bb.combat?.SimulateStepAndApply(deltaSeconds);

                    if (bb.combat.Monster.CurrentHealth <= 0) { // If we killed the monster, gain exp, dispose the combat, and start search for a new one.
                        float exp = bb.combat.Monster.CalculateExperienceReward(bb.Hero);
                        bb.Hero.Equipment.GetMainWeaponSkill().GainExperience(exp);
                        SearchForMonsters(bb);
                        bb.combat.Dispose();
                        bb.combat = null;
                    } else if (bb.WorldState.Rng.Bool(GetFleeChance(bb) * deltaSeconds)) { // Otherwise, if we decide we can't win this combat, flee.
                        bb.nextFightTime = bb.ElapsedTime + NEXT_FIGHT_STEP;
                        bb.isFleeing = true;
                        bb.SetFlavour(new Flavour() { flavorText = $"Fleeing from {bb.combat.Monster}" });
                        bb.combat.Dispose();
                        bb.combat = null;
                    }
                }

                if(bb.combat == null && !bb.isFleeing) { // If we're not fighting and we're not fleeing, then check if we should 
                    float quitChance = GetQuitChance(bb) * deltaSeconds;

                    if (bb.WorldState.Rng.Bool(quitChance)) {
                        bb.Succeed();
                    }
                }
            }

            private void SearchForMonsters(State bb)
            {
                bb.nextFightTime = bb.ElapsedTime + NEXT_FIGHT_STEP;
                bb.SetFlavour(searchingFlavour);
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
                return Mathf.Max(1.0f - bb.Hero.Health.Percent, bb.Hero.Stamina.ExhaustionPercent) / 1800f;
            }

            private float GetFleeChance(State bb)
            {
                float timeToHeroVictory = bb.combat.GetTimeToHeroVictoryInSeconds();
                float timeToHeroDefeat = bb.combat.GetTimeToMonsterVictoryInSeconds();

                if (Mathf.Abs(timeToHeroVictory - timeToHeroDefeat) < 120f)
                    return 0.1f / 3600f;

                float fleeChancePerHour = timeToHeroVictory < timeToHeroDefeat ?
                    0f :
                    Mathf.Max(1.0f - bb.Hero.Health.Percent, bb.Hero.Stamina.ExhaustionPercent);

                return fleeChancePerHour / 1800f;
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

