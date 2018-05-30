namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет параметры интегрирования траекторий частиц.
    /// </summary>
    public class SimulationParameters
    {
        /// <summary>
        /// Размеры ячейки сетки.
        /// </summary>
        public readonly Vector3 CellSize;

        /// <summary>
        /// Количество ячеек в сетке.
        /// </summary>
        public readonly (int X, int Y, int Z) CellCount;
        
        /// <summary>
        /// Размеры пространства моделирования.
        /// </summary>
        public readonly Vector3 SpaceSize;
        
        /// <summary>
        /// Шаг интегрирования, 1е-14 с.
        /// </summary>
        public double IntegrationStep;

        /// <summary>
        /// Температура вещества, К.
        /// </summary>
        public double Temperature;

        /// <summary>
        /// Коэффициент диссипации, ?.
        /// </summary>
        public double DissipationCoefficient;

        /// <summary>
        /// Масса частицы, а. е. м..
        /// </summary>
        public double ParticleMass;
        
        /// <summary>
        /// Количество неподвижных слоев ячеек.
        /// </summary>
        public int StaticCellLayerCount;

        /// <summary>
        /// Радиус взаимодействия между частицами, нм.
        /// </summary>
        public double InteractionRadius;

        /// <summary>
        /// Модуль скорости осаждающейся частицы, А / с.
        /// </summary>
        public double NewParticleVelocity;

        /// <summary>
        /// Количество потоков вычислений.
        /// </summary>
        public int Threads;

        /// <summary>
        /// Создает новый экземпляр <see cref="SimulationParameters"/>.
        /// </summary>
        /// <param name="spaceSize">Размеры пространства моделирования.</param>
        /// <param name="cellCount">Количество ячеек в сетке.</param>
        public SimulationParameters(Vector3 spaceSize, (int X, int Y, int Z) cellCount)
        {
            SpaceSize = spaceSize;
            CellCount = cellCount;

            CellSize.X = SpaceSize.X / CellCount.X;
            CellSize.Y = SpaceSize.Y / CellCount.Y;
            CellSize.Z = SpaceSize.Z / CellCount.Z;
        }
    }
}
