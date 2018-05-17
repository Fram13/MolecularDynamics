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

        public Vector3 RandomForce;

        /// <summary>
        /// Масса частицы.
        /// </summary>
        public double Mass;

        /// <summary>
        /// Возвращает силу взаимодействия пары частиц.
        /// </summary>
        public Vector3 PairForce(Particle other)
        {
            Vector3 r = this.Position - other.Position;
            double n = r.Norm();
            r.DivideToCurrent(n);
            double F = 0;

            if (n <= 1.72)
            {
                throw new IndexOutOfRangeException("Выход за пределы интервала");
            }
            else if (n < 2.53139225)
            {
                F = -4.071 * 1.435 * Math.Exp(-4.071 * (n - 2.531392));
            }
            else if (n < 3.81445743)
            {
                F = -1.26599 * 4 * Math.Pow(n - 3.45503, 3) - 3.0083 * 3 * Math.Pow(n - 3.45503, 2) + 1.40109;
            }

            return r.MultiplyToCurrent(F);
        }

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

        public ref Vector3 GetForceRandomByRef()
        {
            return ref RandomForce;
        }
    }
}
