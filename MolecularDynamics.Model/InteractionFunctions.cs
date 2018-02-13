﻿namespace MolecularDynamics.Model
{
    /// <summary>
    /// Содержит функции, определяющие взаимодействие частиц.
    /// </summary>
    public static class InteractionFunctions
    {
        /// <summary>
        /// Функция, описывающая гравитационное взаимодействие частиц.
        /// </summary>
        /// <param name="current">Текущая частица.</param>
        /// <param name="other">Частица, которая оказывает влияние на текущую частицу.</param>
        /// <returns>Сила гравитационного взаимодействия, создаваемого другой частицей.</returns>
        public static Vector3 GravitationalInteraction(Particle current, Particle other)
        {
            Vector3 r = other.Position - current.Position;
            double n = r.Norm();
            r.DivideToCurrent(n);
            return r.MultiplyToCurrent(other.Mass / (n * n));
        }
    }
}
