using UnityEngine;

namespace SegoLib
{
    /// <summary>
    /// A camera component that will cause the camera to only render frames at the provided framerate.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class RateLimitedCamera : MonoBehaviour
    {
        [field: SerializeField] public float Fps { get; private set; } = 60;
        private Camera m_camera;

        private float m_frameInterval;
        private float m_elapsedTime;

        private void Awake()
        {
            m_camera = GetComponent<Camera>();
            m_frameInterval = 1.0f / Fps;
            m_camera.enabled = false;
        }


        // Update is called once per frame
        void Update()
        {
            m_elapsedTime += Time.deltaTime;
            if (m_elapsedTime >= m_frameInterval) {
                m_camera.Render();
                m_elapsedTime %= m_frameInterval;
            }
        }
    } 
}
