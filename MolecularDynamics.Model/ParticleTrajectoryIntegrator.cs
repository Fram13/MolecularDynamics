using System;
using System.Collections.Generic;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет интегратор траекторий частиц.
    /// </summary>
    public partial class ParticleTrajectoryIntegrator
    {
        private IList<Particle> particles;
        private double step;
        private double halfStep;
        private double g;

        private TrajectoryIntegrationThread[] threads;

        /// <summary>
        /// Создает новый экземпляр <see cref="ParticleTrajectoryIntegrator"/>.
        /// </summary>
        /// <param name="particles">Список частиц, образующих модерируемое вещество.</param>
        /// <param name="step">Шаг интегрирования.</param>
        /// <param name="threadCount"></param>
        /// <param name="g"></param>
        public ParticleTrajectoryIntegrator(IList<Particle> particles, double step, double g, int threadCount)
        {
            this.particles = particles;
            this.step = step;
            halfStep = step / 2.0;
            this.g = g;

            int count = particles.Count;
            int particlesPerThread = count / threadCount;

            if (particles.Count % threadCount != 0)
            {
                threadCount++;
            }

            threads = new TrajectoryIntegrationThread[threadCount];

            int index = 0;

            for (int i = 0; i < threads.Length; i++)
            {
                if (count > particlesPerThread)
                {
                    threads[i] = new TrajectoryIntegrationThread(this, index, index + particlesPerThread);
                    index += particlesPerThread;
                    count -= particlesPerThread;
                }
                else
                {
                    threads[i] = new TrajectoryIntegrationThread(this, index, index + count);
                }
            }
        }

        public void InitialStep()
        {
            Vector3[] velocity = new Vector3[particles.Count];

            for (int i = 0; i < particles.Count; i++)
            {
                Vector3 acc = Acceleration(i);
                Particle curr = particles[i];

                velocity[i] = curr.InitialVelocity + acc * step;
                curr.NextPosition = curr.Position + velocity[i] * step;
            }

            CompleteStep();
        }

        public void NextStep()
        {
            foreach (var thread in threads)
            {
                thread.NextStep();
            }

            foreach (var thread in threads)
            {
                thread.Wait();
            }

            CompleteStep();
        }

        private void NextStep(Object args)
        {
            int startIndex = (args as Tuple<int, int>).Item1;
            int endIndex = (args as Tuple<int, int>).Item2;

            for (int i = startIndex; i < endIndex; i++)
            {
                Particle curr = particles[i];
                Vector3 acc = Acceleration(i);
                curr.NextPosition = 2.0 * curr.Position - curr.PreviousPosition + acc * step * step;
            }
        }

        private void NextStep(int startIndex, int endIndex)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                Particle curr = particles[i];
                Vector3 acc = Acceleration(i);
                curr.NextPosition = 2.0 * curr.Position - curr.PreviousPosition + acc * step * step;
            }
        }

        private Vector3 Acceleration(int i)
        {
            Particle curr = particles[i];
            Vector3 acc = (0.0, 0.0, 0.0);
            Vector3 r;
            Vector3 re;
            double n;
            int j = 0;
            Particle other;

            for (; j < i; j++)
            {
                other = particles[j];
                r = other.Position - curr.Position;
                n = r.Norm();
                re = r / n;

                acc += re * particles[j].Mass / (n * n);
            }

            j++;

            for (; j < particles.Count; j++)
            {
                other = particles[j];
                r = other.Position - curr.Position;
                n = r.Norm();
                re = r / n;

                acc += re * particles[j].Mass / (n * n);
            }

            return acc * g;
        }

        private void CompleteStep()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                Particle curr = particles[i];
                curr.PreviousPosition = curr.Position;
                curr.Position = curr.NextPosition;
            }
        }
    }
}
