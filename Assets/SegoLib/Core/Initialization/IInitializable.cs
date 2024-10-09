using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib
{
    public interface IInitializable 
    {
        public int InitPriority { get; }
        public bool Initialize();
    } 
}
