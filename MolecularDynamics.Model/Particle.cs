using System;

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
        /// Текущвя скорость частицы.
        /// </summary>
        public Vector3 Velocity { get; set; }

        /// <summary>
        /// Масса частицы.
        /// </summary>
        public double Mass { get; set; }

        /// <summary>
        /// Функция, вычисляющая силу взаимодействия пары частиц.
        /// </summary>
        public Func<Particle, Particle, Vector3> InteractionFunction { get; set; }
    }
}
