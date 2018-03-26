using System;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет сетку для быстрого вычисления траекторий движения частиц.
    /// </summary>
    public class ParticleGrid
    {
        #region Fields

        private const int HalfCellBlockWidth = 1;

        private ParticleGridCell[,,] cells;

        /// <summary>
        /// Количество строк сетки.
        /// </summary>
        public readonly int Rows;

        /// <summary>
        /// Количество столбцов сетки.
        /// </summary>
        public readonly int Columns;

        /// <summary>
        /// Количество слоев сетки.
        /// </summary>
        public readonly int Layers;

        /// <summary>
        /// Ширина ячейки.
        /// </summary>
        public readonly double CellWidth;

        /// <summary>
        /// Высота ячейки.
        /// </summary>
        public readonly double CellHeight;

        /// <summary>
        /// Глубина ячейки.
        /// </summary>
        public readonly double CellDepth;

        #endregion Fields

        /// <summary>
        /// Возвращает ячейку сетки по указанным индексам.
        /// </summary>
        /// <param name="row">Номер строки.</param>
        /// <param name="column">Номер столбца.</param>
        /// <param name="layer">Номер слоя.</param>
        public ParticleGridCell this[int row, int column, int layer] => cells[row, column, layer];

        /// <summary>
        /// Создает новый экземпляр <see cref="ParticleGrid"/>.
        /// </summary>
        /// <param name="spaceSize">Размеры моделируемого пространства по трем измерениям.</param>
        /// <param name="cellCount">Количество разбиений пространства по трем измерениям.</param>
        public ParticleGrid(Vector3 spaceSize, (int X, int Y, int Z) cellCount)
        {
            cells = new ParticleGridCell[cellCount.X, cellCount.Y, cellCount.Z];
            (Rows, Columns, Layers) = cellCount;

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    for (int k = 0; k < Layers; k++)
                    {
                        cells[i, j, k] = new ParticleGridCell();
                    }
                }
            }

            CellWidth = spaceSize.X / cellCount.X;
            CellHeight = spaceSize.Y / cellCount.Y;
            CellDepth = spaceSize.Z / cellCount.Z;

            InitializeBoundaryCells();
        }

        /// <summary>
        /// Возвращает ячейку, содержащую частицу с заданным положением в пространстве.
        /// </summary>
        /// <param name="position">Положение частицы в пространстве.</param>
        /// <returns></returns>
        public ParticleGridCell GetContainingCell(ref Vector3 position)
        {
            int r = (int)Math.Floor(position.X / CellWidth) % Rows;
            r += r < 0 ? Rows : 0;

            int c = (int)Math.Floor(position.Y / CellHeight) % Columns;
            c += c < 0 ? Columns : 0;

            int l = (int)Math.Floor(position.Z / CellDepth) % Layers;
            l += l < 0 ? Layers : 0;

            return cells[r, c, l];
        }

        /// <summary>
        /// Определяет соседние ячейки для каждой ячейки.
        /// </summary>
        private void InitializeBoundaryCells()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    for (int k = 0; k < Layers; k++)
                    {
                        ParticleGridCell cell = cells[i, j, k];

                        for (int x = Math.Max(0, i - HalfCellBlockWidth); x < Math.Min(Rows, i + HalfCellBlockWidth + 1); x++)
                        {
                            for (int y = Math.Max(0, j - HalfCellBlockWidth); y < Math.Min(Columns, j + HalfCellBlockWidth + 1); y++)
                            {
                                for (int z = Math.Max(0, k - HalfCellBlockWidth); z < Math.Min(Layers, k + HalfCellBlockWidth + 1); z++)
                                {
                                    if (x != i || y != j || z != k)
                                    {
                                        cell.BoundaryCells.Add(cells[x, y, z]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
