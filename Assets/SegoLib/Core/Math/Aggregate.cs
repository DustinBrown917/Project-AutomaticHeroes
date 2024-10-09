using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.Math
{
    public static class Aggregate
    {
        public enum ECalcType : byte
        {
            Sum,
            Mean,
            Min,
            Max,
        }

        public static float Calculate<T>(IEnumerable<T> values, Func<T, float> f, ECalcType calcType)
        {
            switch (calcType) {
                case ECalcType.Sum:
                    return Sum(values, f);
                case ECalcType.Mean:
                    return Mean(values, f);
                case ECalcType.Max:
                    return Max(values, f);
                case ECalcType.Min:
                    return Min(values, f);
            }

            throw new ArgumentException($"Invalid {nameof(ECalcType)} provided: {calcType}.", nameof(calcType));
        }

        public static float Min<T>(IEnumerable<T> values, Func<T, float> f)
        {
            float min = float.MaxValue;

            foreach (T t in values) {
                float newValue = f(t);
                if (newValue < min) min = newValue;
            }

            return min;
        }

        public static int Min<T>(IEnumerable<T> values, Func<T, int> f)
        {
            int min = int.MaxValue;

            foreach (T t in values) {
                int newValue = f(t);
                if (newValue < min) min = newValue;
            }

            return min;
        }

        public static float Max<T>(IEnumerable<T> values, Func<T, float> f)
        {
            float max = float.MinValue;
            foreach (T t in values) {
                float newValue = f(t);
                if (newValue > max) max = newValue;
            }

            return max;
        }

        public static int Max<T>(IEnumerable<T> values, Func<T, int> f)
        {
            int max = int.MinValue;
            foreach (T t in values) {
                int newValue = f(t);
                if (newValue > max) max = newValue;
            }

            return max;
        }

        public static float Mean<T>(IEnumerable<T> values, Func<T, float> f)
        {
            int count = 0;
            float sum = 0;

            foreach (T t in values) {
                sum += f(t);
                count++;
            }

            return sum / count;
        }

        public static float Mean<T>(IEnumerable<T> values, Func<T, int> f)
        {
            float count = 0;
            int sum = 0;

            foreach (T t in values) {
                sum += f(t);
                count++;
            }

            return sum / count;
        }

        public static float Sum<T>(IEnumerable<T> values, Func<T, float> f)
        {
            float sum = 0;
            foreach (T t in values) {
                sum += f(t);
            }

            return sum;
        }

        public static int Sum<T>(IEnumerable<T> values, Func<T, int> f)
        {
            int sum = 0;
            foreach (T t in values) {
                sum += f(t);
            }

            return sum;
        }
    } 
}
