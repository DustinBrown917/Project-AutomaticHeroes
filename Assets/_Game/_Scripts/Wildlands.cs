using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Wildlands : MonoBehaviour, IHeroBehaviourProvider
    {
        [SerializeField] private Cull m_cull = new Cull();

        public void GetStates(List<IStateFactory> container)
        {
            container.Add(m_cull);
        }
    }

}