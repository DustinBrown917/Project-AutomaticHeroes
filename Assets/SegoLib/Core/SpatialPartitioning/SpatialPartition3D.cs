using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.SpatialPartioning
{
    public class SpatialPartition3D<T>
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/


        public class Cell
        {
            public T[] m_items = new T[0];
        }

        public class Builder
        {
            private List<KeyValuePair<Vector3, T>> m_data = new List<KeyValuePair<Vector3, T>>();

            public void AddData(Vector3 pos, T item)
            {
                m_data.Add(new KeyValuePair<Vector3, T>(pos, item));
            }

            public SpatialPartition3D<T> Build(int divisions)
            {
                if (divisions <= 1) {
                    throw new System.ArgumentException($"{nameof(divisions)} must be greater than 1");
                }

                SpatialPartition3D<T> partition = new SpatialPartition3D<T>(divisions);

                Dictionary<Vector3Int, List<T>> spacedData = new Dictionary<Vector3Int, List<T>>();

                //Separate data into lists based on what cell the data would land in.
                foreach (var kvp in m_data) {
                    Vector3Int address = partition.PosToCellAddress(kvp.Key);

                    if (!spacedData.TryGetValue(address, out List<T> list)) {
                        list = new List<T>();
                        spacedData.Add(address, list);
                    }

                    list.Add(kvp.Value);
                }

                //Put the separated data into the partition.
                foreach (var kvp in spacedData) {
                    partition.GetCell(kvp.Key).m_items = kvp.Value.ToArray();
                }

                return partition;
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/



        private Cell[,,] m_cells;

        private int m_divisions;

        public SpatialPartition3D(int divisions)
        {
            if(divisions <= 1) {
                throw new System.ArgumentException($"{nameof(divisions)} must be greater than 1");
            }
            m_divisions = divisions;
            m_cells = new Cell[m_divisions, m_divisions, m_divisions];

            for(int x = 0; x < divisions; x++) {
                for(int y = 0; y < divisions; y++) {
                    for(int z = 0; z < divisions; z++) {
                        m_cells[x, y, z] = new Cell();
                    }
                }
            }
        }

        /// <summary>
        /// Get the cell at <paramref name="position"/>. All components should be between 0 and 1.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Cell GetCell(Vector3 position)
        {
            return GetCell(PosToCellAddress(position));
        }

        /// <summary>
        /// Get the cell at <paramref name="address"/>.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public Cell GetCell(Vector3Int address)
        {
            return GetCell(address.x, address.y, address.z);
        }

        /// <summary>
        /// Get the cell at the address (<paramref name="x"/>, <paramref name="y"/>, <paramref name="z"/>).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Cell GetCell(int x, int y, int z)
        {
            return m_cells[x,y,z];
        }

        /// <summary>
        /// Get the all cells within <paramref name="range"/> distance of <paramref name="targetCell"/>.
        /// </summary>
        /// <param name="targetCell"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public List<Cell> GetSurroundingCells(Vector3Int targetCell, int range)
        {
            if(range <= 0) { return new List<Cell>() { GetCell(targetCell) }; }

            List<Cell> cells = new List<Cell>();

            for(int x = targetCell.x - range; x < targetCell.x + range; x++) {
                if (x < 0 || x >= m_divisions) { continue; }
                for (int y = targetCell.y - range; y < targetCell.y + range; y++) {
                    if (y < 0 || y >= m_divisions) { continue; }
                    for (int z = targetCell.z - range; z < targetCell.z + range; z++) {
                        if(z < 0 || z >= m_divisions) { continue; }
                        cells.Add(GetCell(x, y, z));
                    }
                }
            }
            return cells;
        }

        /// <summary>
        /// Convert a position into a cell address. All components of <paramref name="position"/> should be between 0 and 1.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector3Int PosToCellAddress(Vector3 position)
        {
            if (!PositionIsValid(position)) {
                throw new System.ArgumentException($"No dimension of {nameof(position)} should exceed 1 or be less than 0.");
            }

            position *= m_divisions;

            Vector3Int iPos = new Vector3Int((int)position.x, (int)position.y, (int)position.z);

            if (iPos.x == m_divisions) iPos.x--;
            if (iPos.y == m_divisions) iPos.y--;
            if (iPos.z == m_divisions) iPos.z--;

            return iPos;
        }

        /// <summary>
        /// Returns true if all components of <paramref name="position"/> are between 0 and 1.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool PositionIsValid(Vector3 position)
        {
            return position.x >= 0 && position.x <= 1.0f && position.y >= 0 && position.y <= 1.0f && position.z >= 0 && position.z <= 1.0f;
        }
    } 
}
