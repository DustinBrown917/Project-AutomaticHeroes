using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class HeroGraphic : HeroComponent
    {
        [SerializeField] private GameObject m_graphicGameObject;
        [SerializeField] private SpriteRenderer m_spriteRenderer;

        public bool Initialized { get; private set; }

        public void Initialize()
        {
            if (Initialized)
                return;

            Initialized = true;
        }

        public void Enable()
        {
            m_graphicGameObject.SetActive(true);
        }

        public void Disable()
        {
            m_graphicGameObject.SetActive(false);
        }
    } 
}
