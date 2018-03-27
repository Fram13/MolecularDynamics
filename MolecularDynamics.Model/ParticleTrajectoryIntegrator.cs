﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет интегратор уравнений движения частиц.
    /// </summary>
    public partial class ParticleTrajectoryIntegrator
    {
        private ParticleGrid grid;
        private double step;
        private bool calculating;

        /// <summary>
        /// Создает новый экземпляр <see cref="ParticleTrajectoryIntegrator"/>.
        /// </summary>
        /// <param name="grid">Список частиц, образующих модерируемое вещество.</param>
        /// <param name="step">Шаг интегрирования.</param>
        public ParticleTrajectoryIntegrator(ParticleGrid grid, double step)
        {
            this.grid = grid;
            this.step = step;

            Task.Run(() =>
            {
                while (true)
                {
                    if (calculating)
                    {
                        NextStep();
                        calculating = false;
                    }
                }
            });
        }

        public void Wait()
        {
            while (calculating);
        }

        public void NextStepAsync()
        {
            calculating = true;
        }

        /// <summary>
        /// Вычисляет следующий шаг движения частиц.
        /// </summary>
        private void NextStep()
        {
            //вычисление сил зваимодействия между каждой парой частиц
            ForEachCell(cell =>
            {
                IList<Particle> particles = cell.Particles;
                IList<ParticleGridCell> boundaryCells = cell.BoundaryCells;

                for (int i = 0; i < particles.Count; i++)
                {
                    Particle particle = particles[i];                    
                    Func<Particle, Particle, Vector3> interactionFunction = particle.InteractionFunction;
                    ref Vector3 force = ref particle.GetForceByRef();

                    force.X = 0.0;
                    force.Y = 0.0;
                    force.Z = 0.0;

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

                for (int i = 0; i < particles.Count; i++)
                {
                    Particle particle = particles[i];
                    ref Vector3 velocity = ref particle.GetVelocityByRef();
                    ref Vector3 force = ref particle.GetForceByRef();

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

                for (int i = 0; i < particles.Count; i++)
                {
                    Particle particle = particles[i];
                    ref Vector3 position = ref particle.GetPositionByRef();
                    var containingCell = grid.GetContainingCell(ref position);

                    if (containingCell != cell)
                    {
                        particles.RemoveAt(i);
                        containingCell.Particles.Add(particle);

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
