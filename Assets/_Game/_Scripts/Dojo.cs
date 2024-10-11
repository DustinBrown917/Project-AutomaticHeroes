using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Dojo : MonoBehaviour, IHeroStateProvider
    {
        private Training_General m_trainingState = new Training_General();

        public void GetStates(List<IStateFactory> container)
        {
            container.Add(m_trainingState);
        }
    }
}