using System;
using System.Collections.Generic;
using MolecularDynamics.Model.Atoms;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет генератор частиц.
    /// </summary>
    public static class ParticleGenerator
    {
        /// <summary>
        /// Создает сетку и заполняет ее атомами вольфрама, образующими ОЦК решетку.
        /// </summary>
        /// <param name="spaceSize">Размер пространства моделирования.</param>
        /// <param name="interactionRadius">Радиус взаимодействия ячеек.</param>
        /// <param name="threads">Количество потоков вычислений.</param>
        /// <returns></returns>
        public static (ParticleGrid, List<Particle>) GenerateWolframGrid(Vector3 spaceSize, double interactionRadius, int threads)
        {
            double cellSize = 3.16 / 2; //А
            (int X, int Y, int Z) cellCount;
            cellCount.X = (int)Math.Ceiling(spaceSize.X / cellSize);
            cellCount.Y = (int)Math.Ceiling(spaceSize.Y / cellSize);
            cellCount.Z = (int)Math.Ceiling(spaceSize.Z / cellSize);

            ParticleGrid grid = new ParticleGrid(spaceSize, cellCount, interactionRadius, threads);
            List<Particle> particles = new List<Particle>();
            Object syncObject = new Object();

            grid.ForEachCell((cell, cellIndicies) =>
            {
                int rest = cellIndicies.Z % 2;

                if (rest == cellIndicies.X % 2 && rest == cellIndicies.Y % 2)
                {
                    Particle particle = new Wolfram()
                    {
                        Position = cell.Position
                    };

                    cell.Particles.Add(particle);

                    lock (syncObject)
                    {
                        particles.Add(particle); 
                    }
                }
            });

            return (grid, particles);
        }
    }
}
