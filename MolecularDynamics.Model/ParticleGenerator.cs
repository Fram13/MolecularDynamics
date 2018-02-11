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
        /// Генерирует частицы, расположенные в пересечениях строк и столбцов плоской сетки.
        /// </summary>
        /// <param name="rows">Количество строк сетки.</param>
        /// <param name="columns">Количетсво столбцов сетки.</param>
        /// <param name="mass">Масса частицы.</param>
        /// <param name="interactionFunction">Функция, вычисляющая ускорение взаимодействия пары частиц.</param>
        public static IList<Particle> Generate2DGrid(int rows, int columns, double mass, Func<Particle, Particle, Vector3> interactionFunction)
        {
            IList<Particle> particles = new List<Particle>(rows * columns);

            double XStep = columns != 0 ? 1.0 / (columns - 1) : 0.0;
            double YStep = rows != 0 ? 1.0 / (rows - 1) : 0.0;

            double x = 0.0;

            for (int i = 0; i < rows; i++)
            {
                double y = 0.0;

                for (int j = 0; j < columns; j++)
                {
                    particles.Add(new Particle()
                    {
                        Position = (x, y, 0.0),
                        Mass = mass,
                        InteractionFunction = interactionFunction
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
        /// <param name="mass">Масса частицы.</param>
        /// <param name="interactionFunction">Функция, вычисляющая ускорение взаимодействия пары частиц.</param>
        public static IList<Particle> Generate3DGrid(int layers, int rows, int columns, double mass, Func<Particle, Particle, Vector3> interactionFunction)
        {
            IList<Particle> particles = new List<Particle>(layers * rows * columns);

            double XStep = columns != 0 ? 1.0 / (columns - 1) : 0.0;
            double YStep = rows != 0 ? 1.0 / (rows - 1) : 0.0;
            double ZStep = layers != 0 ? 1.0 / (layers - 1) : 0.0;

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
                            Mass = mass,
                            InteractionFunction = interactionFunction
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
