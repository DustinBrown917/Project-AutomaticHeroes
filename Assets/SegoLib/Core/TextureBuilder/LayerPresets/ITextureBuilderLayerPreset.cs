using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.TextureBuilder
{
    public interface ITextureBuilderLayerPreset
    {
        public Texture2D Texture { get; }
        public Color Color { get; }
        public Vector2 Position { get; }
        public Vector2 Scale { get; }
        public float Rotation { get; }
    }
}
