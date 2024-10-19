using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Inn : MonoBehaviour, IHeroBehaviourProvider
    {

        [SerializeField] private Rest m_rest;


        public void GetStates(List<IHeroBehaviour> container)
        {
            container.Add(m_rest);
        }
    } 
}
