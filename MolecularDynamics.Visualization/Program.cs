using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MolecularDynamics.Model;
using MolecularDynamics.Model.Atoms;

namespace MolecularDynamics.Visualization
{
    internal class Program
    {
        private static readonly SimulationParameters Parameters = new SimulationParameters(cellSize: (3.15, 3.15, 3.15), cellCount: (5, 10, 5))
        {
            IntegrationStep = 0.1,
            DissipationCoefficient = 0.06 / 3.615,
            Temperature = 100,
            ParticleMass = Wolfram.AtomMass,
            CellLayerCount = 5,
            StaticCellLayerCount = 2,
            InteractionRadius = 4,
            Threads = 1
        };

        private static void Main(string[] args)
        { 
            var (grid, particles) = ParticleGenerator.GenerateWolframGrid(Parameters);

            //ParticleGrid grid = new ParticleGrid((20, 20, 20), (1, 1, 1), 0, 1);
            //List<Particle> particles = new List<Particle>();

            //Particle p1 = new Wolfram()
            //{
            //    Position = (9, 10, 10)
            //};

            //Particle p2 = new Wolfram()
            //{
            //    Position = (11.8, 10, 10)
            //};

            //grid.AddParticle(p1);
            //grid.AddParticle(p2);
            //particles.Add(p1);
            //particles.Add(p2);

            TrajectoryIntegrator integrator = new TrajectoryIntegrator(grid, Parameters);

            using (ParticleVisualizer particleVisualizer = new ParticleVisualizer(particles, integrator, Wolfram.Radius))
            {
                particleVisualizer.Run(30);
            }
        }
    }
}
