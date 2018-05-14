using System;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет частицу, которая образуют моделируемое вещество. 
    /// </summary>
    public class Particle
    {
        /// <summary>
        /// Положение частицы в пространстве.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Скорость частицы.
        /// </summary>
        public Vector3 Velocity;

        /// <summary>
        /// Сила, дествующая на частицу.
        /// </summary>
        public Vector3 Force;

        /// <summary>
        /// Масса частицы.
        /// </summary>
        public double Mass;

        /// <summary>
        /// Функция, вычисляющая силу взаимодействия пары частиц.
        /// </summary>
        public Func<Particle, Particle, Vector3> InteractionFunction { get; set; }

        /// <summary>
        /// Возвращает положение частицы по ссылке.
        /// </summary>
        public ref Vector3 GetPositionByRef()
        {
            return ref Position;
        }

        /// <summary>
        /// Возвращает скорость частицы по ссылке.
        /// </summary>
        public ref Vector3 GetVelocityByRef()
        {
            return ref Velocity;
        }

        /// <summary>
        /// Возвращает силу, дествующую на частицу, по ссылке.
        /// </summary>
        public ref Vector3 GetForceByRef()
        {
            return ref Force;
        }
    }
}
