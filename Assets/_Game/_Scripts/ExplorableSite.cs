using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ExplorableSite : MonoBehaviour, IHeroBehaviourProvider
    {
        [SerializeField] private Explore m_explore;

        private void Start()
        {
            m_explore.Initialize();
        }

        public void GetStates(List<IHeroBehaviour> container)
        {
            container.Add(m_explore);
        }
    } 
}
