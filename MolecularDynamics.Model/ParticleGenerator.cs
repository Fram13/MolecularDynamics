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
        /// Заполняет сетку атомами вольфрама, образующими ОЦК решетку.
        /// </summary>
        /// <param name="grid">Заполняемая сетка.</param>
        /// <param name="startPosition">Начальная позиция атомов вольфрама.</param>
        /// <param name="cellCount">Количество ячеек ОЦК решетки вольфрама.</param>
        /// <param name="parameters">Параметры моделирования частиц.</param>
        /// <returns></returns>
        public static List<Particle> GenerateWolframGrid(this ParticleGrid grid, Vector3 startPosition, (int X, int Y, int Z) cellCount, SimulationParameters parameters)
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
                            Position = position
                        };

                        particles.Add(particle);
                        grid.AddParticle(particle);
                    }
                }
            }

            return particles;
        }
    }
}
