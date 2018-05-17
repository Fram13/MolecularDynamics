using System;
using System.Threading.Tasks;
using MolecularDynamics.Model;
using System.Collections.Generic;

namespace MolecularDynamics.Visualization
{
    class Program
    {
        static void Main(string[] args)
        {
            var tuple = ParticleGenerator.GenerateWolframGrid((20, 20, 20), 3);

            var particles = tuple.Item2;

            TrajectoryIntegrator integrator = new TrajectoryIntegrator(tuple.Item1, Constants.Step);

            float[] positions = new float[particles.Count * 3];
            float[] nextPositions = new float[particles.Count * 3];

            for (int i = 0; i < particles.Count; i++)
            {
                positions[3 * i] = (float)(particles[i].Position.X / 10);
                positions[3 * i + 1] = (float)(particles[i].Position.Y / 10);
                positions[3 * i + 2] = (float)(particles[i].Position.Z / 10);
            }

            Object syncObject = new Object();
            
            using (ParticleVisualizer particleVisualizer = new ParticleVisualizer(positions, sphereRadius: 0.141, faces: 10, syncObject: syncObject))
            {
                Task.Run(() =>
                {
                    while (true)
                    {
                        integrator.NextStep();

                        for (int i = 0; i < particles.Count; i++)
                        {
                            nextPositions[3 * i] = (float)(particles[i].Position.X / 10);
                            nextPositions[3 * i + 1] = (float)(particles[i].Position.Y / 10);
                            nextPositions[3 * i + 2] = (float)(particles[i].Position.Z / 10);
                        }

                        lock (syncObject)
                        {
                            var temp = positions;
                            positions = nextPositions;
                            nextPositions = temp;
                        }
                    }
                });

                particleVisualizer.Run(30);
            }
        }
    }
}