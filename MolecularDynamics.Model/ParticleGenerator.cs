using System.Collections.Generic;
using MolecularDynamics.Model.Atoms;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет генератор частиц.
    /// </summary>
    public static class ParticleGenerator
    {
        /// <summary>
        /// Создает сетку атомов вольфрама, образующими ОЦК решетку.
        /// </summary>
        /// <param name="startPosition">Начальная позиция атомов вольфрама.</param>
        /// <param name="cellCount">Количество ячеек ОЦК решетки вольфрама.</param>
        /// <param name="staticLayerCellCount">Количество неподвижных слоев частиц.</param>
        /// <returns></returns>
        public static List<Particle> GenerateWolframGrid(Vector3 startPosition, (int X, int Y, int Z) cellCount, int staticLayerCellCount)
        {
            List<Particle> particles = new List<Particle>();

            for (int z = 0; z < cellCount.Z; z++)
            {
                for (int x = 0; x < cellCount.X; x++)
                {
                    for (int y = 0; y < cellCount.Y; y++)
                    {
                        Vector3 position;
                        position.X = startPosition.X + Wolfram.GridConstant * x + (z % 2) * Wolfram.GridConstant / 2.0;
                        position.Y = startPosition.Y + Wolfram.GridConstant * y + (z % 2) * Wolfram.GridConstant / 2.0;
                        position.Z = startPosition.Z + z * Wolfram.GridConstant / 2.0;

                        Particle particle = new Wolfram()
                        {
                            Position = position,
                            Static = z < staticLayerCellCount
                        };

                        particles.Add(particle);
                    }
                }
            }

            return particles;
        }
    }
}
