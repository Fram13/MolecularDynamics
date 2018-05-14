using System;
using System.Collections.Generic;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет генератор частиц.
    /// </summary>
    public static class ParticleGenerator
    {
        /// <summary>
        /// Генерирует частицы, расположенные в пересечениях строк и столбцов объемной сетки.
        /// </summary>
        /// <param name="grid">Заполняемая сетка.</param>
        /// <param name="mass">Масса частицы.</param>
        /// <param name="interactionFunction">Функция, вычисляющая ускорение взаимодействия пары частиц.</param>
        public static List<Particle> GenerateParticles(this ParticleGrid grid, double mass, Func<Particle, Particle, Vector3> interactionFunction)
        {
            //List<Particle> particles = new List<Particle>();

            //grid.ForEachCell((cell, indicies) =>
            //{
            //    double x = cell.Position.X - grid.CellSize.X * 0.25;

            //    for (int i = 0; i < 2; i++)
            //    {
            //        double y = cell.Position.Y - grid.CellSize.Y * 0.25;

            //        for (int j = 0; j < 2; j++)
            //        {
            //            double z = cell.Position.Z - grid.CellSize.Z * 0.25;

            //            for (int k = 0; k < 2; k++)
            //            {
            //                Particle particle = new Particle()
            //                {
            //                    InteractionFunction = interactionFunction,
            //                    Mass = mass,
            //                    Position = (x, y, z)
            //                };

            //                cell.Particles.Add(particle);

            //                lock (particles)
            //                {
            //                    particles.Add(particle);
            //                }

            //                z += grid.CellSize.X * 0.5;
            //            }

            //            y += grid.CellSize.Y * 0.5;
            //        }

            //        x += grid.CellSize.Z * 0.5;
            //    }
            //});

            //return particles;

            List<Particle> particles = new List<Particle>();

            grid.ForEachCell((cell, indicies) =>
            {
                Particle particle = new Particle()
                {
                    InteractionFunction = interactionFunction,
                    Mass = mass,
                    Position = cell.Position
                };

                cell.Particles.Add(particle);

                lock (particles)
                {
                    particles.Add(particle);
                }
            });

            return particles;
        }
    }
}
