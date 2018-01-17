using System.Collections.Generic;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет интегратор траекторий частиц.
    /// </summary>
    public class ParticleTrajectoryIntegrator
    {
        private List<Particle> particles;
        private double step;
        private double halfStep;
        private double g;

        /// <summary>
        /// Создает новый экземпляр <see cref="ParticleTrajectoryIntegrator"/>.
        /// </summary>
        /// <param name="particles">Список частиц, образующих модерируемое вещество.</param>
        /// <param name="step">Шаг интегрирования.</param>
        /// <param name="g"></param>
        public ParticleTrajectoryIntegrator(List<Particle> particles, double step, double g)
        {
            this.particles = particles;
            this.step = step;
            halfStep = step / 2.0;
            this.g = g;
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
            for (int i = 0; i < particles.Count; i++)
            {
                Particle curr = particles[i];
                Vector3 acc = Acceleration(i);

                curr.NextPosition = 2.0 * curr.Position - curr.PreviousPosition + acc * step * step;
            }

            CompleteStep();
        }

        private Vector3 Acceleration(int i)
        {
            Particle curr = particles[i];
            Vector3 acc;
            Vector3 r;
            Vector3 re;
            double n;
            int j = 0;

            for (; j < i; j++)
            {
                r = particles[j].Position - curr.Position;
                n = r.Norm();
                re = r / n;

                acc += re * particles[j].Mass / (n * n);
            }

            j++;

            for (; j < particles.Count; j++)
            {
                r = particles[j].Position - curr.Position;
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
