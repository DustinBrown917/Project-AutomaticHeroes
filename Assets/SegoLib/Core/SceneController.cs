using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SegoLib
{
    public class SceneController : MonoBehaviour
    {
        public static SceneController Current { get; private set; }

        public void ChangeToScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        protected virtual void Awake()
        {
            if(Current == null) {
                Current = this;
            }
        }

        protected virtual void OnDestroy()
        {
            if(Current == this) {
                Current = null;
            }
        }
    } 
}
