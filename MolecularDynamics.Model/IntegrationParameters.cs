namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет параметры интегрирования траекторий частиц.
    /// </summary>
    public class IntegrationParameters
    {
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
        /// Размеры пространства моделирования.
        /// </summary>
        public Vector3 SpaceSize;

        /// <summary>
        /// Размеры ячейки сетки.
        /// </summary>
        public Vector3 CellSize;

        /// <summary>
        /// Количество ячеек в сетке.
        /// </summary>
        public (int X, int Y, int Z) CellCount;

        /// <summary>
        /// Количество неподвижных слоев ячеек.
        /// </summary>
        public int StaticCellLayerCount;
    }
}
