using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib
{
    [System.Flags]
    public enum WrapMode2D : byte
    {
        X = 1,
        Y = 2,
        EditorDummy = 4
    }

    [System.Flags]
    public enum WrapMode3D : byte
    {
        X = 1,
        Y = 2,
        Z = 4,
        EditorDummy = 8
    }
}
