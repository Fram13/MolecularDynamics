using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет интегратор уравнений движения частиц.
    /// </summary>
    public partial class ParticleTrajectoryIntegrator
    {
        #region Fields

        private IList<Particle> particles;
        private Vector3[] currentAccelerations;
        private Vector3[] previousAccelerations;
        private Task[] computingTasks;
        private ValueTuple<int, int>[] indexBlocks;
        private double step;
        private double halfStep;
        private double stepSquaredHalf;
        private int threadCount;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Создает новый экземпляр <see cref="ParticleTrajectoryIntegrator"/>.
        /// </summary>
        /// <param name="particles">Список частиц, образующих модерируемое вещество.</param>
        /// <param name="step">Шаг интегрирования.</param>
        /// <param name="threadCount">Количество потоков интегрирования.</param>
        public ParticleTrajectoryIntegrator(IList<Particle> particles, double step, int threadCount)
        {
            this.particles = particles;
            this.step = step;
            this.threadCount = threadCount;

            halfStep = step * 0.5;
            stepSquaredHalf = step * step * 0.5;
            currentAccelerations = new Vector3[particles.Count];
            previousAccelerations = new Vector3[particles.Count];
            computingTasks = new Task[threadCount];
            indexBlocks = new ValueTuple<int, int>[threadCount];
            InitializeIndexBlocks();
        }

        #endregion Constructors

        #region Methods

        #region Private methods

        /// <summary>
        /// Инициализирует блоки индексов.
        /// </summary>
        private void InitializeIndexBlocks()
        {
            int particlesPerThread = particles.Count / threadCount;
            int rest = particles.Count % threadCount;
            int start = 0;

            for (int i = 0; i < threadCount - 1; i++)
            {
                indexBlocks[i] = new ValueTuple<int, int>(start, start + particlesPerThread);
                start += particlesPerThread;
            }

            indexBlocks[threadCount - 1] = new ValueTuple<int, int>(start, start + particlesPerThread + rest);
        }

        /// <summary>
        /// Параллельно выполняет указанное действие для каждого блока индексов.
        /// </summary>
        /// <param name="action">Действие для каждого блока индексов.</param>        
        private void ForEachIndexBlock(Action<int, int> action)
        {
            for (int i = 0; i < threadCount; i++)
            {
                int j = i;
                computingTasks[i] = Task.Run(() => action.Invoke(indexBlocks[j].Item1, indexBlocks[j].Item2));
            }

            Task.WaitAll(computingTasks);
        }

        /// <summary>
        /// Вычисляет ускорения взаимодействия для каждой пары частиц.
        /// </summary>
        private void ComputeAccelerations()
        {
            int count = particles.Count / threadCount;
            int rest = particles.Count % threadCount;

            for (int i = 0; i < count; i++)
            {
                for (int offset = 0; offset < threadCount; offset++)
                {
                    for (int k = 0; k < threadCount; k++)
                    {
                        int particleIndex = i * threadCount + k;
                        int currenfOffset = offset;
                        computingTasks[k] = Task.Run(() => ComputeAcceleration(particleIndex, currenfOffset));
                    }

                    Task.WaitAll(computingTasks);
                }
            }

            if (rest != 0)
            {
                for (int offset = 0; offset < rest; offset++)
                {
                    for (int k = 0; k < rest; k++)
                    {
                        int particleIndex = count * threadCount + k;
                        int currenfOffset = offset;
                        computingTasks[k] = Task.Run(() => ComputeAcceleration(particleIndex, currenfOffset));
                    }

                    Task.WaitAll(computingTasks);
                }
            }
        }

        /// <summary>
        /// Вычисляет ускорение, являющееся результатом влияния остальных частиц на текущую.
        /// </summary>
        /// <param name="particleIndex">Индекс текущей частицы.</param>
        /// <param name="offset">Смещение индексов остальных частиц.</param>
        private void ComputeAcceleration(int particleIndex, int offset)
        {
            Particle current = particles[particleIndex];
            int i = offset;

            while (i < particleIndex)
            {
                currentAccelerations[particleIndex].AddToCurrent(current.InteractionFunction(current, particles[i]));
                i += threadCount;
            }

            if (i == particleIndex)
            {
                i += threadCount;
            }

            while (i < particles.Count)
            {
                currentAccelerations[particleIndex].AddToCurrent(current.InteractionFunction(current, particles[i]));
                i += threadCount;
            }
        }

        #endregion Private methods

        #region Public methods

        /// <summary>
        /// Вычисляет следующий шаг движения частиц.
        /// </summary>
        public void NextStep()
        {
            ForEachIndexBlock((start, end) =>
            {
                for (int i = start; i < end; i++)
                {
                    currentAccelerations[i].X = 0.0;
                    currentAccelerations[i].Y = 0.0;
                    currentAccelerations[i].Z = 0.0;
                }
            });

            ComputeAccelerations();

            ForEachIndexBlock((start, end) =>
            {
                for (int i = start; i < end; i++)
                {
                    previousAccelerations[i] = currentAccelerations[i];

                    currentAccelerations[i].MultiplyToCurrent(stepSquaredHalf);
                    Vector3 v = particles[i].Velocity * step;
                    particles[i].Position = particles[i].Position.AddToCurrent(v).AddToCurrent(currentAccelerations[i]);
                }
            });

            ComputeAccelerations();

            ForEachIndexBlock((start, end) =>
            {
                for (int i = start; i < end; i++)
                {
                    previousAccelerations[i].MultiplyToCurrent(halfStep);
                    currentAccelerations[i].MultiplyToCurrent(halfStep);
                    particles[i].Velocity = particles[i].Velocity.AddToCurrent(previousAccelerations[i]).AddToCurrent(currentAccelerations[i]);
                }
            });
        }

        #endregion Public methods

        #endregion Methods
    }
}
