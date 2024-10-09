using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(Hero))]
    public class HeroComponent : MonoBehaviour
    {
        public Hero Hero { get; private set; }

        protected virtual void Awake()
        {
            Hero = GetComponent<Hero>();
        }

        protected virtual void OnDestroy()
        {
            Hero = null;
        }
    } 
}
