using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New Hero Color", menuName = "Game/Hero Color")]
    public class HeroColor : ScriptableObject
    {
        [field: SerializeField] public Color Primary { get; private set; } = Color.white;
        [field: SerializeField] public Color Secondary { get; private set; } = Color.black;
    } 
}
