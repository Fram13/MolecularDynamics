using System;
using System.Collections.Generic;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет интегратор уравнений движения частиц.
    /// </summary>
    public class TrajectoryIntegrator
    {
        private ParticleGrid grid;
        private double velocityMultiplier;
        private double randomForce;
        private NormalDistribution generator;

        private double integrationStep;
        private Vector3 spaceSize;
        private double dissipationCoefficient;
        private double particleMass;
        private double temperature;
        private int staticCellLayerCount;

        public ParticleGrid Grid => grid;

        /// <summary>
        /// Создает новый экземпляр <see cref="TrajectoryIntegrator"/>.
        /// </summary>
        /// <param name="grid">Список частиц, образующих модерируемое вещество.</param>
        /// <param name="parameters">Параметры интегрирования.</param>
        public TrajectoryIntegrator(ParticleGrid grid, SimulationParameters parameters)
        {
            this.grid = grid;
            integrationStep = parameters.IntegrationStep;
            spaceSize = parameters.SpaceSize;
            dissipationCoefficient = parameters.DissipationCoefficient;
            particleMass = parameters.ParticleMass;
            temperature = parameters.Temperature;
            staticCellLayerCount = parameters.StaticCellLayerCount;

            velocityMultiplier = 1.0 - dissipationCoefficient * integrationStep;
            randomForce = Math.Sqrt(2.0 * dissipationCoefficient * Constants.BoltzmannConstant * particleMass * temperature / integrationStep);

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

                    if (particle.Static)
                    {
                        continue;
                    }

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

                    if (particle.Static)
                    {
                        continue;
                    }

                    particle.Force.AddToCurrent(RandomForce());
                    particle.Force.MultiplyToCurrent(integrationStep / particle.Mass);

                    particle.Velocity.AddToCurrent(particle.Force);
                    particle.Velocity.MultiplyToCurrent(velocityMultiplier);

                    particle.Position.AddToCurrent(particle.Velocity * integrationStep);
                }
            });

            grid.RedistributeParticles();
        }

        private Vector3 PairForce(Particle p1, Particle p2)
        {
            Vector3 r = p2.Position - p1.Position;

            r.X = Math.Abs(r.X) > spaceSize.X / 2.0 ? r.X - Math.Sign(r.X) * spaceSize.X : r.X;
            r.Y = Math.Abs(r.Y) > spaceSize.Y / 2.0 ? r.Y - Math.Sign(r.Y) * spaceSize.Y : r.Y;
            r.Z = Math.Abs(r.Z) > spaceSize.Z / 2.0 ? r.Z - Math.Sign(r.Z) * spaceSize.Z : r.Z;

            double distance = r.Norm();
            r.DivideToCurrent(distance);
            r.MultiplyToCurrent(p1.PairForce(distance));

            return r;
        }

        private Vector3 RandomForce()
        {
            Vector3 r = new Vector3(generator.Next(), generator.Next(), generator.Next());
            r.MultiplyToCurrent(randomForce);
            return r;
        }
    }
}
