using SegoLib.Random;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Explore : StateFactory<Explore.State>
    {
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public class State : HeroState
        {
            private IEvent m_currentEvent = null;

            private IReadOnlyList<IEvent> m_events;
            private float m_sumOfEventWeights;

            private string m_defaultFlavour;

            public float ElapsedTime { get; private set; }
            public float NextEventTime { get; private set; }
            public float EventTime { get; private set; }

            public State(Explore stateFactory, Sprite sprite, Hero hero, IReadOnlyList<IEvent> events, string defaultFlavour) : base(stateFactory, sprite, hero)
            {
                m_events = events;
                m_defaultFlavour = defaultFlavour;
            
                for(int i = 0; i < events.Count; i++) {
                    m_sumOfEventWeights += events[i].Weight;
                }
            }


            public override void EnterState()
            {
                NextEventTime = GetNextEventTime();
            }

            public override void DoState(float deltaSeconds)
            {
                ElapsedTime += deltaSeconds;

                if(m_currentEvent == null && ElapsedTime >= NextEventTime) {
                    m_currentEvent = GetNewEvent();
                    EventTime = 0;
                    m_currentEvent.ApplyPreEffects(this);
                    UpdateFlavour();
                } else if (m_currentEvent != null) {
                    EventTime += deltaSeconds;
                    m_currentEvent.ApplyRepeatEffects(this, deltaSeconds);
                    if(m_currentEvent.Duration <= EventTime) {
                        m_currentEvent.ApplyPostEffects(this);
                        m_currentEvent = null;
                        NextEventTime = GetNextEventTime();
                    }
                    UpdateFlavour();
                }


                if (ShouldEndState()) {
                    SucceedState();
                }
            }

            public override void ExitState()
            {
                m_currentEvent = null;
            }

            private void UpdateFlavour()
            {
                if (m_currentEvent == null)
                    ActionString = m_defaultFlavour;
                else
                    ActionString = $"{m_defaultFlavour}\n{m_currentEvent.GetFlavor(this)}";
            }

            private float GetNextEventTime()
            {
                return ElapsedTime;
            }

            private IEvent GetNewEvent()
            {
                int index = WeightedRandom.Naive(m_events, m_sumOfEventWeights, WorldState.Instance.Rng, x => x.Weight);
                return m_events[index];
            }

            private bool ShouldEndState()
            {
                return Hero.Health.Percent <= 0.2f || Hero.Stamina.ExhaustionPercent >= 0.6f;
            }
        }


        public interface IEvent
        {
            public float Weight { get; }
            public float Duration { get; }
            public string GetFlavor(State state);
            public void ApplyPreEffects(State state);
            public void ApplyRepeatEffects(State state, float deltaSeconds);
            public void ApplyPostEffects(State state);
        }


        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        protected readonly List<IEvent> m_events = new();

        protected string m_defaultFlavour = "Exploring.";

        public Explore()
        {
            m_events.Add(new WanderingAncientRuin(1));
            m_events.Add(new ExaminingBattlefield(1));
            m_events.Add(new HidingFromTitan(1));
            m_events.Add(new GazingInWonder(1));
            m_events.Add(new Digging(1));
            m_events.Add(new RunningFromThreat(1));
            m_events.Add(new DecipheringTrinket(1));
        }

        public override float GetPriority(Hero hero)
        {
            return hero.Brain.BasePriorities.Combat * hero.Stamina.EnergyPercent * hero.Health.Percent;
        }

        protected override State CreateState_Internal(Hero hero)
        {
            return new State(this, Sprite, hero, m_events, m_defaultFlavour);
        }

        protected override Sprite GetSprite()
        {
            return SpriteLibrary.GetSprite("karate");
        }
    }

    public class WanderingAncientRuin : Explore.IEvent
    {
        public float Weight { get; private set; }

        public float Duration { get; } = 3600;

        public WanderingAncientRuin(float weight)
        {
            Weight = weight;
        }


        public void ApplyPreEffects(Explore.State state)
        {
            
        }

        public void ApplyRepeatEffects(Explore.State state, float deltaSeconds)
        {
            
        }
        public void ApplyPostEffects(Explore.State state)
        {
            
        }

        public string GetFlavor(Explore.State state)
        {
            return "Wandering an ancient ruin.";
        }
    }

    public class ExaminingBattlefield : Explore.IEvent
    {
        public float Weight { get; private set; }

        public float Duration { get; } = 3600;

        public ExaminingBattlefield(float weight)
        {
            Weight = weight;
        }


        public void ApplyPreEffects(Explore.State state)
        {

        }

        public void ApplyRepeatEffects(Explore.State state, float deltaSeconds)
        {

        }
        public void ApplyPostEffects(Explore.State state)
        {

        }

        public string GetFlavor(Explore.State state)
        {
            return "Examining remnants of an ancient battle.";
        }
    }

    public class HidingFromTitan : Explore.IEvent
    {
        public float Weight { get; private set; }

        public float Duration { get; } = 3600;

        public HidingFromTitan(float weight)
        {
            Weight = weight;
        }


        public void ApplyPreEffects(Explore.State state)
        {

        }

        public void ApplyRepeatEffects(Explore.State state, float deltaSeconds)
        {

        }
        public void ApplyPostEffects(Explore.State state)
        {

        }

        public string GetFlavor(Explore.State state)
        {
            return "Hiding from a wandering titan.";
        }
    }

    public class GazingInWonder : Explore.IEvent
    {
        public float Weight { get; private set; }

        public float Duration { get; } = 3600;

        public GazingInWonder(float weight)
        {
            Weight = weight;
        }


        public void ApplyPreEffects(Explore.State state)
        {

        }

        public void ApplyRepeatEffects(Explore.State state, float deltaSeconds)
        {

        }
        public void ApplyPostEffects(Explore.State state)
        {

        }

        public string GetFlavor(Explore.State state)
        {
            return "Gazing in wonder at an old, dead machine.";
        }
    }

    public class Digging : Explore.IEvent
    {
        public float Weight { get; private set; }

        public float Duration { get; } = 3600;

        public Digging(float weight)
        {
            Weight = weight;
        }


        public void ApplyPreEffects(Explore.State state)
        {

        }

        public void ApplyRepeatEffects(Explore.State state, float deltaSeconds)
        {

        }
        public void ApplyPostEffects(Explore.State state)
        {

        }

        public string GetFlavor(Explore.State state)
        {
            return "Diggin up an artifact.";
        }
    }

    public class RunningFromThreat : Explore.IEvent
    {
        public float Weight { get; private set; }

        public float Duration { get; } = 3600;

        public RunningFromThreat(float weight)
        {
            Weight = weight;
        }


        public void ApplyPreEffects(Explore.State state)
        {

        }

        public void ApplyRepeatEffects(Explore.State state, float deltaSeconds)
        {

        }
        public void ApplyPostEffects(Explore.State state)
        {

        }

        public string GetFlavor(Explore.State state)
        {
            return "Fleeing from a swarm of ancient drones.";
        }
    }

    public class DecipheringTrinket : Explore.IEvent
    {
        public float Weight { get; private set; }

        public float Duration { get; } = 3600;

        public DecipheringTrinket(float weight)
        {
            Weight = weight;
        }


        public void ApplyPreEffects(Explore.State state)
        {

        }

        public void ApplyRepeatEffects(Explore.State state, float deltaSeconds)
        {

        }
        public void ApplyPostEffects(Explore.State state)
        {

        }

        public string GetFlavor(Explore.State state)
        {
            return "Trying to understand an ancient trinket.";
        }
    }
}



