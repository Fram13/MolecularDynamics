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
        private IntegrationParameters parameters;
        private double velocityMultiplier;
        private double randomForceLength;
        private NormalDistribution generator;

        /// <summary>
        /// Создает новый экземпляр <see cref="TrajectoryIntegrator"/>.
        /// </summary>
        /// <param name="grid">Список частиц, образующих модерируемое вещество.</param>
        /// <param name="parameters">Параметры интегрирования.</param>
        public TrajectoryIntegrator(ParticleGrid grid, IntegrationParameters parameters)
        {
            this.grid = grid;
            this.parameters = parameters;
            velocityMultiplier = 1.0 - parameters.DissipationCoefficient / parameters.IntegrationStep;
            randomForceLength = Math.Sqrt(2.0 * parameters.DissipationCoefficient * Constants.BoltzmannConstant *
                                          parameters.ParticleMass * parameters.Temperature / parameters.IntegrationStep);

            generator = new NormalDistribution();
        }

        /// <summary>
        /// Вычисляет следующий шаг движения частиц.
        /// </summary>
        public void NextStep()
        {
            //вычисление сил зваимодействия между каждой парой частиц
            grid.ForEachCell((cell, cellIndicies) =>
            {
                IList<Particle> particles = cell.Particles;
                IList<ParticleGrid.Cell> boundaryCells = cell.BoundaryCells;

                for (int i = 0; i < particles.Count; i++)
                {
                    Particle particle = particles[i];                    

                    particle.Force.X = 0.0;
                    particle.Force.Y = 0.0;
                    particle.Force.Z = 0.0;

                    //вычисление сил взаимодействия с частицами в текущей ячейке
                    for (int j = 0; j < i; j++)
                    {
                        particle.Force.AddToCurrent(PairForce(particle, particles[j]));
                    }

                    for (int j = i + 1; j < particles.Count; j++)
                    {
                        particle.Force.AddToCurrent(PairForce(particle, particles[j]));
                    }

                    //вычисление сил взаимодействия с частицами в соседних ячейках
                    for (int j = 0; j < boundaryCells.Count; j++)
                    {
                        IList<Particle> boundaryCellParticles = boundaryCells[j].Particles;

                        for (int k = 0; k < boundaryCellParticles.Count; k++)
                        {
                            particle.Force.AddToCurrent(PairForce(particle, boundaryCellParticles[k]));
                        }
                    }
                }
            });

            //интегрирование Верле
            grid.ForEachCell((cell, cellIndicies) =>
            {
                IList<Particle> particles = cell.Particles;

                for (int i = 0; i < particles.Count; i++)
                {
                    Particle particle = particles[i];

                    particle.Force/*.AddToCurrent(RandomForce())*/.MultiplyToCurrent(parameters.IntegrationStep / particle.Mass);
                    particle.Velocity.AddToCurrent(particle.Force).MultiplyToCurrent(velocityMultiplier);

                    particle.Position.AddToCurrent(particle.Velocity * parameters.IntegrationStep);
                }
            });

            grid.RedistributeParticles();
        }

        private Vector3 PairForce(Particle p1, Particle p2)
        {
            Vector3 r = p2.Position - p1.Position;
            double distance = r.Norm();
            r.DivideToCurrent(distance);
            r.MultiplyToCurrent(p1.PairForce(distance));

            return r;
        }

        private Vector3 RandomForce()
        {
            Vector3 r = new Vector3(generator.Next(), generator.Next(), generator.Next());
            r.NormalizeCurrent().MultiplyToCurrent(randomForceLength);
            return r;
        }
    }
}
