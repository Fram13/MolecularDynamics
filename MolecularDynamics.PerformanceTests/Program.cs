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
        /// <param name="rows">Количество строк сетки частиц.</param>
        /// <param name="columns">Количество столбцов сетки частиц.</param>
        /// <param name="steps">Количество шагов интегрирования.</param>
        /// <param name="threads">Количество потоков.</param>
        /// <returns></returns>
        private static long TimeTest(int rows, int columns, int steps, int threads)
        {
            var particles = ParticleGenerator.Generate2DGrid(rows, columns, 1, InteractionFunctions.GravitationalInteraction);
            var integrator = new ParticleTrajectoryIntegrator(particles, 1, threads);

            Stopwatch sw = new Stopwatch();
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
            int steps = 10;

            long time1 = TimeTest(100, 10, steps, 1);
            Console.WriteLine("1 thread");
            Console.WriteLine($"Total: {time1} ms, per step: {(double)time1 / steps} ms");
            Console.WriteLine();

            long time2 = TimeTest(100, 10, steps, 2);
            Console.WriteLine("2 threads");
            Console.WriteLine($"Total: {time2} ms, per step: {(double)time2 / steps} ms");
            Console.WriteLine();

            long time3 = TimeTest(100, 10, steps, 4);
            Console.WriteLine("4 threads");
            Console.WriteLine($"Total: {time3} ms, per step: {(double)time3 / steps} ms");
            Console.WriteLine();

            long time4 = TimeTest(100, 10, steps, 8);
            Console.WriteLine("8 threads");
            Console.WriteLine($"Total: {time4} ms, per step: {(double)time4 / steps} ms");
            Console.WriteLine();

            Console.ReadKey();
        }
    }
}
