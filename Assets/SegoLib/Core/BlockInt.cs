using System;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib
{
    /// <summary>
    /// Object representing a block of 2d coordinates.<br/>
    /// BlockInt will enforce minimum dimensions of (0,0).
    /// </summary>
    [System.Serializable]
    public struct BlockInt
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public class AscendingAreaComparer : IComparer<BlockInt>
        {
            public int Compare(BlockInt x, BlockInt y)
            {
                return x.Area.CompareTo(y.Area);
            }
        }

        public class DescendingAreaComparer : IComparer<BlockInt>
        {
            public int Compare(BlockInt x, BlockInt y)
            {
                return y.Area.CompareTo(x.Area);
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/



        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Delegates ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/


        public delegate void CellAction(int x, int y);

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Delegates ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/



        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Statics ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        public static BlockInt FromMinMax(int minX, int minY, int maxX, int maxY)
        {
            BlockInt block = new BlockInt();
            block.m_x = new IntRange(minX, maxX);
            block.m_y = new IntRange(minY, maxY);

            return block;
        }

        public static BlockInt FromPosAndSize(int posX, int posY, int width, int height)
        {
            BlockInt block = new BlockInt();
            block.Position = new Vector2Int(posX, posY);
            block.Width = width;
            block.Height = height;
            return block;
        }

        /// <summary>
        /// Create a BlockInt that completely contains all other blocks in <paramref name="blocks"/> with <paramref name="padding"/> cells around the edges.
        /// </summary>
        /// <param name="blocks"></param>
        public static BlockInt Encompass(IEnumerable<BlockInt> blocks, int padding = 0)
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;

            foreach (BlockInt block in blocks) {
                if (block.XMin < minX) minX = block.XMin;
                if (block.YMin < minY) minY = block.YMin;
                if (block.XMax > maxX) maxX = block.XMax;
                if (block.YMax > maxY) maxY = block.YMax;
            }

            return FromMinMax(minX - padding, minY - padding, maxX + padding, maxY + padding);
        }

        public static void FixOverlap(ref BlockInt stationaryBlock, ref BlockInt movingBlock)
        {
            // Calculate the distance needed to move the rects apart
            int dx = CalculateSeparation(stationaryBlock.XMin, stationaryBlock.XMax, movingBlock.XMin, movingBlock.XMax);
            int dy = CalculateSeparation(stationaryBlock.YMin, stationaryBlock.YMax, movingBlock.YMin, movingBlock.YMax);

            dx = dy = Mathf.Min(dx, dy);

            if (movingBlock.Center.x < stationaryBlock.Center.x)
                dx *= -1;

            if (movingBlock.Center.y < stationaryBlock.Center.y)
                dy *= -1;

            Vector2Int pos = movingBlock.Position + new Vector2Int(dx, dy);

            movingBlock.Position = pos;
        }

        private static int CalculateSeparation(int aMin, int aMax, int bMin, int bMax)
        {
            // Calculate the overlap between the two rects
            int overlap = Mathf.Min(aMax, bMax) - Mathf.Max(aMin, bMin);

            // Calculate the distance needed to separate the rects
            return overlap + 1;
        }

        /// <summary>
        /// Check if <paramref name="a"/>'s border is touching, but not overlapping, <paramref name="b"/>'s border.
        /// If so, return true with the bordering coordinates in <paramref name="sharedBorder"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="sharedBorder"></param>
        /// <returns></returns>
        public static bool GetSharedBorder(BlockInt a, BlockInt b, out CoordinateInt[] sharedBorder)
        {
            if (b.XMin - 1 == a.XMax) {
                return ProcessPotentialSharedBorder(new IntRange(a.YMin, a.YMax), new IntRange(b.YMin, b.YMax), (i) => new CoordinateInt(a.XMax, i), out sharedBorder);
            } else if (b.XMax + 1 == a.XMin) {
                return ProcessPotentialSharedBorder(new IntRange(a.YMin, a.YMax), new IntRange(b.YMin, b.YMax), (i) => new CoordinateInt(a.XMin, i), out sharedBorder);
            } else if (b.YMax + 1 == a.YMin) {
                return ProcessPotentialSharedBorder(new IntRange(a.XMin, a.XMax), new IntRange(b.XMin, b.XMax), (i) => new CoordinateInt(i, a.YMin), out sharedBorder);
            } else if (b.YMin - 1 == a.YMax) {
                return ProcessPotentialSharedBorder(new IntRange(a.XMin, a.XMax), new IntRange(b.XMin, b.XMax), (i) => new CoordinateInt(i, a.YMax), out sharedBorder);
            }

            sharedBorder = null;
            return false;

            bool ProcessPotentialSharedBorder(IntRange sideA, IntRange sideB, Func<int, CoordinateInt> createCoordianteFromIndex, out CoordinateInt[] potentialBorder)
            {
                if (IntRange.TryGetOverlap(sideA, sideB, out IntRange overlap)) {
                    potentialBorder = new CoordinateInt[overlap.Spread + 1];
                    for (int i = overlap.Min; i <= overlap.Max; i++) {
                        potentialBorder[i - overlap.Min] = createCoordianteFromIndex(i);
                    }

                    return true;
                }

                potentialBorder = null;
                return false;
            }
        }

        /// <summary>
        /// Get whether or not <paramref name="a"/> and <paramref name="b"/> are immediately adjacent.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool AreAdjacent(BlockInt a, BlockInt b)
        {
            if (b.XMin - 1 == a.XMax || b.XMax + 1 == a.XMin) {
                return a.m_y.Overlaps(b.m_y);
            } else if (b.YMax + 1 == a.YMin || b.YMin - 1 == a.YMax) {
                return a.m_x.Overlaps(b.m_x);
            }

            return false;
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Statics ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/



        public int Width
        {
            get => (XMax - XMin) + 1;
            set => XMax = XMin + (value - 1);
        }
        public int Height
        {
            get => (YMax - YMin) + 1;
            set => YMax = YMin + (value - 1);
        }
        public int Area => Width * Height;

        public int XMin
        {
            get => m_x.Min;
            set =>  m_x.Min = value;
        }
        public int XMax
        {
            get => m_x.Max;
            set => m_x.Max = value;
        }
        public int YMin
        {
            get => m_y.Min;
            set => m_y.Min = value;
        }
        public int YMax
        {
            get => m_y.Max;
            set => m_y.Max = value;
        }

        public Vector2Int Position
        {
            get => new Vector2Int(XMin, YMin);
            set {
                m_x = new IntRange(value.x, value.x + (Width-1));
                m_y = new IntRange(value.y, value.y + (Height-1));
            }
        }

        public Vector2 Center => new Vector2((Width * 0.5f) + XMin, (Height * 0.5f) + YMin);

        public CoordinateInt Min => new CoordinateInt(m_x.Min, m_y.Min);
        public CoordinateInt Max => new CoordinateInt(m_x.Max, m_y.Max);

        [SerializeField] private IntRange m_x;
        [SerializeField] private IntRange m_y;




        public void ForEachCellInBounds(CellAction action)
        {
            if (action == null)
                return;

            for (int x = XMin; x <= XMax; x++) {
                for (int y = YMin; y <= YMax; y++) {
                    action(x, y);
                }
            }
        }

        public float GetNormalizedX(int x)
        {
            return Mathf.InverseLerp(XMin, XMax, x);
        }

        public float GetNormalizedY(int y)
        {
            return Mathf.InverseLerp(YMin, YMax, y);
        }

        public bool Overlaps(BlockInt other)
        {
            //  This is right of other  or   this is left of other
            if (XMin > other.XMax       ||   XMax < other.XMin)
                return false;

            //  This is above other   or   this is below other
            if (YMin > other.YMax     ||   YMax < other.YMin)
                return false;

            return true;
        }

        public bool Contains(int x, int y)
        {
            return x >= XMin && x <= XMax && y >= YMin && y <= YMax;
        }

        public bool Contains(BlockInt other)
        {
            return other.XMin >= XMin && other.XMax <= XMax && other.YMin >= YMin && other.YMax <= YMax;
        }

        public bool CanContain(BlockInt other)
        {
            return other.Width <= Width && other.Height <= Height;
        }

        /// <summary>
        /// Moves <paramref name="other"/> inside this block if possible. Returns false if it is not possible.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool MoveInside(ref BlockInt other)
        {
            if (Contains(other))
                return true;

            if (!CanContain(other))
                return false;

            int dX = 0;
            int dY = 0;

            if (other.XMax > XMax)
                dX = XMax - other.XMax;
            else if(other.XMin < XMin)
                dX = XMin - other.XMin;

            if(other.YMax > YMax)
                dY = YMax - other.YMax;
            else if(other.YMin < YMin)
                dY= YMin - other.YMin;

            other.Position = other.Position + new Vector2Int(dX, dY);

            return true;
        }

        public override string ToString()
        {
            return $"Min:({XMin}, {YMin}) Max:({XMax},{YMax})";
        }

        public void DEBUG_Draw()
        {
            UnityEngine.Gizmos.DrawWireCube(Center, new Vector3Int(Width, Height));
        }
    }
}
