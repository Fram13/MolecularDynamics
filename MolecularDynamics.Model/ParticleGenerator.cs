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
        public static ParticleGrid GenerateGrid(Vector3 spaceSize, (int X, int Y, int Z) cellCount, double mass, Func<Particle, Particle, Vector3> interactionFunction)
        {
            return GenerateGrid(spaceSize, cellCount, (grid, cell, origin) =>
            {
                cell.Particles.Add(new Particle()
                {
                    InteractionFunction = interactionFunction,
                    Mass = mass,
                    Position = origin + (grid.CellWidth / 2, grid.CellHeight / 2, grid.CellDepth / 2)
                });

                double x = grid.CellWidth * 0.25;

                for (int i = 0; i < 2; i++)
                {
                    double y = grid.CellHeight * 0.25;

                    for (int j = 0; j < 2; j++)
                    {
                        double z = grid.CellDepth * 0.25;

                        for (int k = 0; k < 2; k++)
                        {
                            cell.Particles.Add(new Particle()
                            {
                                InteractionFunction = interactionFunction,
                                Mass = mass,
                                Position = origin + (x, y, z)
                            });

                            cell.Forces.Add(new Vector3(0, 0, 0));

                            z += grid.CellDepth * 0.5;
                        }

                        y += grid.CellHeight * 0.5;
                    }

                    x += grid.CellWidth * 0.5;
                }

                cell.Forces.Add(new Vector3(0, 0, 0));
            });
        }

        private static ParticleGrid GenerateGrid(Vector3 spaceSize, (int X, int Y, int Z) cellCount, Action<ParticleGrid, ParticleGridCell, Vector3> generationAction)
        {
            ParticleGrid grid = new ParticleGrid(spaceSize, cellCount);

            ForEachCell(grid, generationAction);

            return grid;
        }

        private static void ForEachCell(ParticleGrid grid, Action<ParticleGrid, ParticleGridCell, Vector3> generationAction)
        {
            double x = 0.0;

            for (int i = 0; i < grid.Rows; i++)
            {
                double y = 0.0;

                for (int j = 0; j < grid.Columns; j++)
                {
                    double z = 0.0;

                    for (int k = 0; k < grid.Layers; k++)
                    {
                        generationAction(grid, grid[i, j, k], (x, y, z));

                        z += grid.CellDepth;
                    }

                    y += grid.CellHeight;
                }

                x += grid.CellWidth;
            }
        }
    }
}
