using System;

namespace MolecularDynamics.Model.Atoms
{
    /// <summary>
    /// Представляет атом вольфрама.
    /// </summary>
    public class Wolfram : Particle
    {
        /// <summary>
        /// Радиус атома, нм.
        /// </summary>
        public const double Radius = 0.141;

        /// <summary>
        /// Масса атома, а. е. м..
        /// </summary>
        public const double AtomMass = 183.84;

        /// <summary>
        /// Создает новый экземпляр <see cref="Wolfram"/>.
        /// </summary>
        public Wolfram()
        {
            Mass = AtomMass;
        }

        /// <summary>
        /// Возвращает величину взаимодействия пары частиц.
        /// </summary>
        /// <param name="distance">Расстояние между частицами, А.</param>
        public override double PairForce(double distance)
        {
            double force = 0;

            if (distance <= 1.72)
            {
                throw new ArgumentOutOfRangeException("Выход за пределы интервала");
            }
            else if (distance < 2.53139225)
            {
                force = -4.071 * 1.435 * Math.Exp(-4.071 * (distance - 2.531392));
            }
            else if (distance < 3.81445743)
            {
                force = -1.26599 * 4 * Math.Pow(distance - 3.45503, 3) - 3.0083 * 3 * Math.Pow(distance - 3.45503, 2) + 1.40109;
            }

            return force;
        }
    }
}
