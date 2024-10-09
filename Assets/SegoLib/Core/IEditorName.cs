using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib
{
    public interface IEditorName 
    {
#if UNITY_EDITOR
        public string EDITOR_GetName();
#endif
    } 
}
