using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.Random
{
    /// <summary>
    /// An object that has probability mass for given type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IProbabilityMassProvider<T>
    {
        /// <summary>
        /// Get how common <paramref name="item"/> is among this object's random output.
        /// <para>1 means the item is the most probable item with lower numbers representing lesser probability when compared to the most probable item. 0 means the item will not appear at all.</para>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public float GetProbabilityMass(T item);
    } 
}
