using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MolecularDynamics.Model;
using MolecularDynamics.Model.Atoms;

namespace MolecularDynamics.Visualization
{
    internal class Program
    {
        private static readonly IntegrationParameters Parameters = new IntegrationParameters()
        {
            IntegrationStep = 0.1,
            DissipationCoefficient = 0.06 / 3.615,
            Temperature = 200
        };

        private static void Main(string[] args)
        {
            ParticleGrid grid = new ParticleGrid((80, 10, 10), (1, 1, 1), 0, 1);
            List<Particle> particles = new List<Particle>();

            Particle p1 = new WolframAtom()
            {
                Position = (28.5, 0, 0)
            };

            Particle p2 = new WolframAtom()
            {
                Position = (31, 0, 0)
            };

            grid.AddParticle(p1);
            grid.AddParticle(p2);

            particles.Add(p1);
            particles.Add(p2);

            TrajectoryIntegrator integrator = new TrajectoryIntegrator(grid, Parameters);

            using (ParticleVisualizer particleVisualizer = new ParticleVisualizer(particles, WolframAtom.Radius))
            {
                Task.Run(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(5);

                        integrator.NextStep();

                        float[] nextPositions = particleVisualizer.NextPositionsComponents;

                        for (int i = 0; i < particles.Count; i++)
                        {
                            nextPositions[3 * i] = (float)(particles[i].Position.X / 10.0);
                            nextPositions[3 * i + 1] = (float)(particles[i].Position.Y / 10.0);
                            nextPositions[3 * i + 2] = (float)(particles[i].Position.Z / 10.0);
                        }

                        particleVisualizer.SwapPositionsBuffers();
                    }
                });

                particleVisualizer.Run(30);
            }
        }
    }
}
