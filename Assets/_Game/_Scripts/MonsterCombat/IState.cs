
using SegoLib;
using System;

namespace Game.Monsters.Combat
{

    public interface IState : SegoLib.IDisposable
    {
        public enum ECombatResult : byte
        {
            Victorious,
            Fled,
            Defeated
        }

        public WorldState WorldState { get; }
        public Hero Hero { get; }
        public Monster Monster { get; }
        public string FlavourText { get; set; }
        public event ValueUpdatedWithSourceEvent<IState, string> flavourTextUpdated;
        public bool CombatIsConcluded { get; }
        public ECombatResult Result { get; set; }
        public float ElapsedTime { get; }
        public INodeState NodeState { get; set; }
        public void OnWorldTimeUpdate(float deltaSeconds);
    }
}
