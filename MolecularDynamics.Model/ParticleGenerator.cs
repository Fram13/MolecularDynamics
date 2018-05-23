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
        public static (ParticleGrid, List<Particle>) GenerateWolframGrid(IntegrationParameters parameters, double interactionRadius, int threads)
        {
            double delta = parameters.CellSize.X * 0.25;
            int emptyLayerCount = parameters.CellCount.Y / 2;

            ParticleGrid grid = new ParticleGrid(parameters.SpaceSize, parameters.CellCount, parameters.CellSize, interactionRadius, threads);
            List<Particle> particles = new List<Particle>();
            Object syncObject = new Object();

            grid.ForEachCell((cell, cellIndicies) =>
            {
                if (cellIndicies.Y >= emptyLayerCount)
                {
                    return;
                }

                Vector3 p = cell.Position;

                p.X -= delta;
                p.Y -= delta;
                p.Z -= delta;

                Particle p1 = new Wolfram()
                {
                    Position = p
                };

                p.X += parameters.CellSize.X / 2.0;
                p.Y += parameters.CellSize.Y / 2.0;
                p.Z += parameters.CellSize.Z / 2.0;

                Particle p2 = new Wolfram()
                {
                    Position = p
                };

                cell.Particles.Add(p1);
                cell.Particles.Add(p2);

                lock (particles)
                {
                    particles.Add(p1);
                    particles.Add(p2); 
                }
            });

            return (grid, particles);
        }
    }
}
