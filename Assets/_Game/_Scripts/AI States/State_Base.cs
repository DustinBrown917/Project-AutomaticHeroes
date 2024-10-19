using SegoLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class HeroState : IHeroState
    {



        public void Cancel(int blackboardIndex)
        {
            throw new NotImplementedException();
        }

        public void CancelAllBlackboards()
        {
            throw new NotImplementedException();
        }

        public bool CanHeroDoState(Hero hero)
        {
            throw new NotImplementedException();
        }

        public void DoState(int blackboardIndex, float deltaSeconds)
        {
            throw new NotImplementedException();
        }

        public IHeroState.IBlackboard EnterState()
        {
            throw new NotImplementedException();
        }

        public void ExitState(int blackboardIndex)
        {
            throw new NotImplementedException();
        }

        public float GetPriority(Hero hero)
        {
            throw new NotImplementedException();
        }
    }
}
