using System;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет частицу, которая образуют моделируемое вещество. 
    /// </summary>
    public class Particle
    {
        private Vector3 position;
        private Vector3 velocity;
        private Vector3 force;

        /// <summary>
        /// Текущее положение частицы в пространстве.
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        /// <summary>
        /// Текущвя скорость частицы.
        /// </summary>
        public Vector3 Velocity
        {
            get
            {
                return velocity;
            }
            set
            {
                velocity = value;
            }
        }

        /// <summary>
        /// Текущая сила, дествующая на частицу.
        /// </summary>
        public Vector3 Force
        {
            get
            {
                return force;
            }
            set
            {
                force = value;
            }
        }

        /// <summary>
        /// Масса частицы.
        /// </summary>
        public double Mass { get; set; }

        /// <summary>
        /// Функция, вычисляющая силу взаимодействия пары частиц.
        /// </summary>
        public Func<Particle, Particle, Vector3> InteractionFunction { get; set; }

        /// <summary>
        /// Возвращает положение частицы по ссылке.
        /// </summary>
        public ref Vector3 GetPositionByRef()
        {
            return ref position;
        }

        /// <summary>
        /// Возвращает скорость частицы по ссылке.
        /// </summary>
        public ref Vector3 GetVelocityByRef()
        {
            return ref velocity;
        }

        /// <summary>
        /// Возвращает силу, дествующую на частицу, по ссылке.
        /// </summary>
        public ref Vector3 GetForceByRef()
        {
            return ref force;
        }
    }
}
