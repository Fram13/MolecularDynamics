using System;
using System.Collections.Generic;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет интегратор уравнений движения частиц.
    /// </summary>
    public partial class TrajectoryIntegrator
    {
        private ParticleGrid grid;
        private double step;

        /// <summary>
        /// Создает новый экземпляр <see cref="TrajectoryIntegrator"/>.
        /// </summary>
        /// <param name="grid">Список частиц, образующих модерируемое вещество.</param>
        /// <param name="step">Шаг интегрирования.</param>
        public TrajectoryIntegrator(ParticleGrid grid, double step)
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
            grid.ForEachCell((cell, indicies) =>
            {
                IList<Particle> particles = cell.Particles;
                IList<ParticleGrid.Cell> boundaryCells = cell.BoundaryCells;

                for (int i = 0; i < particles.Count; i++)
                {
                    Particle particle = particles[i];                    
                    ref Vector3 force = ref particle.GetForceByRef();

                    force.X = 0.0;
                    force.Y = 0.0;
                    force.Z = 0.0;

                    //вычисление сил взаимодействия с частицами в текущей ячейке
                    for (int j = 0; j < i; j++)
                    {
                        force.AddToCurrent(particle.PairForce(particles[j]));
                    }

                    for (int j = i + 1; j < particles.Count; j++)
                    {
                        force.AddToCurrent(particle.PairForce(particles[j]));
                    }

                    //вычисление сил взаимодействия с частицами в соседних ячейках
                    for (int j = 0; j < boundaryCells.Count; j++)
                    {
                        IList<Particle> boundaryCellParticles = boundaryCells[j].Particles;

                        for (int k = 0; k < boundaryCellParticles.Count; k++)
                        {
                            force.AddToCurrent(particle.PairForce(boundaryCellParticles[k]));
                        }
                    }
                }
            });

            //интегрирование Верле
            grid.ForEachCell((cell, indicies) =>
            {
                IList<Particle> particles = cell.Particles;

                for (int i = 0; i < particles.Count; i++)
                {
                    Particle particle = particles[i];
                    ref Vector3 velocity = ref particle.GetVelocityByRef();
                    ref Vector3 force = ref particle.GetForceByRef();
                    ref Vector3 randomForce = ref particle.GetForceRandomByRef();

                    randomForce.AddToCurrent(Constants.RandomForceLength * Constants.RandomNormalVector);

                    force.AddToCurrent(randomForce).MultiplyToCurrent(step / particle.Mass);
                    velocity.AddToCurrent(force).MultiplyToCurrent(1.0 - Constants.DissipationCoefficient * step);

                    ref Vector3 position = ref particle.GetPositionByRef();
                    position.AddToCurrent(velocity * step);
                }
            });

            grid.RedistributeParticles();
        }
    }
}
