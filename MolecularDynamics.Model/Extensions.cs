using System;
using System.Collections.Generic;
using MolecularDynamics.Model.Atoms;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет методы для вычисления параметров системы частиц.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Вычисляет температуру системы частиц, К.
        /// </summary>
        /// <param name="particles">Список частиц, образующих систему частиц.</param>
        public static double Temperature(this List<Particle> particles)
        {
            double avgVel = 0.0;
            int count = 0;

            for (int i = 0; i < particles.Count; i++)
            {
                if (particles[i].Static || particles[i].Free)
                {
                    continue;
                }
                
                avgVel += particles[i].Velocity.NormSquared();
                count++;
            }

            avgVel /= count;

            return Atoms.Wolfram.AtomMass * avgVel / 3.0 / Constants.BoltzmannConstant;
        }

        /// <summary>
        /// Вычисляет кинетическую энергию частицы.
        /// </summary>
        /// <param name="particle">Частица, для которой необходимо вычислить кинетическую энергию.</param>
        /// <returns></returns>
        public static double Energy(this Particle particle)
        {
            return particle.Mass * particle.Velocity.NormSquared() / 2.0;
        }

        /// <summary>
        /// Вычисляет плотность вещества по слоям кристаллической решетки.
        /// </summary>
        /// <param name="particles">Список частиц, образующих систему частиц.</param>
        /// <param name="parameters">Параметры моделирования.</param>
        /// <returns></returns>
        public static List<double> LayersDensity(this List<Particle> particles, SimulationParameters parameters)
        {
            int layers = (int)Math.Ceiling(parameters.SpaceSize.Z / Wolfram.GridConstant);
            double layersVolume = Wolfram.GridConstant * parameters.SpaceSize.X * parameters.SpaceSize.Y;
            double min = 0.0;
            double max = Wolfram.GridConstant;
            List<double> density = new List<double>();

            for (int i = 0; i < layers; i++)
            {
                int count = 0;

                for (int j = 0; j < particles.Count; j++)
                {
                    if (!(particles[j].Position.Z < min) && particles[j].Position.Z < max)
                    {
                        count++;
                    }
                }

                density.Add(count * Wolfram.AtomMass / layersVolume);

                min = max;
                max += Wolfram.GridConstant;
            }

            return density;
        }
    }
}
