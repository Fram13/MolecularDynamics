using System;
using System.Threading;

namespace MolecularDynamics.Model
{
    public partial class ParticleTrajectoryIntegrator
    {
        private class TrajectoryIntegrationThread
        {
            private Thread thread;
            private int startIndex;
            private int endIndex;
            private bool needCalculate;

            public TrajectoryIntegrationThread(ParticleTrajectoryIntegrator integrator, int startIndex, int endIndex)
            {
                this.thread = new Thread(new ParameterizedThreadStart(args =>
                {
                    while (true)
                    {
                        if (needCalculate)
                        {
                            integrator.NextStep(args);
                            needCalculate = false;
                        }
                    }
                }));

                this.thread.Name = "Start index: " + startIndex;
                this.startIndex = startIndex;
                this.endIndex = endIndex;

                thread.Start(Tuple.Create(startIndex, endIndex));
            }

            public void NextStep()
            {
                needCalculate = true;
            }

            public void Wait()
            {
                while (needCalculate);
            }
        }
    }
}
