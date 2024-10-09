using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib
{
    /// <summary>
    /// Represents a continuous range between Min and Max values.
    /// </summary>
    [System.Serializable]
    public struct FloatRange
    {
        public static implicit operator Vector2(FloatRange range) => new Vector2(range.m_min, range.m_max);

        [SerializeField] private float m_min;
        [SerializeField] private float m_max;

        /// <summary>
        /// The Min end of the range.
        /// </summary>
        public float Min
        {
            get => m_min;
            set => SetMin(value);
        }

        /// <summary>
        /// The Max end of the range.
        /// </summary>
        public float Max
        {
            get => m_max;
            set => SetMax(value);
        }

        /// <summary>
        /// The difference between Min and Max.
        /// </summary>
        public float Spread => m_max - m_min;

        /// <summary>
        /// The midpoint between Min and Max.
        /// </summary>
        public float Mid => (m_min + m_max) * 0.5f;


        /// <summary>
        /// Constructor. <paramref name="min"/> must be less than or equal to <paramref name="max"/>.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public FloatRange(float min, float max)
        {
            if (min > max)
                throw new System.ArgumentException($"{nameof(min)} must be less than or equal to {nameof(max)}. min: {min}, max: {max}");

            m_min = min;
            m_max = max;
        }

        /// <summary>
        /// Sets the Min and Max values. <paramref name="min"/> must be less than or equal to <paramref name="max"/>.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void Set(float min, float max)
        {
            if (min > max)
                throw new System.ArgumentException($"{nameof(min)} must be less than or equal to {nameof(max)}. min: {min}, max: {max}");

            m_min = min;
            m_max = max;
        }

        /// <summary>
        /// Sets the Min value. If <paramref name="min"/> is greater than Max, Min will be set to Max.
        /// </summary>
        /// <param name="min"></param>
        private void SetMin(float min)
        {
            if (min > m_max)
                m_min = m_max;
            else
                m_min = min;
        }

        /// <summary>
        /// Sets the Max value. If <paramref name="max"/> is less than Min, Max will be set to Min.
        /// </summary>
        /// <param name="min"></param>
        private void SetMax(float max)
        {
            if (max < m_min)
                m_max = m_min;
            else
                m_max = max;
        }

        /// <summary>
        /// Linearly interpolates between Min and Max by <paramref name="t"/>.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public float Lerp(float t)
        {
            return Mathf.Lerp(m_min, m_max, t);
        }

        /// <summary>
        /// Determines where <paramref name="val"/> lies between Min and Max.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public float InverseLerp(float val)
        {
            return Mathf.InverseLerp(m_min, m_max, val);
        }

        /// <summary>
        /// Get a random number between Min and Max, inclusive.
        /// </summary>
        /// <returns></returns>
        public float GetRandom()
        {
            return UnityEngine.Random.Range(m_min, m_max);
        }

        /// <summary>
        /// Returns true if a number is contained within the range of Min to Max, inclusive.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool Contains(float f)
        {
            return f >= m_min && f <= m_max;
        }

        /// <summary>
        /// ToString.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Min: " + m_min + " Max: " + m_max;
        }
    }








    /// <summary>
    /// Represents a discrete range between Min and Max values.
    /// </summary>
    [System.Serializable]
    public struct IntRange
    {
        /// <summary>
        /// Get the overlap between <paramref name="a"/> and <paramref name="b"/> and return it in <paramref name="result"/>.<br/>
        /// If no overlap is found, return false.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetOverlap(IntRange a, IntRange b, out IntRange result)
        {
            if (!a.Overlaps(b)) {
                result = default;
                return false;
            }

            result = new IntRange(Mathf.Max(a.Min, b.Min), Mathf.Min(a.Max, b.Max));
            return true;
        }

        public static implicit operator Vector2Int(IntRange range) => new Vector2Int(range.m_min, range.m_max);

        [SerializeField] private int m_min;
        [SerializeField] private int m_max;

        /// <summary>
        /// The Min end of the range.
        /// </summary>
        public int Min
        {
            get => m_min;
            set => SetMin(value);
        }

        /// <summary>
        /// The Max end of the range.
        /// </summary>
        public int Max
        {
            get => m_max;
            set => SetMax(value);
        }

        /// <summary>
        /// The difference between Min and Max.
        /// </summary>
        public int Spread => m_max - m_min;

        /// <summary>
        /// The midpoint between Min and Max.
        /// </summary>
        public float Mid => (m_min + m_max) * 0.5f;




        /// <summary>
        /// Constructor. <paramref name="min"/> must be less than or equal to <paramref name="max"/>.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public IntRange(int min, int max)
        {
            if (min > max)
                throw new System.ArgumentException($"{nameof(min)} must be less than or equal to {nameof(max)}. min: {min}, max: {max}");

            m_min = min;
            m_max = max;
        }

        /// <summary>
        /// Sets the Min and Max values. <paramref name="min"/> must be less than or equal to <paramref name="max"/>.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void Set(int min, int max)
        {
            if (min > max)
                throw new System.ArgumentException($"{nameof(min)} must be less than or equal to {nameof(max)}. min: {min}, max: {max}");

            m_min = min;
            m_max = max;
        }

        /// <summary>
        /// Sets the Min value. If <paramref name="min"/> is greater than Max, Min will be set to Max.
        /// </summary>
        /// <param name="min"></param>
        private void SetMin(int min)
        {
            if (min > m_max)
                m_min = m_max;
            else
                m_min = min;
        }

        /// <summary>
        /// Sets the Max value. If <paramref name="max"/> is less than Min, Max will be set to Min.
        /// </summary>
        /// <param name="min"></param>
        private void SetMax(int max)
        {
            if (max < m_min)
                m_max = m_min;
            else
                m_max = max;
        }

        /// <summary>
        /// Returns true if a number is contained within the range of Min to Max, inclusive.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool Contains(float val)
        {
            return val >= m_min && val <= m_max;
        }

        /// <summary>
        /// ToString.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Min: " + m_min + " Max: " + m_max;
        }

        /// <summary>
        /// True if <paramref name="other"/> overlaps with this range.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Overlaps(IntRange other)
        {
            return !(other.Min > Max || other.Max < Min);
        }
    }

}