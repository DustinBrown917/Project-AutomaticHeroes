using UnityEngine;

namespace SegoLib
{
    /// <summary>
    /// Extensions specifically for Colors.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// The magnitude of the of the RGB components of a color.
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public static float MagRGB(this Color col) => Mathf.Sqrt(col.r * col.r + col.g * col.g + col.b * col.b);



        /// <summary>
        /// Get the square magnitude of the RGB components of a color.
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public static float SquareMagRGB(this Color col) => col.r * col.r + col.g * col.g + col.b * col.b;



        /// <summary>
        /// Convert a color to a Vector3.
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public static Vector3 ToVec3(this Color col) => new Vector3(col.r, col.g, col.b);



        /// <summary>
        /// Multiply a colors RGB components by <paramref name="amount"/>.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Color MultiplyRGB(this Color c, float amount)
        {
            c.r *= amount;
            c.g *= amount;
            c.b *= amount;

            return c;
        }
    } 
}
