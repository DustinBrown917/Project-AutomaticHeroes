using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib
{
    /// <summary>
    /// Utils specifically pertaining to colors.
    /// </summary>
    public static class ColorUtils
    {
        /// <summary>
        /// Subtracts Color <paramref name="b"/> from Color <paramref name="a"/>.
        /// (a - b)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Color SubtractRBG(Color a, Color b)
        {
            return new Color(a.r - b.r, a.g - b.g, a.b - b.b, a.a);
        }
        


        /// <summary>
        /// The linear distance between the RGB components of two colors.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float DistanceRGB(Color a, Color b)
        {
            return Mathf.Abs(SubtractRBG(a, b).MagRGB());
        }



        /// <summary>
        /// The square distance between the RGB components of two colors.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float DistanceSquareRGB(Color a, Color b)
        {
            return SubtractRBG(a, b).SquareMagRGB();
        }



        /// <summary>
        /// Get <paramref name="num"/> num colors that are as distinct as possible.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static Color[] GetDistinctColors(int num)
        {
            Color[] cols = new Color[num];
            float step = 1.0f / num;
            for (int i = 0; i < num; i++) {
                cols[i] = Color.HSVToRGB(step * i, UnityEngine.Random.Range(0.6f, 1.0f), UnityEngine.Random.Range(0.6f, 1.0f));
            }

            return cols;
        }
    } 
}
