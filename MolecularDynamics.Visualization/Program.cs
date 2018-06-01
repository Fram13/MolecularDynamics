using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using MolecularDynamics.Model;
using MolecularDynamics.Model.Atoms;

namespace MolecularDynamics.Visualization
{
    internal class Program
    {
        private static readonly SimulationParameters Parameters = new SimulationParameters(
            spaceSize: (10.5 * Wolfram.GridConstant, 10.5 * Wolfram.GridConstant, 21 * Wolfram.GridConstant),
            cellCount: (20, 20, 40))
        {
            IntegrationStep = 0.1,
            DissipationCoefficient = 0.06 / 3.615,
            Temperature = 200,
            ParticleMass = Wolfram.AtomMass,
            StaticCellLayerCount = 6,
            InteractionRadius = 4,
            NewParticleVelocity = Math.Sqrt(3.0 * Constants.BoltzmannConstant * Wolfram.MeltingPoint / Wolfram.AtomMass),
            Threads = 4
        };

        private static Random random = new Random();

        private static List<Particle> particles;
        private static ParticleGrid grid;
        private static TrajectoryIntegrator integrator;
        private static double nextParticleTime;
        private static int realTotalTime = 0;

        private static void Integrate(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                integrator.NextStep();
                nextParticleTime += Parameters.IntegrationStep;
                Parameters.TotalTime += Parameters.IntegrationStep;

                if (nextParticleTime > 400)
                {
                    Vector3 position;
                    position.X = Parameters.SpaceSize.X * random.NextDouble();
                    position.Y = Parameters.SpaceSize.Y * random.NextDouble();
                    position.Z = Parameters.SpaceSize.Z - Wolfram.Radius;

                    Particle particle = new Wolfram()
                    {
                        Position = position,
                        Velocity = (0, 0, -Parameters.NewParticleVelocity)
                    };

                    grid.AddParticle(particle);
                    particles.Add(particle);

                    nextParticleTime = 0.0;
                } 
            }
        }

        private static void ShowStatistics(CancellationToken ct)
        {
            Console.WriteLine("Время моделирования, 1е-14 с\t\tВремя интегрирования, с\t\tКоличество частиц\t\tТемпература, К");

            while (!ct.IsCancellationRequested)
            {
                Console.WriteLine($"{Math.Round(Parameters.TotalTime, 4)}\t\t{realTotalTime}\t\t{particles.Count}\t\t{Math.Round(particles.Temperature(), 4)}");
                Console.WriteLine();

                Thread.Sleep(5000);
                realTotalTime += 5;
            }
        }

        private static void Main(string[] args)
        {
            grid = new ParticleGrid(Parameters);
            particles = ParticleGenerator.GenerateWolframGrid((1.5, 1.5, 1.5), (10, 10, 20), Parameters.StaticCellLayerCount);
            grid.AddParticles(particles);

            integrator = new TrajectoryIntegrator(grid, Parameters);

            using (ParticleVisualizer particleVisualizer = new ParticleVisualizer(particles, integrator, Parameters.SpaceSize, Wolfram.Radius))
            {
                particleVisualizer.UpdateParticles = Integrate;
                particleVisualizer.UpdateStatistics = ShowStatistics;

                particleVisualizer.Run(30);
            }
        }

        private static void SerializeParticles(Stream stream, List<Particle> particleList)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, particleList);
        }

        private static List<Particle> DeserializeParticles(Stream stream)
        {
            BinaryFormatter bf = new BinaryFormatter();
            return (List<Particle>)bf.Deserialize(stream);
        }
    }
}
