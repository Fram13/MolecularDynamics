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
        /// <param name="parameters">Параметры моделирования частиц.</param>
        /// <returns></returns>
        public static (ParticleGrid, List<Particle>) GenerateWolframGrid(SimulationParameters parameters)
        {
            double delta = parameters.CellSize.X * 0.25;

            ParticleGrid grid = new ParticleGrid(parameters.SpaceSize,
                                                 parameters.CellCount,
                                                 parameters.CellSize, 
                                                 parameters.InteractionRadius, 
                                                 parameters.Threads);
            
            List<Particle> particles = new List<Particle>();

            grid.ForEachCell((cell, cellIndicies) =>
            {
                if (cellIndicies.Y >= parameters.CellLayerCount)
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
