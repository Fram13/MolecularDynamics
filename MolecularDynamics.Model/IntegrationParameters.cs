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
    }
}
