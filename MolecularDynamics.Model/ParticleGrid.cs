using System;
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
        private (int X, int Y, int Z) _cellCount;
        private Vector3 _cellSize;
        private double _interactionRadiusSquared;
        private Task[] _tasks;
        private Cell[,,] _cells;

        /// <summary>
        /// Создает новый экземпляр <see cref="ParticleGrid"/>.
        /// </summary>
        /// <param name="spaceSize">Размеры моделируемого пространства по трем измерениям.</param>
        /// <param name="cellCount">Количество разбиений пространства по трем измерениям.</param>
        /// <param name="cellSize">Размеры ячейки сетки.</param>
        /// <param name="interactionRadius">Радиус взаимодействия частиц.</param>
        /// <param name="threads">Количество потоков исполнения.</param>
        public ParticleGrid(Vector3 spaceSize, (int X, int Y, int Z) cellCount, Vector3 cellSize, double interactionRadius, int threads)
        {            
            _spaceSize = spaceSize;
            _cellCount = cellCount;
            _cellSize = cellSize;
            _interactionRadiusSquared = interactionRadius * interactionRadius;            
            _tasks = new Task[threads - 1];
            _cells = new Cell[cellCount.X, cellCount.Y, cellCount.Z];

            for (int i = 0; i < _cellCount.X; i++)
            {
                for (int j = 0; j < _cellCount.Y; j++)
                {
                    for (int k = 0; k < _cellCount.Z; k++)
                    {
                        Vector3 center = new Vector3(_cellSize.X * (i + 0.5), _cellSize.Y * (j + 0.5), _cellSize.Z * (k + 0.5));
                        _cells[i, j, k] = new Cell(center);
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
        public Cell GetContainingCell(ref Vector3 position)
        {
            (int X, int Y, int Z) containingCellIndex;

            position.X = position.X < 0.0 ? position.X + _spaceSize.X : position.X;
            position.X = position.X > _spaceSize.X ? position.X - _spaceSize.X : position.X;

            position.Y = position.Y < 0.0 ? position.Y + _spaceSize.Y : position.Y;
            position.Y = position.Y > _spaceSize.Y ? position.Y - _spaceSize.Y : position.Y;

            position.Z = position.Z < 0.0 ? position.Z + _spaceSize.Z : position.Z;
            position.Z = position.Z > _spaceSize.Z ? position.Z - _spaceSize.Z : position.Z;

            containingCellIndex.X = (int)Math.Floor(position.X / _cellSize.X);// % (CellCount.X + 1);
            containingCellIndex.Y = (int)Math.Floor(position.Y / _cellSize.Y);// % (CellCount.Y + 1);
            containingCellIndex.Z = (int)Math.Floor(position.Z / _cellSize.Z);// % (CellCount.Z + 1);

            //containingCellIndex.X = containingCellIndex.X == CellCount.X ? containingCellIndex.X - 1 : containingCellIndex.X;
            //containingCellIndex.Y = containingCellIndex.Y == CellCount.Y ? containingCellIndex.Y - 1 : containingCellIndex.Y;
            //containingCellIndex.Z = containingCellIndex.Z == CellCount.Z ? containingCellIndex.Z - 1 : containingCellIndex.Z;

            return _cells[containingCellIndex.X, containingCellIndex.Y, containingCellIndex.Z];
        }

        /// <summary>
        /// Перемещает частицы в соответствующие ячейки.
        /// </summary>
        public void RedistributeParticles()
        {
            ForEachCell((cell, cellIndicies) =>
            {
                IList<Particle> particles = cell.Particles;

                for (int i = 0; i < particles.Count; i++)
                {
                    Particle particle = particles[i];
                    var containingCell = GetContainingCell(ref particle.Position);

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
            int ByModulo(int value, int modulo)
            {
                if (modulo == 1)
                {
                    return 0;
                }

                if (value >= 0)
                {
                    return value % modulo;
                }

                int rest = value % modulo;
                return rest != 0 ? rest + modulo : rest;
            }

            double interactionRadius = Math.Sqrt(_interactionRadiusSquared);
            (int X, int Y, int Z) nearestCellCount;
            nearestCellCount.X = (int)Math.Ceiling(interactionRadius / _cellSize.X) - 1;
            nearestCellCount.Y = (int)Math.Ceiling(interactionRadius / _cellSize.Y) - 1;
            nearestCellCount.Z = (int)Math.Ceiling(interactionRadius / _cellSize.Z) - 1;

            ForEachCell((cell, cellIndicies) =>
            {
                //Vector3 nearestCellPosition;

                //nearestCellPosition.X = cell.Position.X - nearestCellCount.X * _cellSize.X;
                int x = ByModulo(cellIndicies.X - nearestCellCount.X, _cellCount.X);

                for (int xCounter = 0; xCounter < 2 * nearestCellCount.X + 1; xCounter++)
                {
                    //nearestCellPosition.Y = cell.Position.Y - nearestCellCount.Y * _cellSize.Y;
                    int y = ByModulo(cellIndicies.Y - nearestCellCount.Y, _cellCount.Y);

                    for (int yCounter = 0; yCounter < 2 * nearestCellCount.Y + 1; yCounter++)
                    {
                        //nearestCellPosition.Z = cell.Position.Z - nearestCellCount.Z * _cellSize.Z;
                        int z = ByModulo(cellIndicies.Z - nearestCellCount.Z, _cellCount.Z);

                        for (int zCounter = 0; zCounter < 2 * nearestCellCount.Z + 1; zCounter++)
                        {
                            if (cellIndicies.X != x || cellIndicies.Y != y || cellIndicies.Z != z)
                            {
                                //if ((cell.Position - nearestCellPosition).NormSquared() < _interactionRadiusSquared)
                                //{
                                //    cell.BoundaryCells.Add(_cells[x, y, z]);
                                //}

                                cell.BoundaryCells.Add(_cells[x, y, z]);
                            }

                            //nearestCellPosition.Z += _cellSize.Z;
                            z = ByModulo(z + 1, _cellCount.Z);
                        }

                        //nearestCellPosition.Y += _cellSize.Y;
                        y = ByModulo(y + 1, _cellCount.Y);
                    }

                    //nearestCellPosition.X += _cellSize.X;
                    x = ByModulo(x + 1, _cellCount.X);
                }
            });
        }

        /// <summary>
        /// Выполняет указанное действие для каждой ячейки сетки частиц.
        /// </summary>
        /// <param name="action">Действие для каждоЙ ячейки.</param>
        public void ForEachCell(Action<Cell, (int X, int Y, int Z)> action)
        {
            int perCore = _cellCount.X / (_tasks.Length + 1);

            for (int taskCounter = 0; taskCounter < _tasks.Length; taskCounter++)
            {
                int counter = taskCounter;
                _tasks[taskCounter] = Task.Run(() =>
                {
                    for (int i = counter * perCore; i < (counter + 1) * perCore; i++)
                    {
                        for (int j = 0; j < _cellCount.Y; j++)
                        {
                            for (int k = 0; k < _cellCount.Z; k++)
                            {
                                action(_cells[i, j, k], (i, j, k));
                            }
                        }
                    }
                });
            }

            for (int i = _tasks.Length * perCore; i < _cellCount.X; i++)
            {
                for (int j = 0; j < _cellCount.Y; j++)
                {
                    for (int k = 0; k < _cellCount.Z; k++)
                    {
                        action(_cells[i, j, k], (i, j, k));
                    }
                }
            }

            Task.WaitAll(_tasks);
        }

        /// <summary>
        /// Добавляет частицу в сетку.
        /// </summary>
        /// <param name="particle">Добавляемая частица.</param>
        public void AddParticle(Particle particle)
        {
            GetContainingCell(ref particle.Position).Particles.Add(particle);
        }

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
