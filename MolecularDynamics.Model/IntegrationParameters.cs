namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет параметры интегрирования траекторий частиц.
    /// </summary>
    public class IntegrationParameters
    {
        /// <summary>
        /// Постоянная Больцмана, ?.
        /// </summary>
        public const double BoltzmannConstant = 8.617332478e-5;

        /// <summary>
        /// Шаг интегрирования, 1е-14 с
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
    }
}
