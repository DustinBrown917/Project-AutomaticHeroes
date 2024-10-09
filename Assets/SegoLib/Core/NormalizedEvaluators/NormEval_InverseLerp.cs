using UnityEngine;

namespace SegoLib
{
    /// <summary>
    /// A normalized evaluation that takes in a float value and returns where it falls between A and B.
    /// </summary>
    public class NormEval_InverseLerp : INormalizedEvaluation<float>
    {
        public float A { get; }
        public float B { get; }

        public NormEval_InverseLerp(float a, float b)
        {
            A = a;
            B = b;
        }

        /// <summary>
        /// Find where <paramref name="t"/> lies between this objects A and B properties.
        /// </summary>
        /// <param name="t"></param>
        /// <returns>A value between 0 and 1 proportional to where <paramref name="t"/> lies between A and B. <br/>
        /// If <paramref name="t"/> falls outside of the range of the range made by A and B, 0 or 1 will be returned depending on which side of the range <paramref name="t"/> falls on.</returns>
        public float Evaluate(float t)
        {
            return Mathf.InverseLerp(A, B, t);
        }
    } 
}
