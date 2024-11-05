using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class HeroGraphic : HeroComponent
    {
        [SerializeField] private GameObject m_graphicGameObject;
        [SerializeField] private SpriteRenderer m_spriteRenderer;

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
