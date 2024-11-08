using System;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New Tag", menuName = "Game/Tag")]
    public class Tag : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }

        [NonSerialized] private int m_hash;

        private void OnEnable()
        {
            if(Name != null)
                m_hash = Name.GetHashCode();
        }

        public override int GetHashCode()
        {
            return m_hash;
        }
    } 
}
