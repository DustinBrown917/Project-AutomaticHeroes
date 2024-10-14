using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Dojo : MonoBehaviour, IHeroStateProvider
    {
        [SerializeField] private Training_General m_trainingState;

        public void GetStates(List<IStateFactory> container)
        {
            container.Add(m_trainingState);
        }
    }
}