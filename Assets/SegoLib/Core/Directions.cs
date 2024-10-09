using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib
{
    public static class Directions
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        /// <summary>
        /// Provides a way to enumerate over directions in the following order:
        /// <para>[N, NE, E, SE, S, SW, W, NW]</para>
        /// </summary>
        public class D8 : IEnumerable<Vector2Int>
        {
            public IEnumerator<Vector2Int> GetEnumerator()
            {
                yield return N;
                yield return NE;
                yield return E;
                yield return SE;
                yield return S;
                yield return SW;
                yield return W;
                yield return NW;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        /// <summary>
        /// Provides a way to enumerate over directions in the following order:
        /// <para>[None, N, NE, E, SE, S, SW, W, NW]</para>
        /// </summary>
        public class D9 : IEnumerable<Vector2Int>
        {
            public IEnumerator<Vector2Int> GetEnumerator()
            {
                yield return None;
                yield return N;
                yield return NE;
                yield return E;
                yield return SE;
                yield return S;
                yield return SW;
                yield return W;
                yield return NW;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        /// <summary>
        /// Provides a way to enumerate over directions in the following order:
        /// <para>[N, E, S, W]</para>
        /// </summary>
        public class D4 : IEnumerable<Vector2Int>
        {
            public IEnumerator<Vector2Int> GetEnumerator()
            {
                yield return N;
                yield return E;
                yield return S;
                yield return W;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        /// <summary>
        /// Provides a way to enumerate over directions in the following order:
        /// <para>[None, N, E, S, W]</para>
        /// </summary>
        public class D5 : IEnumerable<Vector2Int>
        {
            public IEnumerator<Vector2Int> GetEnumerator()
            {
                yield return None;
                yield return N;
                yield return E;
                yield return S;
                yield return W;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/








        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Statics ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        /// <summary>
        /// North (0, 1)
        /// <para>• ↑ •</para>
        /// <para>• • •</para>
        /// <para>• • •</para>
        /// </summary>
        public static readonly Vector2Int N = new Vector2Int(0, 1);

        /// <summary>
        /// South (0, -1)
        /// <para>• • •</para>
        /// <para>• • •</para>
        /// <para>• ↓ •</para>
        /// </summary>
        public static readonly Vector2Int S = new Vector2Int(0, -1);

        /// <summary>
        /// East (1, 0)
        /// <para>• • •</para>
        /// <para>• • →</para>
        /// <para>• • •</para>
        /// </summary>
        public static readonly Vector2Int E = new Vector2Int(1, 0);

        /// <summary>
        /// West (-1, 0)
        /// <para>• • •</para>
        /// <para>← • •</para>
        /// <para>• • •</para>
        /// </summary>
        public static readonly Vector2Int W = new Vector2Int(-1, 0);

        /// <summary>
        /// Northeast (1, 1)
        /// <para>• • ↗</para>
        /// <para>• • •</para>
        /// <para>• • •</para>
        /// </summary>
        public static readonly Vector2Int NE = new Vector2Int(1, 1);

        /// <summary>
        /// Northwest (-1, 1)
        /// <para>↖ • •</para>
        /// <para>• • •</para>
        /// <para>• • •</para>
        /// </summary>
        public static readonly Vector2Int NW = new Vector2Int(-1, 1);

        /// <summary>
        /// Southeast (1, -1)
        /// <para>• • •</para>
        /// <para>• • •</para>
        /// <para>• • ↘</para>
        /// </summary>
        public static readonly Vector2Int SE = new Vector2Int(1, -1);

        /// <summary>
        /// Southwest (-1, -1)
        /// <para>• • •</para>
        /// <para>• • •</para>
        /// <para>↙ • •</para>
        /// </summary>
        public static readonly Vector2Int SW = new Vector2Int(-1, -1);

        /// <summary>
        /// No Direction (0, 0)
        /// <para>• • •</para>
        /// <para>• • •</para>
        /// <para>• • •</para>
        /// </summary>
        public static readonly Vector2Int None = Vector2Int.zero;

        /// <summary>
        /// Cached instance of D4.
        /// <para>(See class summary for more details)</para>
        /// </summary>
        public static readonly D4 _D4 = new D4();

        /// <summary>
        /// Cached instance of D5.
        /// <para>(See class summary for more details)</para>
        /// </summary>
        public static readonly D5 _D5 = new D5();

        /// <summary>
        /// Cached instance of D8.
        /// <para>(See class summary for more details)</para>
        /// </summary>
        public static readonly D8 _D8 = new D8();

        /// <summary>
        /// Cached instance od D9.
        /// <para>(See class summary for more details)</para>
        /// </summary>
        public static readonly D9 _D9 = new D9();

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Statics ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
    } 
}
