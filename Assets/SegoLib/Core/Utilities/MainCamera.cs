using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib
{
    public static class MainCamera
    {
        private static Camera m_camera;

        public static Camera Get()
        {
            if(m_camera == null) {
                m_camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            }

            return m_camera;
        }
    } 
}
