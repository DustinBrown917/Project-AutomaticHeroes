using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib
{
    /// <summary>
    /// This object has a sprite that is intended to be used for UI.
    /// </summary>
    public interface IDisplaySprite
    {
        public Sprite DisplaySprite { get; }
    }
}
