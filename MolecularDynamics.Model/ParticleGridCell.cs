using System.Collections.Generic;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет ячейку сетки частиц.
    /// </summary>
    public class ParticleGridCell
    {
        /// <summary>
        /// Список частиц, находящихся в текущей ячейке.
        /// </summary>
        public IList<Particle> Particles { get; } = new List<Particle>();

        /// <summary>
        /// Список векторов сил, действующих на каждую частицу в текущей ячейке.
        /// </summary>
        public ValueList<Vector3> Forces { get; } = new ValueList<Vector3>();

        /// <summary>
        /// Список граничных ячеек.
        /// </summary>
        public IList<ParticleGridCell> BoundaryCells { get; } = new List<ParticleGridCell>();
    }
}
