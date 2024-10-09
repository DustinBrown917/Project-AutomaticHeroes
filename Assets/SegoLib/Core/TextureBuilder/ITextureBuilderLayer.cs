using UnityEngine;

public interface ITextureBuilderLayer
{
    public Texture2D Texture { get; set; }
    public Color Color { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Scale { get; set; }

    public void ResetLayer();
    public void SetActive(bool active);
}
