using Game.Monsters.Combat;
using UnityEngine;

namespace Game.AI.General
{
    /// <summary>
    /// The monster from the most recent fight in this behaviour, or null if there hasn't been one yet.
    /// </summary>
    [CreateAssetMenu(fileName = "New Last Combat Monster Data Source", menuName = "Game/AI/Data Sources/Last Combat Monster")]
    public class BehaviourDataSource_LastCombatMonster : BehaviourDataSource
    {
        public override object GetData(IHeroBehaviour.IState bb)
        {
            if (bb.TryGetInjectedData(InjectedDataIdentities.PreviousCombatSummary, out MonsterCombatSummary lastCombat))
                return lastCombat.monster;

            return null;
        }
    }
}
