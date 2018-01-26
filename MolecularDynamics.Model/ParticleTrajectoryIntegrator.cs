using System.Collections.Generic;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет интегратор уравнений движения частиц.
    /// </summary>
    public partial class ParticleTrajectoryIntegrator
    {
        #region Fields

        private TrajectoryIntegrationThread[] threads;
        private IList<Particle> particles;
        private double step;
        private double halfStep;
        private double stepSquared;
        private double interactionCoefficient;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Создает новый экземпляр <see cref="ParticleTrajectoryIntegrator"/>.
        /// </summary>
        /// <param name="particles">Список частиц, образующих модерируемое вещество.</param>
        /// <param name="step">Шаг интегрирования.</param>
        /// <param name="interactionCoefficient">Коэффициент взаимодействия между частицами.</param>
        /// <param name="threadCount">Количество потоков интегрирования.</param>
        public ParticleTrajectoryIntegrator(IList<Particle> particles, double step, double interactionCoefficient, int threadCount)
        {
            this.particles = particles;
            this.step = step;
            halfStep = step / 2.0;
            stepSquared = step * step;
            this.interactionCoefficient = interactionCoefficient;

            threads = new TrajectoryIntegrationThread[threadCount];
            InitializeThreads(particles.Count, threadCount);
            InitialStep();
        }

        #endregion Constructors

        #region Private methods

        /// <summary>
        /// Инициализирует потоки интегрирования уравнений движения частиц.
        /// </summary>
        /// <param name="particleCount">Количество частиц.</param>
        /// <param name="threadCount">Количетсво потоков.</param>
        private void InitializeThreads(int particleCount, int threadCount)
        {
            int particlesPerThread = particleCount / threadCount;
            int rest = particleCount % threadCount;
            int start = 0;

            for (int i = 0; i < threadCount - 1; i++)
            {
                threads[i] = new TrajectoryIntegrationThread(this, start, start + particlesPerThread);
                start += particlesPerThread;
            }

            threads[threadCount - 1] = new TrajectoryIntegrationThread(this, start, start + particlesPerThread + rest);
        }

        /// <summary>
        /// Выполняет начальный шаг интегрирования уравнений движения частиц.
        /// </summary>
        private void InitialStep()
        {
            Vector3[] velocity = new Vector3[particles.Count];
            Vector3 acc;
            Particle curr;

            for (int i = 0; i < particles.Count; i++)
            {
                acc = Acceleration(i);
                curr = particles[i];

                velocity[i] = curr.InitialVelocity + acc * step;
                curr.NextPosition = curr.Position + velocity[i] * step;
            }

            RunCompleteStepInThreads();
            WaitThreads();
        }

        /// <summary>
        /// Выполняет следующий шаг интергирования уравнений движения диапазона частиц.
        /// </summary>
        /// <param name="startIndex">Левая граница диапазона частиц.</param>
        /// <param name="endIndex">Правая граница диапазона частиц.</param>
        private void NextStep(int startIndex, int endIndex)
        {
            Particle curr;
            Vector3 acc;

            for (int i = startIndex; i < endIndex; i++)
            {
                curr = particles[i];
                acc = Acceleration(i);
                curr.NextPosition = 2.0 * curr.Position - curr.PreviousPosition + acc * stepSquared;
            }
        }

        /// <summary>
        /// Вычисляет ускорение движения заданной частицы.
        /// </summary>
        /// <param name="i">Порядковый номер частицы.</param>
        /// <returns>Ускорение движения заданной частицы.</returns>
        private Vector3 Acceleration(int i)
        {
            Particle curr = particles[i];
            Particle other;

            Vector3 acc = (0.0, 0.0, 0.0);

            Vector3 r;
            double n;

            int j = 0;

            for (; j < i; j++)
            {
                other = particles[j];
                r = other.Position - curr.Position;
                n = r.Norm();
                r /= n;

                acc += r * (particles[j].Mass / (n * n));
            }

            j++;

            for (; j < particles.Count; j++)
            {
                other = particles[j];
                r = other.Position - curr.Position;
                n = r.Norm();
                r /= n;

                acc += r * (particles[j].Mass / (n * n));
            }

            return acc * interactionCoefficient;
        }

        /// <summary>
        /// Завершает шаг интегрирования уравнений движения диапазона частиц.
        /// </summary>
        /// <param name="startIndex">Левая граница диапазона частиц.</param>
        /// <param name="endIndex">Правая граница диапазона частиц.</param>
        private void CompleteStep(int startIndex, int endIndex)
        {
            Particle curr;

            for (int i = startIndex; i < endIndex; i++)
            {
                curr = particles[i];
                curr.PreviousPosition = curr.Position;
                curr.Position = curr.NextPosition;
            }
        }

        #region Threads control methods

        private void RunNextStepInThreads()
        {
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].NextStep();
            }
        }

        private void RunCompleteStepInThreads()
        {
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].CompleteStep();
            }
        }

        private void WaitThreads()
        {
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i].Wait();
            }
        }

        #endregion Threads control methods

        #endregion Private methods

        #region Public methods

        /// <summary>
        /// Вычисляет следующий шаг движения частиц.
        /// </summary>
        public void NextStep()
        {
            RunNextStepInThreads();
            WaitThreads();
            RunCompleteStepInThreads();
            WaitThreads();
        }

        #endregion Public methods
    }
}
