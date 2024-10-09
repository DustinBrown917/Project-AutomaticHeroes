using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib
{
    /// <summary>
    /// A utility class to allow coroutines to be started by objects that don't otherwise have access to a MonoBehaviour.
    /// </summary>
    public class CoroutineAnchor : MonoBehaviour
    {
        private static MonoBehaviour m_anchor;
        private static MonoBehaviour Anchor
        {
            get {
                if(m_anchor == null) m_anchor = new GameObject("Coroutine Achor").AddComponent<CoroutineAnchor>();
                return m_anchor;
            }
        }

        public static new Coroutine StartCoroutine(IEnumerator routine)
        {
            return Anchor.StartCoroutine(routine);
        }

        public static new Coroutine StartCoroutine(string methodName)
        {
            return Anchor.StartCoroutine(methodName);
        }

        public static new Coroutine StartCoroutine(string methodName, object value)
        {
            return Anchor.StartCoroutine(methodName, value);
        }

        public static new void StopCoroutine(Coroutine coroutine)
        {
            Anchor.StopCoroutine(coroutine);
        }

        public static new void StopCoroutine(IEnumerator routine)
        {
            Anchor.StopCoroutine(routine);
        }

        public static new void StopCoroutine(string methodName)
        {
            Anchor.StopCoroutine(methodName);
        }

        public static new void StopAllCoroutines()
        {
            Anchor.StopAllCoroutines();
        }


        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    } 
}
