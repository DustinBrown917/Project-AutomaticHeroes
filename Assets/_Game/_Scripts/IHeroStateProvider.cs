using System.Collections.Generic;

namespace Game
{
    public interface IHeroBehaviourProvider
    {
        public void GetStates(List<IHeroBehaviour> container);
    } 
}
