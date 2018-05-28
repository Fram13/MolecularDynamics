using System;
using System.Collections.Generic;
using System.Threading;
using MolecularDynamics.Model;
using MolecularDynamics.Model.Atoms;

namespace MolecularDynamics.Visualization
{
    internal class Program
    {
        private static readonly SimulationParameters Parameters = new SimulationParameters(
            spaceSize: (6.5 * Wolfram.GridConstant, 6.5 * Wolfram.GridConstant, 14 * Wolfram.GridConstant),
            cellCount: (14, 14, 28))
        {
            IntegrationStep = 0.1,
            DissipationCoefficient = 0.06 / 3.615,
            Temperature = 200,
            ParticleMass = Wolfram.AtomMass,
            StaticCellLayerCount = 6,
            InteractionRadius = 4,
            Threads = 4
        };

        private static Random random = new Random();

        private static List<Particle> particles;
        private static ParticleGrid grid;
        private static TrajectoryIntegrator integrator;
        private static double nextParticleTime;
        private static double totalTime;
        private static int realTotalTime;

        private static void Integrate(List<Particle> particleList)
        {
            integrator.NextStep();
            nextParticleTime += Parameters.IntegrationStep;
            totalTime += Parameters.IntegrationStep;

            if (nextParticleTime > 400)
            {
                Vector3 position;
                position.X = Parameters.SpaceSize.X * random.NextDouble();
                position.Y = Parameters.SpaceSize.Y * random.NextDouble();
                position.Z = Parameters.SpaceSize.Z - Wolfram.Radius;

                Particle particle = new Wolfram()
                {
                    Position = position,
                    Velocity = (0, 0, -0.4)
                };

                grid.AddParticle(particle);
                particleList.Add(particle);

                nextParticleTime = 0.0;
            }
        }

        private static void ShowStatistics(CancellationToken ct)
        {
            Console.WriteLine("Время моделирования, 1е-14 с\t\tВремя интегрирования, с\t\tКоличество частиц\t\tТемпература, К");

            while (!ct.IsCancellationRequested)
            {
                Console.WriteLine($"{Math.Round(totalTime, 4)}\t\t{realTotalTime}\t\t{particles.Count}\t\t{Math.Round(particles.Temperature(), 4)}");
                Console.WriteLine();

                Thread.Sleep(5000);
                realTotalTime += 5;
            }
        }

        private static void Main(string[] args)
        {
            grid = new ParticleGrid(Parameters.SpaceSize,
                                                 Parameters.CellCount,
                                                 Parameters.CellSize,
                                                 Parameters.InteractionRadius,
                                                 Parameters.Threads);

            particles = grid.GenerateWolframGrid((1.5, 1.5, 1.5), (6, 6, 12), Parameters);

            integrator = new TrajectoryIntegrator(grid, Parameters);

            using (ParticleVisualizer particleVisualizer = new ParticleVisualizer(particles, Parameters.SpaceSize, Wolfram.Radius))
            {
                particleVisualizer.UpdateParticlesExternally = Integrate;
                particleVisualizer.UpdateStatistics = ShowStatistics;

                particleVisualizer.Run(30);
            }
        }
    }
}
