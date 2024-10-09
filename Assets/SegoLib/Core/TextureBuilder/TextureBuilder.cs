using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.TextureBuilder
{
    public class TextureBuilder : MonoBehaviour
    {
        [field: SerializeField] public TextureBuilderLayer LayerPrefab { get; private set; }
        [SerializeField] private Camera m_camera;
        private List<TextureBuilderLayer> m_layers = new List<TextureBuilderLayer>();
        public RenderTexture RenderTexture { get; private set; }

        public ITextureBuilderLayer this[int index] => GetLayer(index);

        public Vector2Int CurrentDimensions { get; private set; }
        public float OutputTextureScale { get; private set; } = 1;

        [field: SerializeField]
        public Vector3 LayerZOffset { get; private set; }

        public int LayerCount => m_layers.Count;

        public Action<TextureBuilder> onDimensionsUpdated;

        private void Awake()
        {
            RenderTexture = new RenderTexture(128, 128, 16, RenderTextureFormat.ARGB32);
            RenderTexture.name = name + " Texture Builder Render Texture";
            m_camera.targetTexture = RenderTexture;
            CurrentDimensions = new Vector2Int(128, 128);
        }

        public void SetOutputTextureScale(float val)
        {
            if (val <= 0) {
                throw new ArgumentException($"{nameof(val)} must be greater than 0.");
            }

            OutputTextureScale = val;

            UpdateDimensions(CurrentDimensions.x, CurrentDimensions.y);
        }

        public ITextureBuilderLayer AddLayer()
        {
            return InstantiateNewLayer();
        }

        public ITextureBuilderLayer AddLayer(ITextureBuilderLayerPreset preset)
        {
            TextureBuilderLayer layer = InstantiateNewLayer();

            layer.Texture = preset.Texture;
            layer.Color = preset.Color;
            layer.Position = preset.Position;
            layer.DirectSetScale(preset.Scale);
            layer.Rotation = preset.Rotation;

            return layer;
        }

        /// <summary>
        /// Destroys a layer.
        /// </summary>
        /// <param name="layerIndex"></param>
        public void DestroyLayer(int layerIndex)
        {
            if (layerIndex >= 0 && layerIndex < m_layers.Count) {
                m_layers[layerIndex].ResetLayer();
                Destroy(m_layers[layerIndex].gameObject);
                m_layers.RemoveAt(layerIndex);
            }
        }

        /// <summary>
        /// Destroys all layers.
        /// </summary>
        public void DestroyAllLayers()
        {
            while (m_layers.Count > 0) {
                DestroyLayer(0);
            }
        }

        /// <summary>
        /// Resets all layers to a default state.
        /// </summary>
        public void ResetAllLayers()
        {
            for (int i = 0; i < m_layers.Count; i++) {
                m_layers[i].ResetLayer();
            }
        }

        /// <summary>
        /// Set the dimensions of the output texture.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetDimensions(int width, int height)
        {
            UpdateDimensions(width, height);
        }

        /// <summary>
        /// Update the dimensiions of the layers to match the base layer.
        /// </summary>
        /// <param name="texture"></param>
        private void UpdateDimensions(int width, int height)
        {
            CurrentDimensions = new Vector2Int(width, height);

            int renderWidth = Mathf.RoundToInt(width * OutputTextureScale);
            int renderHeight = Mathf.RoundToInt(height * OutputTextureScale);

            if (RenderTexture.width == renderWidth && RenderTexture.height == renderHeight)
                return;

            if (RenderTexture.IsCreated())
                RenderTexture.Release();

            RenderTexture.width = renderWidth;
            RenderTexture.height = renderHeight;
        }

        /// <summary>
        /// Get the texture at a given layer.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ITextureBuilderLayer GetLayer(int index)
        {
            return m_layers[index];
        }

        /// <summary>
        /// Build a single texture out of the layers of this TextureBuilder.
        /// </summary>
        /// <returns></returns>
        public Texture2D Build()
        {
            if (!RenderTexture.IsCreated()) {
                RenderTexture.Create();
            }

            m_camera.Render();

            if (SystemInfo.copyTextureSupport.HasFlag(UnityEngine.Rendering.CopyTextureSupport.RTToTexture))
                return RenderTextureToTexture2D_GraphicsCopy(RenderTexture);
            else
                return RenderTextureToTexture2D(RenderTexture);
        }


        /// <summary>
        /// Copies a <paramref name="sourceTex"/> to a Texture2D.
        /// </summary>
        /// <param name="sourceTex"></param>
        /// <returns></returns>
        private Texture2D RenderTextureToTexture2D(RenderTexture sourceTex)
        {
            Texture2D tex = new Texture2D(sourceTex.width, sourceTex.height, TextureFormat.RGBA32, false);
            RenderTexture.active = sourceTex;
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            tex.Apply(false, true);
            return tex;
        }

        /// <summary>
        /// Copies a <paramref name="sourceTex"/> to a Texture2D.
        /// </summary>
        /// <param name="sourceTex"></param>
        /// <returns></returns>
        private Texture2D RenderTextureToTexture2D_GraphicsCopy(RenderTexture sourceTex)
        {
            Texture2D tex = new Texture2D(sourceTex.width, sourceTex.height, TextureFormat.RGBA32, false);
            Graphics.CopyTexture(sourceTex, tex);
            return tex;
        }

        /// <summary>
        /// Create a new layer.
        /// </summary>
        private TextureBuilderLayer InstantiateNewLayer()
        {
            GameObject newLayerGO = Instantiate(LayerPrefab.gameObject, transform);
            TextureBuilderLayer layer = newLayerGO.GetComponent<TextureBuilderLayer>();
            m_layers.Add(layer);
            newLayerGO.transform.localPosition = LayerZOffset * m_layers.Count;
            newLayerGO.transform.localScale = m_layers[0].transform.localScale;
            newLayerGO.gameObject.SetActive(true);

            return layer;
        }
    }

}



