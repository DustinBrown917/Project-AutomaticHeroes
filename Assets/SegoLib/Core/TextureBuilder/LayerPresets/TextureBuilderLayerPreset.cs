using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.TextureBuilder
{
    [System.Serializable]
    public class LayerPreset : ITextureBuilderLayerPreset
    {
        [field: SerializeField] public Texture2D Texture { get; set; }
        [field: SerializeField] public Color Color { get; set; } = Color.white;
        [field: SerializeField] public Vector2 Position { get; set; } = Vector2.zero;
        [field: SerializeField] public Vector2 Scale { get; set; } = Vector2.one;
        [field: SerializeField] public float Rotation { get; set; } = 0;
    }
}
