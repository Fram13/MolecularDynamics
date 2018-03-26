using System;
using System.Collections.Generic;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет интегратор уравнений движения частиц.
    /// </summary>
    public partial class ParticleTrajectoryIntegrator
    {
        private ParticleGrid grid;
        private double step;

        /// <summary>
        /// Создает новый экземпляр <see cref="ParticleTrajectoryIntegrator"/>.
        /// </summary>
        /// <param name="grid">Список частиц, образующих модерируемое вещество.</param>
        /// <param name="step">Шаг интегрирования.</param>
        public ParticleTrajectoryIntegrator(ParticleGrid grid, double step)
        {
            this.grid = grid;
            this.step = step;
        }

        /// <summary>
        /// Вычисляет следующий шаг движения частиц.
        /// </summary>
        public void NextStep()
        {
            //вычисление сил зваимодействия между каждой парой частиц
            ForEachCell(cell =>
            {
                IList<Particle> particles = cell.Particles;
                ValueList<Vector3> forces = cell.Forces;
                IList<ParticleGridCell> boundaryCells = cell.BoundaryCells;

                for (int i = 0; i < cell.Forces.Count; i++)
                {
                    ref Vector3 f = ref cell.Forces.GetByRef(i);
                    f.X = 0.0;
                    f.Y = 0.0;
                    f.Z = 0.0;
                }

                for (int i = 0; i < particles.Count; i++)
                {
                    Particle particle = particles[i];
                    Func<Particle, Particle, Vector3> interactionFunction = particle.InteractionFunction;
                    ref Vector3 force = ref forces.GetByRef(i);

                    //вычисление сил взаимодействия с частицами в текущей ячейке
                    for (int j = 0; j < i; j++)
                    {
                        force.AddToCurrent(interactionFunction(particle, particles[j]));
                    }

                    for (int j = i + 1; j < particles.Count; j++)
                    {
                        force.AddToCurrent(interactionFunction(particle, particles[j]));
                    }

                    //вычисление сил взаимодействия с частицами в соседних ячейках
                    for (int j = 0; j < boundaryCells.Count; j++)
                    {
                        IList<Particle> boundaryCellParticles = boundaryCells[j].Particles;

                        for (int k = 0; k < boundaryCellParticles.Count; k++)
                        {
                            force.AddToCurrent(interactionFunction(particle, boundaryCellParticles[k]));
                        }
                    }
                }
            });

            //интегрирование Верле
            ForEachCell(cell =>
            {
                IList<Particle> particles = cell.Particles;
                ValueList<Vector3> forces = cell.Forces;

                for (int i = 0; i < particles.Count; i++)
                {
                    Particle particle = particles[i];
                    ref Vector3 velocity = ref particle.GetVelocityByRef();
                    ref Vector3 force = ref forces.GetByRef(i);

                    force.MultiplyToCurrent(step / particle.Mass);
                    velocity.AddToCurrent(force);

                    ref Vector3 position = ref particle.GetPositionByRef();
                    position.AddToCurrent(velocity * step);
                }
            });

            //перераспределение частиц по ячейкам
            ForEachCell(cell =>
            {
                IList<Particle> particles = cell.Particles;
                ValueList<Vector3> forces = cell.Forces;

                for (int i = 0; i < particles.Count; i++)
                {
                    Particle particle = particles[i];
                    ref Vector3 position = ref particle.GetPositionByRef();
                    var containingCell = grid.GetContainingCell(ref position);

                    if (containingCell != cell)
                    {
                        particles.RemoveAt(i);
                        containingCell.Particles.Add(particle);
                        forces.RemoveAt(i);
                        containingCell.Forces.Add((0, 0, 0));

                        //обработать перенос
                    }
                }
            });
        }

        /// <summary>
        /// Выполняет указанное действие для каждой ячейки сетки частиц.
        /// </summary>
        /// <param name="action">Действие для каждоЙ ячейки.</param>        
        private void ForEachCell(Action<ParticleGridCell> action)
        {
            for (int i = 0; i < grid.Rows; i++)
            {
                for (int j = 0; j < grid.Columns; j++)
                {
                    for (int k = 0; k < grid.Layers; k++)
                    {
                        action(grid[i, j, k]);
                    }
                }
            }
        }
    }
}
