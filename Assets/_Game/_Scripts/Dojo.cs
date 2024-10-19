using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Dojo : MonoBehaviour, IHeroBehaviourProvider
    {
        [SerializeField] private Training m_trainingState;

        public void GetStates(List<IHeroBehaviour> container)
        {
            container.Add(m_trainingState);
        }
    }
}