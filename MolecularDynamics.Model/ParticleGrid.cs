using System;
using System.Collections.Generic;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет сетку для быстрого вычисления траекторий движения частиц.
    /// </summary>
    public class ParticleGrid
    {
        private Vector3 spaceSize;
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
            this.spaceSize = spaceSize;

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
        /// <param name="cellIndex"></param>
        /// <param name="position">Положение частицы в пространстве.</param>
        /// <returns></returns>
        public (ParticleGridCell, Vector3) GetContainingCell((int X, int Y, int Z) cellIndex, ref Vector3 position)
        {
            (int X, int Y, int Z) containingCellIndex;

            containingCellIndex.X = (int)Math.Floor(position.X / CellSize.X) % CellCount.X;
            containingCellIndex.X += containingCellIndex.X < 0 ? CellCount.X : 0;

            containingCellIndex.Y = (int)Math.Floor(position.Y / CellSize.Y) % CellCount.Y;
            containingCellIndex.Y += containingCellIndex.Y < 0 ? CellCount.Y : 0;

            containingCellIndex.Z = (int)Math.Floor(position.Z / CellSize.Z) % CellCount.Z;
            containingCellIndex.Z += containingCellIndex.Z < 0 ? CellCount.Z : 0;

            (int X, int Y, int Z) changeOfIndex;
            changeOfIndex.X = containingCellIndex.X - cellIndex.X;
            changeOfIndex.Y = containingCellIndex.Y - cellIndex.Y;
            changeOfIndex.Z = containingCellIndex.Z - cellIndex.Z;

            Vector3 changeOfPosition = new Vector3(0.0, 0.0, 0.0);
            changeOfPosition.X += Math.Abs(changeOfIndex.X) == CellCount.X - 1 ? Math.Sign(changeOfIndex.X) * spaceSize.X : 0.0;
            changeOfPosition.Y += Math.Abs(changeOfIndex.Y) == CellCount.Y - 1 ? Math.Sign(changeOfIndex.Y) * spaceSize.Y : 0.0;
            changeOfPosition.Z += Math.Abs(changeOfIndex.Z) == CellCount.Z - 1 ? Math.Sign(changeOfIndex.Z) * spaceSize.Z : 0.0;

            return (cells[containingCellIndex.X, containingCellIndex.Y, containingCellIndex.Z], changeOfPosition);
        }

        /// <summary>
        /// Перемещает частицы в соответствующие ячейки.
        /// </summary>
        public void RedistributeParticles()
        {
            ForEachCell((cell, index) =>
            {
                IList<Particle> particles = cell.Particles;

                for (int i = 0; i < particles.Count; i++)
                {
                    Particle particle = particles[i];
                    ref Vector3 position = ref particle.GetPositionByRef();
                    var (containingCell, changeOfPosition) = GetContainingCell(index, ref position);

                    if (containingCell != cell)
                    {
                        particles.RemoveAt(i);
                        containingCell.Particles.Add(particle);
                        position.AddToCurrent(changeOfPosition);
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
