using System;
using System.Collections.Generic;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Предоставляет методы для заполнения сетки частицами.
    /// </summary>
    public static class ParticleGenerator
    {
        public static Tuple<ParticleGrid, List<Particle>> GenerateWolframGrid(Vector3 spaceSize, int threads)
        {
            double cellSize = 3.16 / 2; //A

            (int X, int Y, int Z) cellCount;
            cellCount.X = (int)Math.Ceiling(spaceSize.X / cellSize);
            cellCount.Y = (int)Math.Ceiling(spaceSize.Y / cellSize);
            cellCount.Z = (int)Math.Ceiling(spaceSize.Z / cellSize);

            ParticleGrid grid = new ParticleGrid(spaceSize, cellCount, 10 * cellSize, threads);
            List<Particle> particles = new List<Particle>();

            for (int z = 0; z < grid.CellCount.Z; z++)
            {
                for (int x = z % 2; x < grid.CellCount.X; x += 2)
                {
                    for (int y = z % 2; y < grid.CellCount.Y; y += 2)
                    {
                        Particle particle = new Particle()
                        {
                            Position = grid[x, y, z].Position,
                            RandomForce = Constants.RandomForceLength * Constants.RandomNormalVector,
                            Mass = Constants.WolframAtomMass
                        };

                        grid[x, y, z].Particles.Add(particle);
                        particles.Add(particle);
                    }
                }
            }

            return Tuple.Create(grid, particles);
        }
    }
}
