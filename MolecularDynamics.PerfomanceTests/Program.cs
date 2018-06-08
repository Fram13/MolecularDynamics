using MolecularDynamics.Model;
using MolecularDynamics.Model.Atoms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MolecularDynamics.PerfomanceTests
{
    class Program
    {
        private static SimulationParameters CreateParameters((int X, int Y, int Z) atomCount, int staticLayerCount, int threads)
        {
            Vector3 spaceSize;
            spaceSize.X = atomCount.X * Wolfram.GridConstant;
            spaceSize.Y = atomCount.Y * Wolfram.GridConstant;
            spaceSize.Z = atomCount.Z * Wolfram.GridConstant * 1.5;

            var cellCount = (atomCount.X * 2, atomCount.Y * 2, atomCount.Z * 3);

            SimulationParameters parameters = new SimulationParameters(spaceSize, cellCount)
            {
                IntegrationStep = 0.1,
                DissipationCoefficient = 0.06 / 3.615,
                Temperature = 200,
                ParticleMass = Wolfram.AtomMass,
                StaticCellLayerCount = staticLayerCount,
                InteractionRadius = 5,
                NewParticleVelocity = Math.Sqrt(3.0 * Constants.BoltzmannConstant * Wolfram.BoilingPoint / Wolfram.AtomMass),
                ParticleAppearancePeriod = 200,
                Threads = threads
            };

            return parameters;
        }

        private static void Test()
        {
            var cellCount = (20, 20, 20);
            int staticLayerCount = 2;

            for (int i = 0; i < 15; i++)
            {
                SimulationParameters parameters = CreateParameters(cellCount, staticLayerCount, i + 1);
                List<Particle> particles = ParticleGenerator.GenerateWolframGrid((0.7, 0.7, 0.7), cellCount, staticLayerCount);
                ParticleGrid grid = new ParticleGrid(parameters);
                grid.AddParticles(particles);
                TrajectoryIntegrator integrator = new TrajectoryIntegrator(grid, parameters);
                Stopwatch sw = new Stopwatch();

                sw.Start();

                for (int k = 0; k < 1000; k++)
                {
                    integrator.NextStep();
                }

                sw.Stop();

                Console.WriteLine($"Particles: {particles.Count}; Threads: {i + 1}; 1000 steps in {sw.ElapsedMilliseconds}; {sw.ElapsedMilliseconds / 1000.0} per step");

                sw.Reset();
            }            
        }

        static void Main(string[] args)
        {
            Test();
            Console.ReadKey();
        }
    }
}
