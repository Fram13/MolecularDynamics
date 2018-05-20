using System;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет потокобезопасный генератор нормально распределенныъ величин.
    /// </summary>
    public class NormalDistribution
    {
        private Random _random = new Random();
        private Object _syncObject = new Object();

        /// <summary>
        /// Генерирует нормально распределенное число с параметрами M[X] = 0 и D[X] = 1.
        /// </summary>
        public double Next()
        {
            double s = 1.0;
            double v1, v2;

            lock (_syncObject)
            {
                do
                {
                    v1 = 2.0 * _random.NextDouble() - 1.0;
                    v2 = 2.0 * _random.NextDouble() - 1.0;
                    s = v1 * v1 + v2 * v2;
                } while (!(s < 1.0)); 
            }

            return v1 * Math.Sqrt(-2.0 * Math.Log(s) / s);
        }
    }
}
