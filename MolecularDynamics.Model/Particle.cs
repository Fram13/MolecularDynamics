using System;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет частицу, которая образуют моделируемое вещество. 
    /// </summary>
    [Serializable]
    public abstract class Particle
    {
        /// <summary>
        /// Положение частицы в пространстве, А.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Скорость частицы, А / 1e-14 с.
        /// </summary>
        public Vector3 Velocity;

        /// <summary>
        /// Сила, дествующая на частицу, а. е. м. * А / 1e-14 с ^ 2.
        /// </summary>
        public Vector3 Force;

        /// <summary>
        /// Масса частицы, а. е. м..
        /// </summary>
        public double Mass;

        /// <summary>
        /// Указывает, является ли частица неподвижной.
        /// </summary>
        public bool Static;

        /// <summary>
        /// Указывает, участвует ли частица во взаимодействии с другими частицами.
        /// </summary>
        public bool Free;

        /// <summary>
        /// Возвращает величину взаимодействия пары частиц.
        /// </summary>
        /// <param name="distance">Расстояние между частицами, А.</param>
        public abstract double PairForce(double distance);
    }
}
