using System.Collections.Generic;

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

            for (int i = 0; i < particles.Count; i++)
            {
                avgVel += particles[i].Velocity.NormSquared();
            }

            avgVel /= particles.Count;

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
    }
}
