using System;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет генератор частиц.
    /// </summary>
    public static class ParticleGridGenerator
    {
        /// <summary>
        /// Генерирует частицы, расположенные в пересечениях строк и столбцов объемной сетки.
        /// </summary>
        /// <param name="spaceSize"></param>
        /// <param name="cellCount"></param>
        /// <param name="mass">Масса частицы.</param>
        /// <param name="interactionFunction">Функция, вычисляющая ускорение взаимодействия пары частиц.</param>
        /// <param name="interactionRadius"></param>
        public static ParticleGrid GenerateGrid(Vector3 spaceSize, (int X, int Y, int Z) cellCount, double mass, Func<Particle, Particle, Vector3> interactionFunction, double interactionRadius)
        {
            return GenerateGrid(spaceSize, cellCount, interactionRadius, (grid, cell) =>
            {
                cell.Particles.Add(new Particle()
                {
                    InteractionFunction = interactionFunction,
                    Mass = mass,
                    Position = cell.Position
                });

                double x = cell.Position.X - grid.CellSize.X * 0.25;

                for (int i = 0; i < 2; i++)
                {
                    double y = cell.Position.Y - grid.CellSize.Y * 0.25;

                    for (int j = 0; j < 2; j++)
                    {
                        double z = cell.Position.Z - grid.CellSize.Z * 0.25;

                        for (int k = 0; k < 2; k++)
                        {
                            cell.Particles.Add(new Particle()
                            {
                                InteractionFunction = interactionFunction,
                                Mass = mass,
                                Position = (x, y, z)
                            });

                            z += grid.CellSize.X * 0.5;
                        }

                        y += grid.CellSize.Y * 0.5;
                    }

                    x += grid.CellSize.Z * 0.5;
                }
            });
        }

        private static ParticleGrid GenerateGrid(Vector3 spaceSize, (int X, int Y, int Z) cellCount, double interactionRadius, Action<ParticleGrid, ParticleGridCell> generationAction)
        {
            ParticleGrid grid = new ParticleGrid(spaceSize, cellCount, interactionRadius);

            for (int i = 0; i < grid.CellCount.X; i++)
            {
                for (int j = 0; j < grid.CellCount.Y; j++)
                {
                    for (int k = 0; k < grid.CellCount.Z; k++)
                    {
                        generationAction(grid, grid[i, j, k]);
                    }
                }
            }

            return grid;
        }
    }
}
