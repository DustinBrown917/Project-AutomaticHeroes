using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.Random
{
    [CreateAssetMenu(fileName = "New Perlin Generation Params", menuName = "SegoLib/Random/Perlin Params")]
    public class PerlinNoiseParamsSO : ScriptableObject
    {
        [field: SerializeField] public PerlinNoise.GenerationParameters Parameters { get; set; }
        [field: SerializeField] public FloatRange GeneratedMinMax { get; private set; }
    } 
}
