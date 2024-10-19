using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ExploringHowlingPlains : MonoBehaviour, IHeroBehaviourProvider
    {
        private Explore Explore = new Explore();

        public void GetStates(List<IStateFactory> container)
        {
            container.Add(Explore);
        }
    }

}