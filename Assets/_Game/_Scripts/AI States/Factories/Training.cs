using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class Training : HeroBehaviour
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        [System.Serializable]
        public class Flavour
        {
            public string flavorText;

            public WeaponTypeDef[] mustBeWieldingWeaponType;

            public bool WeaponSuitsFlavour(IWeapon weapon)
            {
                if (mustBeWieldingWeaponType == null || mustBeWieldingWeaponType.Length == 0)
                    return true;

                for(int i = 0; i < mustBeWieldingWeaponType.Length; i++) {
                    if (mustBeWieldingWeaponType[i] == weapon.WeaponType)
                        return true;
                }

                return false;
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== State ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        protected class State : IHeroBehaviour.State
        {
            public float nextFlavourUpdate;

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
            public List<Flavour> flavours = new();

            public string stateString = "Training {0}.";

            public Sprite backgroundSprite;

            public int TrainerLevel
            {
                get => m_trainerLevel;
                set => m_trainerLevel = ValidateTrainerLevel(value);
            }
            [SerializeField] private int m_trainerLevel = 1;

            public HeroSkillDef SkillDef
            {
                get => m_skillDef;
                set {
                    if (value == null)
                        return;
                    m_skillDef = value;
                }
            }
            [SerializeField] private HeroSkillDef m_skillDef;

            private float m_trainingDuration = 4 * 3600;
            private float m_expPerSecond = WorldTime.PerHourToPerSecond(1.5f);

            public Behaviour(IHeroBehaviour parentBehaviour) : base(parentBehaviour) { }

            protected override State CreateNewState(Hero hero)
            {
                TrainerLevel = m_trainerLevel; // Forces validation of trainer level.
                State state = new State(ParentBehaviour, WorldState.Instance, hero);
                state.StateString = string.Format(stateString, SkillDef.DisplayName);
                state.BackgroundSprite = backgroundSprite;
                state.ForegroundSprite = SkillDef.DisplaySprite;
                return state;
            }


            protected override void OnWorldTimeUpdate(State bb, float deltaSeconds)
            {
                if (!bb.Hero.Stamina.TrySpendEnergy(deltaSeconds * HeroStamina.TRAINING_STAMINA_PER_SECOND)) {
                    bb.Succeed();
                } else {
                    bb.Hero.Skills[SkillDef].GainExperience(deltaSeconds * m_expPerSecond);

                    if (bb.ElapsedTime >= bb.nextFlavourUpdate) {
                        bb.SetFlavour(GetNewFlavourState(bb));
                        bb.nextFlavourUpdate = bb.ElapsedTime + 4800f;
                    }
                }

                if (bb.ElapsedTime >= m_trainingDuration)
                    bb.Succeed();
            }


            private Flavour GetNewFlavourState(State bb)
            {
                List<Flavour> validFlavours = UnityEngine.Pool.ListPool<Flavour>.Get();
                Flavour chosenFlavour;

                foreach(Flavour flavour in flavours) {
                    if (flavour.WeaponSuitsFlavour(bb.Hero.Equipment.MainWeapon))
                        validFlavours.Add(flavour);
                }

                if (validFlavours.Count > 0)
                    chosenFlavour = bb.WorldState.Rng.GetRandom(validFlavours);
                else
                    chosenFlavour = DefaultFlavour;

                UnityEngine.Pool.ListPool<Flavour>.Release(validFlavours);
                return chosenFlavour;
            }

            private int ValidateTrainerLevel(int value)
            {
                if (value < 1)
                    value = 1;
                return value;
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Behaviour ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        private static Flavour DefaultFlavour { get; } = new() { flavorText = string.Empty };

        [SerializeField] private Behaviour m_behaviour;
        public List<Flavour> Flavours => m_behaviour.flavours;
        public string StateString { get => m_behaviour.stateString; set => m_behaviour.stateString = value; }
        public Sprite BackgroundSprite { get => m_behaviour.backgroundSprite; set => m_behaviour.backgroundSprite = value; }
        public HeroSkillDef SkillDef { get => m_behaviour.SkillDef; set => m_behaviour.SkillDef = value; }

        public Training()
        {
            m_behaviour = new Behaviour(this);
        }

        protected override IHeroBehaviour.IBehaviour InitializeBehaviour()
        {
            if(m_behaviour.SkillDef == null) {
                m_behaviour.SkillDef = HeroSkillDef.All.Items[0];
            }

            return m_behaviour;
        }

        public override bool CanHeroDoBehaviour(Hero hero)
        {
            return true;
        }

        public override float GetPriority(Hero hero)
        {
            return hero.Brain.BasePriorities.Training * hero.Stamina.EnergyPercent;
        }
    }
}
