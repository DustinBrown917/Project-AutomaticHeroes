using SegoLib;
using UnityEngine;

namespace Game
{
    public interface IHeroState : SegoLib.IDisposable
    {
        public delegate void StateCompleteDelegate(IHeroState state);
        public delegate void StateCancelledDelegate(IHeroState state);

        public IStateFactory StateFactory { get; }
        public Hero Hero { get; }

        public float Progress { get; }
        public event ValueChangedWithSourceEvent<IHeroState, float> progressChanged;

        public Sprite Sprite { get; }

        public bool StateIsComplete { get; }
        public event StateCompleteDelegate stateCompleted;

        public bool StateIsCancelled { get; }
        public event StateCancelledDelegate stateCancelled;

        public void EnterState();

        public void DoState(float deltaSeconds);

        public void ExitState();

        public void Cancel();

        public string GetActionString();
    }
}
