using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MolecularDynamics.Model;

namespace MolecularDynamics.Tests
{
    [TestClass]
    public class NormalDistributionTests
    {
        [TestMethod]
        public void NormalDistributionParametersTest()
        {
            //arrange
            NormalDistribution generator = new NormalDistribution();

            //act
            double[] samples = new double[1000000];

            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = generator.Next();
            }

            double mean = 0.0;

            for (int i = 0; i < samples.Length; i++)
            {
                mean += samples[i];
            }

            mean /= samples.Length;

            double stdev = 0.0;

            for (int i = 0; i < samples.Length; i++)
            {
                stdev += (mean - samples[i]) * (mean - samples[i]);
            }

            stdev /= samples.Length - 1;

            //assert
            Assert.IsTrue(Math.Abs(mean) < 1e-2);
            Assert.IsTrue(Math.Abs(stdev - 1.0) < 1e-2);
        }
    }
}
