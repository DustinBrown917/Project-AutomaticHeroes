using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Wildlands : MonoBehaviour, IHeroBehaviourProvider
    {
        [SerializeField] private MonsterHunt m_monsterHunt;

        public void GetStates(List<IHeroBehaviour> container)
        {
            container.Add(m_monsterHunt);
        }
    } 
}
