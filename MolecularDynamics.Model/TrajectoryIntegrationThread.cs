using System;
using System.Threading;
using System.Threading.Tasks;

namespace MolecularDynamics.Model
{
    public partial class ParticleTrajectoryIntegrator
    {
        private class TrajectoryIntegrationThread
        {
            private Thread thread;
            private bool needCalculate;

            public TrajectoryIntegrationThread(ParticleTrajectoryIntegrator integrator, int startIndex, int endIndex)
            {
                //thread = new Thread(new ParameterizedThreadStart(args =>
                //{
                //    while (true)
                //    {
                //        if (needCalculate)
                //        {
                //            integrator.NextStep(args);
                //            needCalculate = false;
                //        }
                //    }
                //}));

                //thread.Name = "Start index: " + startIndex;
                //thread.IsBackground = true;
                //thread.Start(Tuple.Create(startIndex, endIndex));

                Task.Run(() =>
                {
                    while (true)
                    {
                        if (needCalculate)
                        {
                            integrator.NextStep(startIndex, endIndex);
                            needCalculate = false;
                        }
                    }
                });
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
