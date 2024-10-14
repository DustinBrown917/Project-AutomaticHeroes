using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Inn : MonoBehaviour, IHeroStateProvider
    {

        [SerializeField] private Rest m_rest;


        public void GetStates(List<IStateFactory> container)
        {
            container.Add(m_rest);
        }
    } 
}
