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
            var (grid, particles) = ParticleGenerator.GenerateWolframGrid((16, 16, 16), 13, 4);
            TrajectoryIntegrator integrator = new TrajectoryIntegrator(grid, Parameters);

            using (ParticleVisualizer particleVisualizer = new ParticleVisualizer(particles, WolframAtom.Radius))
            {
                Task.Run(() =>
                {
                    while (true)
                    {
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
