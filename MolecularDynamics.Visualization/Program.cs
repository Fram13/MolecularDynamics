using System.Collections.Generic;
using MolecularDynamics.Model;
using MolecularDynamics.Model.Atoms;

namespace MolecularDynamics.Visualization
{
    internal class Program
    {
        private static readonly SimulationParameters Parameters = new SimulationParameters(
            spaceSize: (6.5 * Wolfram.GridConstant, 6.5 * Wolfram.GridConstant, 14 * Wolfram.GridConstant),
            cellCount: (14, 14, 28))
        {
            IntegrationStep = 0.1,
            DissipationCoefficient = 0.06 / 3.615,
            Temperature = 200,
            ParticleMass = Wolfram.AtomMass,
            StaticCellLayerCount = 6,
            InteractionRadius = 4,
            Threads = 1
        };

        private static void Main(string[] args)
        {
            ParticleGrid grid = new ParticleGrid(Parameters.SpaceSize,
                                                 Parameters.CellCount,
                                                 Parameters.CellSize,
                                                 Parameters.InteractionRadius,
                                                 Parameters.Threads);

            List<Particle> particles = grid.GenerateWolframGrid((1.5, 1.5, 1.5), (6, 6, 12), Parameters);

            System.Console.WriteLine("Particles: " + particles.Count);

            TrajectoryIntegrator integrator = new TrajectoryIntegrator(grid, Parameters);

            using (ParticleVisualizer particleVisualizer = new ParticleVisualizer(particles, integrator, Parameters.SpaceSize / 10.0, Wolfram.Radius))
            {
                particleVisualizer.Run(30);
            }
        }
    }
}
