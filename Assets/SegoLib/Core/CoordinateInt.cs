using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib
{
    public readonly struct CoordinateInt : IEquatable<CoordinateInt>
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public class XComparerAscending : IComparer<CoordinateInt>
        {
            public int Compare(CoordinateInt x, CoordinateInt y) => x.x.CompareTo(y.x);
        }

        public class XComparerDescending : IComparer<CoordinateInt>
        {
            public int Compare(CoordinateInt x, CoordinateInt y) => y.x.CompareTo(x.x);
        }

        public class YComparerAscending : IComparer<CoordinateInt>
        {
            public int Compare(CoordinateInt x, CoordinateInt y) => x.y.CompareTo(y.y);
        }

        public class YComparerDescending : IComparer<CoordinateInt>
        {
            public int Compare(CoordinateInt x, CoordinateInt y) => y.y.CompareTo(x.y);
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Statics ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public static implicit operator CoordinateInt(Vector2Int vec) => new CoordinateInt(vec.x, vec.y);
        public static implicit operator Vector2Int(CoordinateInt coord) => new Vector2Int(coord.x, coord.y);

        public static implicit operator CoordinateInt((int x, int y) tuple) => new CoordinateInt(tuple.x, tuple.y);
        public static implicit operator (int x, int y)(CoordinateInt coord) => (coord.x, coord.y);

        public static implicit operator Vector2(CoordinateInt coord) => new Vector2(coord.x, coord.y);
        public static implicit operator Vector3(CoordinateInt coord) => new Vector3(coord.x, coord.y);
        public static implicit operator Vector3Int(CoordinateInt coord) => new Vector3Int(coord.x, coord.y);

        public static CoordinateInt operator +(CoordinateInt a, CoordinateInt b) => new CoordinateInt(a.x + b.x, a.y + b.y);
        public static CoordinateInt operator -(CoordinateInt a, CoordinateInt b) => new CoordinateInt(a.x - b.x, a.y - b.y);

        public static CoordinateInt operator +(CoordinateInt a, (int x, int y) b) => new CoordinateInt(a.x + b.x, a.y + b.y);
        public static CoordinateInt operator -(CoordinateInt a, (int x, int y) b) => new CoordinateInt(a.x - b.x, a.y - b.y);
        public static CoordinateInt operator +((int x, int y) a, CoordinateInt b) => new CoordinateInt(a.x + b.x, a.y + b.y);
        public static CoordinateInt operator -((int x, int y) a, CoordinateInt b) => new CoordinateInt(a.x - b.x, a.y - b.y);

        public static CoordinateInt operator +(CoordinateInt a, Vector2Int b) => new CoordinateInt(a.x + b.x, a.y + b.y);
        public static CoordinateInt operator -(CoordinateInt a, Vector2Int b) => new CoordinateInt(a.x - b.x, a.y - b.y);
        public static Vector2Int operator +(Vector2Int a, CoordinateInt b) => new Vector2Int(a.x + b.x, a.y + b.y);
        public static Vector2Int operator -(Vector2Int a, CoordinateInt b) => new Vector2Int(a.x - b.x, a.y - b.y);

        public static bool operator ==(CoordinateInt a, CoordinateInt b) => a.Equals(b);
        public static bool operator !=(CoordinateInt a, CoordinateInt b) => !a.Equals(b);

        public static readonly CoordinateInt origin = new CoordinateInt(0, 0);

        public static float Distance(CoordinateInt a, CoordinateInt b)
        {
            return Mathf.Sqrt(DistanceSquared(a, b));
        }

        public static float DistanceSquared(CoordinateInt a, CoordinateInt b)
        {
            CoordinateInt diff = a - b;
            return (diff.x * diff.x) + (diff.y * diff.y);
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Statics ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        public readonly int x;
        public readonly int y;

        public CoordinateInt(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(CoordinateInt other)
        {
            return other.x == x && other.y == y;
        }

        public override bool Equals(object obj)
        {
            return obj is CoordinateInt other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public override string ToString()
        {
            return $"x:{x}, y:{y}";
        }
    }
}
