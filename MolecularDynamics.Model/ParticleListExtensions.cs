using System;
using System.Collections.Generic;
using System.Text;

namespace MolecularDynamics.Model
{
    public static class ParticleListExtensions
    {
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
    }
}
