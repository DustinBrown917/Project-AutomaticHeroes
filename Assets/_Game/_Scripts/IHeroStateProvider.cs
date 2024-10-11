using System.Collections.Generic;

namespace Game
{
    public interface IHeroStateProvider
    {
        public void GetStates(List<IStateFactory> container);
    } 
}
