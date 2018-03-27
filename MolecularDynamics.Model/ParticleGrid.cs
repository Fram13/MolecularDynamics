using System;
using System.Collections.Generic;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет сетку для быстрого вычисления траекторий движения частиц.
    /// </summary>
    public class ParticleGrid
    {
        private ParticleGridCell[,,] cells;
        private double interactionRadius;

        /// <summary>
        /// Количество ячеек по трем измерениям.
        /// </summary>
        public readonly (int X, int Y, int Z) CellCount;

        /// <summary>
        /// Размеры ячеек по трем измерениям.
        /// </summary>
        public readonly Vector3 CellSize;

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
        /// <param name="interactionRadius"></param>
        public ParticleGrid(Vector3 spaceSize, (int X, int Y, int Z) cellCount, double interactionRadius)
        {
            cells = new ParticleGridCell[cellCount.X, cellCount.Y, cellCount.Z];
            this.CellCount = cellCount;
            this.interactionRadius = interactionRadius;

            CellSize.X = spaceSize.X / cellCount.X;
            CellSize.Y = spaceSize.Y / cellCount.Y;
            CellSize.Z = spaceSize.Z / cellCount.Z;

            for (int i = 0; i < CellCount.X; i++)
            {
                for (int j = 0; j < CellCount.Y; j++)
                {
                    for (int k = 0; k < CellCount.Z; k++)
                    {
                        ParticleGridCell cell = new ParticleGridCell()
                        {
                            Position = new Vector3(CellSize.X * (i + 0.5), CellSize.Y * (j + 0.5), CellSize.Z * (k + 0.5))
                        };

                        cells[i, j, k] = cell;
                    }
                }
            }

            InitializeBoundaryCells();
        }

        /// <summary>
        /// Возвращает ячейку, содержащую частицу с заданным положением в пространстве.
        /// </summary>
        /// <param name="position">Положение частицы в пространстве.</param>
        /// <returns></returns>
        public ParticleGridCell GetContainingCell(ref Vector3 position)
        {
            int r = (int)Math.Floor(position.X / CellSize.X) % CellCount.X;
            r += r < 0 ? CellCount.X : 0;

            int c = (int)Math.Floor(position.Y / CellSize.Y) % CellCount.Y;
            c += c < 0 ? CellCount.Y : 0;

            int l = (int)Math.Floor(position.Z / CellSize.Z) % CellCount.Z;
            l += l < 0 ? CellCount.Z : 0;

            return cells[r, c, l];
        }

        /// <summary>
        /// Перемещает частицы в соответствующие ячейки.
        /// </summary>
        public void RedistributeParticles()
        {
            ForEachCell(cell =>
            {
                IList<Particle> particles = cell.Particles;

                for (int i = 0; i < particles.Count; i++)
                {
                    Particle particle = particles[i];
                    ref Vector3 position = ref particle.GetPositionByRef();
                    var containingCell = GetContainingCell(ref position);

                    if (containingCell != cell)
                    {
                        particles.RemoveAt(i);
                        containingCell.Particles.Add(particle);

                        //обработать перенос
                    }
                }
            });
        }

        /// <summary>
        /// Определяет соседние ячейки для каждой ячейки.
        /// </summary>
        private void InitializeBoundaryCells()
        {
            (int X, int Y, int Z) nearestCellCount;
            nearestCellCount.X = (int)Math.Ceiling(interactionRadius / CellSize.X);
            nearestCellCount.Y = (int)Math.Ceiling(interactionRadius / CellSize.Y);
            nearestCellCount.Z = (int)Math.Ceiling(interactionRadius / CellSize.Z);

            ForEachCell((cell, indicies) =>
            {
                for (int x = Math.Max(0, indicies.X - nearestCellCount.X); x < Math.Min(CellCount.X, indicies.X + nearestCellCount.X + 1); x++)
                {
                    for (int y = Math.Max(0, indicies.Y - nearestCellCount.Y); y < Math.Min(CellCount.Y, indicies.Y + nearestCellCount.Y + 1); y++)
                    {
                        for (int z = Math.Max(0, indicies.Z - nearestCellCount.Z); z < Math.Min(CellCount.Z, indicies.Z + nearestCellCount.Z + 1); z++)
                        {
                            if (x != indicies.X || y != indicies.Y || z != indicies.Z)
                            {
                                Vector3 distance = cell.Position - cells[x, y, z].Position;

                                if (!(distance.Norm() > interactionRadius))
                                {
                                    cell.BoundaryCells.Add(cells[x, y, z]);
                                }
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Выполняет указанное действие для каждой ячейки сетки частиц.
        /// </summary>
        /// <param name="action">Действие для каждоЙ ячейки.</param>
        public void ForEachCell(Action<ParticleGridCell> action)
        {
            for (int i = 0; i < CellCount.X; i++)
            {
                for (int j = 0; j < CellCount.Y; j++)
                {
                    for (int k = 0; k < CellCount.Z; k++)
                    {
                        action(cells[i, j, k]);
                    }
                }
            }
        }

        /// <summary>
        /// Выполняет указанное действие для каждой ячейки сетки частиц.
        /// </summary>
        /// <param name="action">Действие для каждоЙ ячейки.</param>
        public void ForEachCell(Action<ParticleGridCell, (int X, int Y, int Z)> action)
        {
            for (int i = 0; i < CellCount.X; i++)
            {
                for (int j = 0; j < CellCount.Y; j++)
                {
                    for (int k = 0; k < CellCount.Z; k++)
                    {
                        action(cells[i, j, k], (i, j, k));
                    }
                }
            }
        }
    }
}
