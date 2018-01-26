namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет частицу, которая образуют моделируемое вещество. 
    /// </summary>
    public class Particle
    {
        /// <summary>
        /// Текущее положение частицы в пространстве.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Предыдущее положение частицы в пространстве.
        /// </summary>
        internal Vector3 PreviousPosition { get; set; }

        /// <summary>
        /// Следующее положение частицы в пространстве.
        /// </summary>
        internal Vector3 NextPosition { get; set; }

        /// <summary>
        /// Начальная скорость частицы.
        /// </summary>
        public Vector3 InitialVelocity { get; set; }

        /// <summary>
        /// Радиус частицы.
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// Масса частицы.
        /// </summary>
        public double Mass { get; set; }
    }
}
