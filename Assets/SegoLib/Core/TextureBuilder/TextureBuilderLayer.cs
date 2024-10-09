using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class TextureBuilderLayer : MonoBehaviour, ITextureBuilderLayer
{
    private static Color m_DefaultColor { get; } = Color.white;

    private static Texture2D m_defaultTexture;
    private static Texture2D m_DefaultTexture
    {
        get {
            if (m_defaultTexture == null)
                GenerateDefaultTexture();
            return m_defaultTexture;
        }
    }

    [RuntimeInitializeOnLoadMethod]
    private static void GenerateDefaultTexture()
    {
        m_defaultTexture = new Texture2D(4, 4);

        for(int x = 0; x < m_defaultTexture.width; x++) {
            for(int y = 0; y < m_defaultTexture.height; y++) {
                m_defaultTexture.SetPixel(x, y, Color.clear);
            }
        }

        m_defaultTexture.Apply(false, true);
    }


    public Texture2D Texture { get => m_currentTexture; set => SetTexture(value); }
    public Color Color { get => m_currentColor; set => SetColor(value); }
    public Vector2 Position { get => transform.localPosition; set => transform.localPosition = new Vector3(value.x, value.y, transform.localPosition.z); }
    public Vector2 Scale { get => transform.localScale; set => DirectSetScale(value); }
    public float Rotation { get => m_currentRotation; set => SetRotation(value); }

    private float m_currentRotation = 0;

    private Texture2D m_currentTexture;
    [SerializeField] private string m_texturePropertyName = "_MainTex";
    private int m_texturePropertyId;


    private Color m_currentColor;
    [SerializeField] private string m_colorPropertyName = "_Color";
    private int m_colorPropertyId;

    private MeshRenderer m_meshRenderer;

    private MaterialPropertyBlock m_propBlock;



    private void Awake()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_propBlock = new MaterialPropertyBlock();
        m_meshRenderer.GetPropertyBlock(m_propBlock);

        m_texturePropertyId = Shader.PropertyToID(m_texturePropertyName);
        m_colorPropertyId = Shader.PropertyToID(m_colorPropertyName);
    }

    public void SetTexture(Texture2D texture)
    {
        m_currentTexture = texture;
        m_propBlock.SetTexture(m_texturePropertyId, m_currentTexture);
        m_meshRenderer.SetPropertyBlock(m_propBlock);
    }

    public void SetColor(Color color)
    {
        m_currentColor = color;
        m_propBlock.SetColor(m_colorPropertyId, m_currentColor);
        m_meshRenderer.SetPropertyBlock(m_propBlock);
    }


    public void DirectSetScale(Vector2 scale)
    {
        transform.localScale = new Vector3(scale.x, scale.y, 1);
    }

    public void ScaleDimensionsRelativeToSize(int width, int height)
    {
        if (Texture == null)
            return;

        Vector3 currentScale = transform.localScale;
        currentScale.x = Texture.width / (float)width;
        currentScale.y = Texture.height / (float)height;

        transform.localScale = currentScale;
    }

    public void SetScaleRelative(int relativeWidth, int relativeHeight, float scaleX, float scaleY)
    {
        if (Texture == null)
            return;

        Vector3 currentScale = transform.localScale;
        currentScale.x = (Texture.width / (float)relativeWidth) * scaleX;
        currentScale.y = (Texture.height / (float)relativeHeight) * scaleY;

        transform.localScale = currentScale;
    }

    public void SetRotation(float angleInDegrees)
    {
        m_currentRotation = angleInDegrees;
        transform.localRotation = Quaternion.Euler(0, 0, angleInDegrees);
    }

    public void ResetLayer()
    {
        m_propBlock.SetTexture(m_texturePropertyId, m_DefaultTexture);
        m_propBlock.SetColor(m_colorPropertyId, m_DefaultColor);
        m_meshRenderer.SetPropertyBlock(m_propBlock);

        m_currentColor = m_DefaultColor;
        m_currentTexture = m_DefaultTexture;

        transform.localScale = Vector3.one;
        transform.localPosition = new Vector3(0, 0, transform.localPosition.z);

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }

    public void SetActive(bool active) => gameObject.SetActive(active);
}
