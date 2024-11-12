using SegoLib;
using System;
using UnityEngine;
using Game.Flow;
using SegoLib.Random;

namespace Game.Monsters.Combat
{
    [RequireNode(typeof(MonsterCombatNode_Enter), typeof(MonsterCombatNode_Exit))]
    [CreateAssetMenu(fileName = "New Monster Combat Graph", menuName = "Game/Graphs/Monster Combat")]
    public class MonsterCombatGraph : FlowGraph
    {
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        private class State : IMonsterCombatState
        {
            public WorldState WorldState { get; private set; }
            public Hero Hero { get; private set; }
            public Monster Monster { get; private set; }
            public MonsterCombat Combat { get; private set; }
            public float ElapsedTime { get; private set; }
            public float NodeEndTime { get; set; }
            public bool CombatIsConcluded { get; private set; }
            public ECombatResult Result { get; set; }

            public string FlavourText
            {
                get => m_flavourText;
                set => SetFlavourText(value);
            }
            private string m_flavourText;
            public event ValueUpdatedWithSourceEvent<IMonsterCombatState, string> FlavourTextUpdated;

            private MonsterCombatNode m_currentNode;

            public bool IsDisposed { get; private set; }
            public event Action<IPrivatelyDisposable> Disposed;


            public State(WorldState worldState, Hero hero, Monster monster, MonsterCombatNode node, IMonsterCombatHeroYieldHandler shouldHeroYield)
            {
                WorldState = worldState;
                Hero = hero;
                Monster = monster;
                Combat = new MonsterCombat(worldState, hero, monster, shouldHeroYield);
                m_currentNode = node;
                m_currentNode.Enter(this);
            }


            public void ConcludeCurrentNode(MonsterCombatNode nextNode)
            {
                m_currentNode = nextNode;

                if (m_currentNode == null)
                    CombatIsConcluded = true;
                 else
                    m_currentNode.Enter(this);
            }

            public void OnWorldTimeUpdate(float deltaSeconds)
            {
                ElapsedTime += deltaSeconds;

                if(m_currentNode != null) {
                    m_currentNode.OnWorldTimeUpdate(this, deltaSeconds);
                }
            }

            public void SetFlavourText(string newValue)
            {
                if (newValue == null)
                    newValue = string.Empty;

                if (newValue == m_flavourText)
                    return;

                m_flavourText = newValue;

                FlavourTextUpdated?.Invoke(this, m_flavourText);
            }

            public void Dispose()
            {
                if(IsDisposed) return;
                IsDisposed = true;

                Combat.Dispose();
                Combat = null;
                Monster = null;
                WorldState = null;
                Hero = null;

                Disposed?.Invoke(this);
                Disposed = null;
            }

            public string Format(string str) => string.Format(str, WorldState, Hero, Monster);

            public MonsterCombatSummary GetSummary()
            {
                return new MonsterCombatSummary(Hero, Monster, ElapsedTime, Result);
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        public enum EHeroYieldCondition : byte
        {
            None,
            Sparring,
            Real
        }


        public static IMonsterCombatHeroYieldHandler GetHeroYieldHandler(EHeroYieldCondition condition)
        {
            switch (condition) {
                case EHeroYieldCondition.None: return new ShouldHeroYield_Default();
                case EHeroYieldCondition.Sparring: return new ShouldHeroYield_Sparring();
                case EHeroYieldCondition.Real: return new ShouldHeroYield_Real();
                default: return new ShouldHeroYield_Default();
            }
        }

        public EHeroYieldCondition heroYieldCondition;
        private MonsterCombatNode_Enter m_entryNode;

        protected override void OnInitialize()
        {
            if(m_entryNode == null) {
                m_entryNode = FindEntryNode();
            }
        }

        public IMonsterCombatState Enter(WorldState worldState, Hero hero, Monster monster)
        {
            State newState = new(worldState, hero, monster, m_entryNode, GetHeroYieldHandler(heroYieldCondition));
            return newState;
        }

        private MonsterCombatNode_Enter FindEntryNode()
        {
            foreach(MonsterCombatNode node in nodes) {
                if(node is MonsterCombatNode_Enter entryNode) {
                    return entryNode;
                }
            }

            return null;
        }

    } 

    public interface IMonsterCombatHeroYieldHandler
    {
        public bool ShouldHeroYield(MonsterCombat combat);
    }

    public class ShouldHeroYield_Sparring : IMonsterCombatHeroYieldHandler 
    {
        public bool ShouldHeroYield(MonsterCombat combat)
        {
            return combat.Hero.Heuristics.IsCurrentHealthLow() || combat.Hero.Heuristics.IsExhaustionHigh();
        }
    }

    public class ShouldHeroYield_Real : IMonsterCombatHeroYieldHandler
    {
        public bool ShouldHeroYield(MonsterCombat combat)
        {
            float balanceOfPower = combat.GetBalanceOfPower();

            // Flip BoP because monster advantage is negative.
            balanceOfPower *= -1f;

            return combat.WorldState.Rng.Bool(balanceOfPower);
        }
    }

    public class ShouldHeroYield_Default : IMonsterCombatHeroYieldHandler
    {
        public bool ShouldHeroYield(MonsterCombat combat) => false;
    }
}
