using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.EditorFriendly
{
    [System.Serializable]
    public struct EditorFriendlyKVP<TKey, TValue>
    {
        public TKey key;
        public TValue value;
    }
}
