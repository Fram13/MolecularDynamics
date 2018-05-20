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
            Temperature = 200,
            ParticleMass = Wolfram.AtomMass
        };

        private static void Main(string[] args)
        {
            var (grid, particles) = ParticleGenerator.GenerateWolframGrid((32, 32, 32), 13, 4);
            //ParticleGrid grid = new ParticleGrid((20, 20, 20), (1, 1, 1), 0, 1);
            //List<Particle> particles = new List<Particle>();

            //Particle p1 = new Wolfram()
            //{
            //    Position = (9, 10, 10)
            //};

            //Particle p2 = new Wolfram()
            //{
            //    Position = (11.8, 10, 10)
            //};

            //grid.AddParticle(p1);
            //grid.AddParticle(p2);
            //particles.Add(p1);
            //particles.Add(p2);

            TrajectoryIntegrator integrator = new TrajectoryIntegrator(grid, Parameters);

            using (ParticleVisualizer particleVisualizer = new ParticleVisualizer(particles, Wolfram.Radius))
            {
                Task.Run(() =>
                {
                    while (true)
                    {
                        //Thread.Sleep(50);
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
