using System.Threading.Tasks;

namespace MolecularDynamics.Model
{
    public partial class ParticleTrajectoryIntegrator
    {
        /// <summary>
        /// Представляет поток интегрирования уравнений движения диапазона частиц.
        /// </summary>
        private class TrajectoryIntegrationThread
        {
            #region Fields

            private bool needNextStep;
            private bool needCompleteStep;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Создает новый экземпляр <see cref="TrajectoryIntegrationThread"/>.
            /// </summary>
            /// <param name="integrator">Экземпляр <see cref="ParticleTrajectoryIntegrator"/>, в контексте которого создается поток.</param>
            /// <param name="startIndex">Левая граница диапазона частиц.</param>
            /// <param name="endIndex">Правая граница диапазона частиц.</param>
            public TrajectoryIntegrationThread(ParticleTrajectoryIntegrator integrator, int startIndex, int endIndex)
            {
                Task.Run(() =>
                {
                    while (true)
                    {
                        if (needNextStep)
                        {
                            integrator.NextStep(startIndex, endIndex);
                            needNextStep = false;
                        }

                        if (needCompleteStep)
                        {
                            integrator.CompleteStep(startIndex, endIndex);
                            needCompleteStep = false;
                        }
                    }
                });
            }

            #endregion Constructors

            #region Methods

            /// <summary>
            /// Выполняет следующий шаг интергирования уравнений движения диапазона частиц.
            /// </summary>
            public void NextStep()
            {
                needNextStep = true;
            }

            /// <summary>
            /// Завершает шаг интегрирования уравнений движения диапазона частиц.
            /// </summary>
            public void CompleteStep()
            {
                needCompleteStep = true;
            }

            /// <summary>
            /// Ожидает завершения выполнения вычислений данного потока.
            /// </summary>
            public void Wait()
            {
                while (needNextStep);
                while (needCompleteStep);
            }

            #endregion Methods
        }
    }
}
