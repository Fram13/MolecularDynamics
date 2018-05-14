using System;
using System.Threading.Tasks;
using MolecularDynamics.Model;
using System.Collections.Generic;

namespace MolecularDynamics.Visualization
{
    class Program
    {
        static void Main(string[] args)
        {
            ParticleGrid grid = new ParticleGrid((1, 1, 1), (5, 5, 5), 0.5, 4);
            List<Particle> particles = grid.GenerateParticles(0.0001, InteractionFunctions.GravitationalInteraction);
            TrajectoryIntegrator integrator = new TrajectoryIntegrator(grid, 0.005);

            float[] positions = new float[particles.Count * 3];
            float[] nextPositions = new float[particles.Count * 3];

            for (int i = 0; i < particles.Count; i++)
            {
                positions[3 * i] = (float)particles[i].Position.X;
                positions[3 * i + 1] = (float)particles[i].Position.Y;
                positions[3 * i + 2] = (float)particles[i].Position.Z;
            }

            Object syncObject = new Object();
            
            using (ParticleVisualizer particleVisualizer = new ParticleVisualizer(positions, 0.05, 10, syncObject))
            {
                Task.Run(() =>
                {
                    while (true)
                    {
                        integrator.NextStep();

                        for (int i = 0; i < particles.Count; i++)
                        {
                            nextPositions[3 * i] = (float)particles[i].Position.X;
                            nextPositions[3 * i + 1] = (float)particles[i].Position.Y;
                            nextPositions[3 * i + 2] = (float)particles[i].Position.Z;
                        }

                        lock (syncObject)
                        {
                            var temp = positions;
                            positions = nextPositions;
                            nextPositions = temp;
                        }
                    }
                });

                particleVisualizer.Run(30);
            }
        }
    }
}