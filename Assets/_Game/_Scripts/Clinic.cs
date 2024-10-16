using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Clinic : MonoBehaviour, IHeroStateProvider
    {

        [SerializeField] private TreatWounds m_treatWounds;


        public void GetStates(List<IStateFactory> container)
        {
            container.Add(m_treatWounds);
        }
    } 
}
