﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет сетку для быстрого вычисления траекторий движения частиц.
    /// </summary>
    public class ParticleGrid
    {
        private Vector3 _spaceSize;
        private Cell[,,] _cells;
        private double _interactionRadiusSquared;
        private Task[] _tasks;

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
        public Cell this[int row, int column, int layer] => _cells[row, column, layer];

        /// <summary>
        /// Создает новый экземпляр <see cref="ParticleGrid"/>.
        /// </summary>
        /// <param name="spaceSize">Размеры моделируемого пространства по трем измерениям.</param>
        /// <param name="cellCount">Количество разбиений пространства по трем измерениям.</param>
        /// <param name="interactionRadius">Радиус взаимодействия частиц.</param>
        /// <param name="threads">Количество потоков исполнения.</param>
        public ParticleGrid(Vector3 spaceSize, (int X, int Y, int Z) cellCount, double interactionRadius, int threads)
        {            
            _spaceSize = spaceSize;
            CellCount = cellCount;
            _interactionRadiusSquared = interactionRadius * interactionRadius;
            _cells = new Cell[cellCount.X, cellCount.Y, cellCount.Z];
            _tasks = new Task[threads - 1];                        

            CellSize.X = spaceSize.X / cellCount.X;
            CellSize.Y = spaceSize.Y / cellCount.Y;
            CellSize.Z = spaceSize.Z / cellCount.Z;

            for (int i = 0; i < CellCount.X; i++)
            {
                for (int j = 0; j < CellCount.Y; j++)
                {
                    for (int k = 0; k < CellCount.Z; k++)
                    {
                        Vector3 center = new Vector3(CellSize.X * (i + 0.5), CellSize.Y * (j + 0.5), CellSize.Z * (k + 0.5));
                        _cells[i, j, k] = new Cell(center);
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
        public /*(*/Cell/*, Vector3)*/ GetContainingCell((int X, int Y, int Z) cellIndex, ref Vector3 position)
        {
            //(int X, int Y, int Z) containingCellIndex;

            //containingCellIndex.X = (int)Math.Floor(position.X / CellSize.X) % CellCount.X;
            //containingCellIndex.X += containingCellIndex.X < 0 ? CellCount.X : 0;

            //containingCellIndex.Y = (int)Math.Floor(position.Y / CellSize.Y) % CellCount.Y;
            //containingCellIndex.Y += containingCellIndex.Y < 0 ? CellCount.Y : 0;

            //containingCellIndex.Z = (int)Math.Floor(position.Z / CellSize.Z) % CellCount.Z;
            //containingCellIndex.Z += containingCellIndex.Z < 0 ? CellCount.Z : 0;

            //(int X, int Y, int Z) changeOfIndex;
            //changeOfIndex.X = containingCellIndex.X - cellIndex.X;
            //changeOfIndex.Y = containingCellIndex.Y - cellIndex.Y;
            //changeOfIndex.Z = containingCellIndex.Z - cellIndex.Z;

            //Vector3 changeOfPosition = new Vector3(0.0, 0.0, 0.0);
            //changeOfPosition.X += Math.Abs(changeOfIndex.X) == CellCount.X - 1 ? Math.Sign(changeOfIndex.X) * spaceSize.X : 0.0;
            //changeOfPosition.Y += Math.Abs(changeOfIndex.Y) == CellCount.Y - 1 ? Math.Sign(changeOfIndex.Y) * spaceSize.Y : 0.0;
            //changeOfPosition.Z += Math.Abs(changeOfIndex.Z) == CellCount.Z - 1 ? Math.Sign(changeOfIndex.Z) * spaceSize.Z : 0.0;

            //return (cells[containingCellIndex.X, containingCellIndex.Y, containingCellIndex.Z], changeOfPosition);

            (int X, int Y, int Z) containingCellIndex;

            position.X = position.X < 0.0 ? _spaceSize.X : position.X;
            position.X = position.X > _spaceSize.X ? 0.0 : position.X;

            position.Y = position.Y < 0.0 ? _spaceSize.Y : position.Y;
            position.Y = position.Y > _spaceSize.Y ? 0.0 : position.Y;

            position.Z = position.Z < 0.0 ? _spaceSize.Z : position.Z;
            position.Z = position.Z > _spaceSize.Z ? 0.0 : position.Z;

            containingCellIndex.X = (int)Math.Floor(position.X / CellSize.X) % (CellCount.X + 1);
            containingCellIndex.Y = (int)Math.Floor(position.Y / CellSize.Y) % (CellCount.Y + 1);
            containingCellIndex.Z = (int)Math.Floor(position.Z / CellSize.Z) % (CellCount.Z + 1);

            containingCellIndex.X = containingCellIndex.X == CellCount.X ? containingCellIndex.X - 1 : containingCellIndex.X;
            containingCellIndex.Y = containingCellIndex.Y == CellCount.Y ? containingCellIndex.Y - 1 : containingCellIndex.Y;
            containingCellIndex.Z = containingCellIndex.Z == CellCount.Z ? containingCellIndex.Z - 1 : containingCellIndex.Z;

            return _cells[containingCellIndex.X, containingCellIndex.Y, containingCellIndex.Z];
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
                    var containingCell = GetContainingCell(index, ref position);

                    if (containingCell != cell)
                    {
                        lock (particles)
                        {
                            lock (containingCell.Particles)
                            {
                                particles.RemoveAt(i);
                                containingCell.Particles.Add(particle);
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Определяет соседние ячейки для каждой ячейки.
        /// </summary>
        private void InitializeBoundaryCells()
        {
            double interactionRadius = Math.Sqrt(_interactionRadiusSquared);
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
                                Vector3 distance = cell.Position - _cells[x, y, z].Position;

                                if (!(distance.NormSquared() > _interactionRadiusSquared))
                                {
                                    cell.BoundaryCells.Add(_cells[x, y, z]);
                                }
                            }
                        }
                    }
                }
            });
        }

        ///// <summary>
        ///// Выполняет указанное действие для каждой ячейки сетки частиц.
        ///// </summary>
        ///// <param name="action">Действие для каждоЙ ячейки.</param>
        //public void ForEachCell(Action<Cell> action)
        //{
        //    int perCore = CellCount.X / (_tasks.Length + 1);

        //    for (int taskCounter = 1; taskCounter <= _tasks.Length; taskCounter++)
        //    {
        //        int counter = taskCounter;
        //        _tasks[taskCounter - 1] = Task.Run(() =>
        //        {
        //            for (int i = counter * perCore; i < (counter + 1) * perCore; i++)
        //            {
        //                for (int j = 0; j < CellCount.Y; j++)
        //                {
        //                    for (int k = 0; k < CellCount.Z; k++)
        //                    {
        //                        action(_cells[i, j, k]);
        //                    }
        //                }
        //            }
        //        });
        //    }

        //    for (int i = 0; i < perCore; i++)
        //    {
        //        for (int j = 0; j < CellCount.Y; j++)
        //        {
        //            for (int k = 0; k < CellCount.Z; k++)
        //            {
        //                action(_cells[i, j, k]);
        //            }
        //        }
        //    }

        //    Task.WaitAll(_tasks);
        //}

        /// <summary>
        /// Выполняет указанное действие для каждой ячейки сетки частиц.
        /// </summary>
        /// <param name="action">Действие для каждоЙ ячейки.</param>
        public void ForEachCell(Action<Cell, (int X, int Y, int Z)> action)
        {
            int perCore = CellCount.X / (_tasks.Length + 1);

            for (int taskCounter = 0; taskCounter < _tasks.Length; taskCounter++)
            {
                int counter = taskCounter;
                _tasks[taskCounter] = Task.Run(() =>
                {
                    for (int i = counter * perCore; i < (counter + 1) * perCore; i++)
                    {
                        for (int j = 0; j < CellCount.Y; j++)
                        {
                            for (int k = 0; k < CellCount.Z; k++)
                            {
                                action(_cells[i, j, k], (i, j, k));
                            }
                        }
                    }
                });
            }

            for (int i = _tasks.Length * perCore; i < CellCount.X; i++)
            {
                for (int j = 0; j < CellCount.Y; j++)
                {
                    for (int k = 0; k < CellCount.Z; k++)
                    {
                        action(_cells[i, j, k], (i, j, k));
                    }
                }
            }

            Task.WaitAll(_tasks);
        }

        ///// <summary>
        ///// Выполняет указанное действие для каждой ячейки сетки частиц.
        ///// </summary>
        ///// <param name="action">Действие для каждоЙ ячейки.</param>
        //public void ForEachCellSequentally(Action<ParticleGridCell, (int X, int Y, int Z)> action)
        //{
        //    for (int i = 0; i < CellCount.X; i++)
        //    {
        //        for (int j = 0; j < CellCount.Y; j++)
        //        {
        //            for (int k = 0; k < CellCount.Z; k++)
        //            {
        //                action(cells[i, j, k], (i, j, k));
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Представляет ячейку сетки частиц.
        /// </summary>
        public class Cell
        {
            /// <summary>
            /// Положение центра ячейки.
            /// </summary>
            public readonly Vector3 Position;

            /// <summary>
            /// Список частиц, находящихся в ячейке.
            /// </summary>
            public IList<Particle> Particles { get; } = new List<Particle>();

            /// <summary>
            /// Список соседних ячеек.
            /// </summary>
            public IList<Cell> BoundaryCells { get; } = new List<Cell>();

            /// <summary>
            /// Создает новый экземпляр <see cref="Cell"/>.
            /// </summary>
            /// <param name="position">Положение центра ячейки.</param>
            public Cell(Vector3 position)
            {
                Position = position;
            }
        }
    }
}
