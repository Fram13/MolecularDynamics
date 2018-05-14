using System;
using System.Diagnostics;
using MolecularDynamics.Model;

namespace MolecularDynamics.PerformanceTests
{
    class Program
    {
        /// <summary>
        /// Измеряет время моделирования движения частиц.
        /// </summary>
        /// <param name="steps">Количество шагов интегрирования.</param>
        /// <returns></returns>
        private static long TimeTest(int steps)
        {
            Stopwatch sw = new Stopwatch();

            ParticleGrid grid = new ParticleGrid(spaceSize: (1, 1, 1), cellCount: (25, 25, 25), interactionRadius: 0.18, threads: 1);
            grid.GenerateParticles(mass: 1, interactionFunction: InteractionFunctions.PseudoGravitationInteraction);
            TrajectoryIntegrator integrator = new TrajectoryIntegrator(grid, step: 0.05);

            sw.Start();

            for (int i = 0; i < steps; i++)
            {
                integrator.NextStep();
            }

            sw.Stop();

            return sw.ElapsedMilliseconds;
        }

        static void Main(string[] args)
        {
            int steps = 40;

            long time1 = TimeTest(steps);
            Console.WriteLine($"Total: {time1} ms, per step: {(double)time1 / steps} ms");
            Console.ReadKey();
        }
    }
}
