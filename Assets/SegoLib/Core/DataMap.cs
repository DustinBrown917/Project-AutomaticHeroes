using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.DataMap
{

    public abstract class DataMap
    {
        public WrapMode2D WrapMode { get; set; }
        public abstract int Width { get; }
        public abstract int Height { get; }
    }

    public class DataMap<T> : DataMap
    {
        public static implicit operator T[,](DataMap<T> map) => map.m_data;

        protected T[,] m_data = new T[1, 1];
        public override int Width => m_data.GetLength(0);
        public override int Height => m_data.GetLength(1);
        public T this[int x, int y]
        {
            get => m_data[x, y];
            set => SetData(value, x, y);
        }
        public T this[Vector2Int index] {
            get => m_data[index.x, index.y];
            set => m_data[index.x, index.y] = value;
        }

        public DataMap() : this(1, 1) { }

        public DataMap(int width, int height)
        {
            m_data = new T[width, height];
        }

        /// <summary>
        /// Sets the data at the given coordinate.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public virtual void SetData(T data, int x, int y)
        {
            m_data[x, y] = data;
        }

        /// <summary>
        /// Copies the data of this map into a new 2d array.
        /// </summary>
        /// <returns></returns>
        public T[,] GetDataClone()
        {
            return (T[,])m_data.Clone();
        }

        /// <summary>
        /// True if x is within the width bounds ofthe map.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public bool XIsValid(int x)
        {
            return x >= 0 && x < Width;
        }

        /// <summary>
        /// True if Y is within the height bounds of the map.
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool YIsValid(int y)
        {
            return y >= 0 && y < Height;
        }

        /// <summary>
        /// Returns true if the coordinate is within the map's bounds.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsValid(int x, int y)
        {
            return XIsValid(x) && YIsValid(y);
        }

        /// <summary>
        /// Returns true if the coordinate is within the map's bounds.
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public bool IsValid(Vector2Int coord)
        {
            return XIsValid(coord.x) && YIsValid(coord.y);
        }

        /// <summary>
        /// Ensures the x coordinate is valid by applying wrap to the coordinate if it overflows
        /// and wrap is enabled, clamping it to the edge if not.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public int ValidateX(int x)
        {
            if (WrapMode.HasFlag(WrapMode2D.X)) {
                return WrapX(x);
            }
            return Mathf.Clamp(x, 0, Width - 1);
        }

        /// <summary>
        /// Ensures the y coordinate is valid by applying wrap to the coordinate if it overflows
        /// and wrap is enabled, clamping it to the edge if not.
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public int ValidateY(int y)
        {
            if (WrapMode.HasFlag(WrapMode2D.Y)) {
                return WrapY(y);
            }
            return Mathf.Clamp(y, 0, Height - 1);
        }

        /// <summary>
        /// Ensures the given coordinates are valid, applying wrap to any coordintes that
        /// exceed the bounds of the map if wrap is enabled, clamping them to the edges of
        /// the map otherwise.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="wrapMode"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        public void Validate(int x, int y, out int nx, out int ny)
        {
            nx = ValidateX(x);
            ny = ValidateY(y);
        }

        /// <summary>
        /// Get a list of cells relative to <paramref name="origin"/>.
        /// Only valid cells on this map are returned.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="relativeAddresses"></param>
        /// <returns></returns>
        public List<Vector2Int> GetValidCellsRelativeTo(Vector2Int origin, IEnumerable<Vector2Int> relativeAddresses)
        {
            List<Vector2Int> neighbours = new List<Vector2Int>();

            foreach(Vector2Int dir in relativeAddresses) {
                Vector2Int neighbour = ApplyWrapping(origin + dir);
                if (IsValid(neighbour)) {
                    neighbours.Add(neighbour);
                }
            }

            return neighbours;
        }

        /// <summary>
        /// Get a hashset containing ever cell address in the map.
        /// </summary>
        /// <returns></returns>
        public HashSet<Vector2Int> GetCellAddressHash()
        {
            HashSet<Vector2Int> hash = new HashSet<Vector2Int>();
            for(int x = 0; x < Width; x++) {
                for(int y = 0; y < Height; y++) {
                    hash.Add(new Vector2Int(x, y));
                }
            }
            return hash;
        }

        /// <summary>
        /// Gets the distance between two points on the map given a specific wrap mode.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="wrapMode"></param>
        /// <returns></returns>
        public float GetWrappedDistance(Vector2Int a, Vector2Int b, WrapMode2D wrapMode)
        {
            float dx;
            float dy;

            dx = Mathf.Abs(a.x - b.x);
            if ((wrapMode & WrapMode2D.X) == WrapMode2D.X && dx > Width * 0.5f) dx = Width - dx;

            dy = Mathf.Abs(a.y - b.y);
            if ((wrapMode & WrapMode2D.Y) == WrapMode2D.Y && dx > Height * 0.5f) dy = Height - dy;

            return new Vector2(dx, dy).magnitude;
        }

        /// <summary>
        /// Wraps <paramref name="y"/> into a valid y coordinate for this map.
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        protected int WrapY(int y)
        {
            return y % Height;
        }

        /// <summary>
        /// Wraps <paramref name="x"/> int a valid x coordinate for this map.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        protected int WrapX(int x)
        {
            return x % Width;
        }

        /// <summary>
        /// Applies this map's wrapping rules to a set of coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        protected void ApplyWrapping(int x, int y, out int nx, out int ny)
        {
            if ((WrapMode & WrapMode2D.X) == WrapMode2D.X) {
                nx = x % Width;
                if (nx < 0) nx += Width;
            } else {
                nx = x;
            }

            if ((WrapMode & WrapMode2D.Y) == WrapMode2D.Y) {
                ny = y % Height;
                if (ny < 0) ny += Height;
            } else {
                ny = y;
            }
        }

        /// <summary>
        /// Applies this map's wrapping rules to a set of coordinates.
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        protected Vector2Int ApplyWrapping(Vector2Int coords)
        {
            ApplyWrapping(coords.x, coords.y, out int x, out int y);

            return new Vector2Int(x, y);
        }
    } 
}
