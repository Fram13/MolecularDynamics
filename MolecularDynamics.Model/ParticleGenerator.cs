using System;
using System.Collections.Generic;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет генератор частиц.
    /// </summary>
    public class ParticleGenerator
    {
        /// <summary>
        /// Создает новый экземпляр <see cref="ParticleGenerator"/>.
        /// </summary>
        public ParticleGenerator() { }

        /// <summary>
        /// Генерирует частицы, расположенные в пересечениях строк и столбцов плоской сетки.
        /// </summary>
        /// <param name="rows">Количество строк сетки.</param>
        /// <param name="columns">Количетсво столбцов сетки.</param>
        /// <returns></returns>
        public IList<Particle> Generate2DGrid(int rows, int columns)
        {
            List<Particle> particles = new List<Particle>(rows * columns);

            double XStep = 1.0 / (columns - 1);
            double YStep = 1.0 / (rows - 1);
            double radius = Math.Min(XStep, YStep) * 0.4;

            double x = 0.0;

            for (int i = 0; i < rows; i++)
            {
                double y = 0.0;

                for (int j = 0; j < columns; j++)
                {
                    particles.Add(new Particle()
                    {
                        Position = (x, y, 0.0),
                        Mass = 1,
                        Radius = radius
                    });

                    y += YStep;
                }

                x += XStep;
            }

            return particles;
        }

        /// <summary>
        /// Генерирует частицы, расположенные в пересечениях строк и столбцов объемной сетки.
        /// </summary>
        /// <param name="layers">Количество слоев сетки.</param>
        /// <param name="rows">Количество строк сетки.</param>
        /// <param name="columns">Количетсво столбцов сетки.</param>
        /// <returns></returns>
        public IList<Particle> Generate3DGrid(int layers, int rows, int columns)
        {
            List<Particle> particles = new List<Particle>(layers * rows * columns);

            double XStep = 1.0 / (columns - 1);
            double YStep = 1.0 / (rows - 1);
            double ZStep = 1.0 / (layers - 1);
            double radius = Math.Min(ZStep, Math.Min(XStep, YStep)) * 0.4;

            double z = 0.0;

            for (int k = 0; k < layers; k++)
            {
                double x = 0.0;

                for (int i = 0; i < rows; i++)
                {
                    double y = 0.0;

                    for (int j = 0; j < columns; j++)
                    {
                        particles.Add(new Particle()
                        {
                            Position = (x, y, z),
                            Mass = 1,
                            Radius = radius
                        });

                        y += YStep;
                    }

                    x += XStep;
                }

                z += ZStep;
            }

            return particles;
        }
    }
}
