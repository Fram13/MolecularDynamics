﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MolecularDynamics.Model;

namespace MolecularDynamics.Tests
{
    [TestClass]
    public class Vector3Tests
    {
        [TestMethod]
        public void ComponentProperties()
        {
            //arrange
            //act
            Vector3 v = new Vector3(1.0, 2.0, 3.0);

            //assert
            Assert.AreEqual(1.0, v.X);
            Assert.AreEqual(2.0, v.Y);
            Assert.AreEqual(3.0, v.Z);
        }

        [TestMethod]
        public void CustomConstructor()
        {
            //arrange
            //act
            Vector3 v = new Vector3(1.0, 2.0, 3.0);

            //assert
            Assert.AreEqual(1.0, v.X);
            Assert.AreEqual(2.0, v.Y);
            Assert.AreEqual(3.0, v.Z);
        }

        [TestMethod]
        public void ImplicitCastFromTuple()
        {
            //arrange
            //act
            Vector3 v = (1.0, 2.0, 3.0);

            //assert
            Assert.AreEqual(1.0, v.X);
            Assert.AreEqual(2.0, v.Y);
            Assert.AreEqual(3.0, v.Z);
        }

        [TestMethod]
        public void Add()
        {
            //arrange
            Vector3 v1 = (1.0, 2.0, 3.0);
            Vector3 v2 = (0.0, -2.0, 4.0);

            //act
            Vector3 v3 = v1.Add(v2);

            //assert
            Assert.AreEqual(1.0, v3.X);
            Assert.AreEqual(0.0, v3.Y);
            Assert.AreEqual(7.0, v3.Z);
        }

        [TestMethod]
        public void Subtract()
        {
            //arrange
            Vector3 v1 = (1.0, 2.0, 3.0);
            Vector3 v2 = (0.0, -2.0, 4.0);

            //act
            Vector3 v3 = v1.Subtract(v2);

            //assert
            Assert.AreEqual(1.0, v3.X);
            Assert.AreEqual(4.0, v3.Y);
            Assert.AreEqual(-1.0, v3.Z);
        }

        [TestMethod]
        public void Multiply()
        {
            //arrange
            Vector3 v1 = (1.0, 2.0, 3.0);
            double s = 3.0;

            //act
            Vector3 v3 = v1.Multiply(s);

            //assert
            Assert.AreEqual(3.0, v3.X);
            Assert.AreEqual(6.0, v3.Y);
            Assert.AreEqual(9.0, v3.Z);
        }

        [TestMethod]
        public void Divide()
        {
            //arrange
            Vector3 v1 = (1.0, 2.0, 3.0);
            double s = 2.0;

            //act
            Vector3 v3 = v1.Divide(s);

            //assert
            Assert.AreEqual(0.5, v3.X);
            Assert.AreEqual(1.0, v3.Y);
            Assert.AreEqual(1.5, v3.Z);
        }

        [TestMethod]
        public void AddToCurrent()
        {
            //arrange
            Vector3 v1 = (1.0, 2.0, 3.0);
            Vector3 v2 = (0.0, -2.0, 4.0);

            //act
            v1.AddToCurrent(v2);

            //assert
            Assert.AreEqual(1.0, v1.X);
            Assert.AreEqual(0.0, v1.Y);
            Assert.AreEqual(7.0, v1.Z);
        }

        [TestMethod]
        public void SubtractToCurrent()
        {
            //arrange
            Vector3 v1 = (1.0, 2.0, 3.0);
            Vector3 v2 = (0.0, -2.0, 4.0);

            //act
            v1.SubtractToCurrent(v2);

            //assert
            Assert.AreEqual(1.0, v1.X);
            Assert.AreEqual(4.0, v1.Y);
            Assert.AreEqual(-1.0, v1.Z);
        }

        [TestMethod]
        public void MultiplyToCurrent()
        {
            //arrange
            Vector3 v1 = (1.0, 2.0, 3.0);
            double s = 3.0;

            //act
            v1.MultiplyToCurrent(s);

            //assert
            Assert.AreEqual(3.0, v1.X);
            Assert.AreEqual(6.0, v1.Y);
            Assert.AreEqual(9.0, v1.Z);
        }

        [TestMethod]
        public void DivideToCurrent()
        {
            //arrange
            Vector3 v1 = (1.0, 2.0, 3.0);
            double s = 2.0;

            //act
            v1.DivideToCurrent(s);

            //assert
            Assert.AreEqual(0.5, v1.X);
            Assert.AreEqual(1.0, v1.Y);
            Assert.AreEqual(1.5, v1.Z);
        }

        [TestMethod]
        public void SumOperator()
        {
            //arrange
            Vector3 v1 = (1.0, 2.0, 3.0);
            Vector3 v2 = (0.0, -2.0, 4.0);

            //act
            Vector3 v3 = v1 + v2;

            //assert
            Assert.AreEqual(1.0, v3.X);
            Assert.AreEqual(0.0, v3.Y);
            Assert.AreEqual(7.0, v3.Z);
        }

        [TestMethod]
        public void SubtractOperator()
        {
            //arrange
            Vector3 v1 = (1.0, 2.0, 3.0);
            Vector3 v2 = (0.0, -2.0, 4.0);

            //act
            Vector3 v3 = v1 - v2;

            //assert
            Assert.AreEqual(1.0, v3.X);
            Assert.AreEqual(4.0, v3.Y);
            Assert.AreEqual(-1.0, v3.Z);
        }

        [TestMethod]
        public void MultiplyOperator1()
        {
            //arrange
            Vector3 v1 = (1.0, 2.0, 3.0);
            double s = 3.0;

            //act
            Vector3 v3 = v1 * s;

            //assert
            Assert.AreEqual(3.0, v3.X);
            Assert.AreEqual(6.0, v3.Y);
            Assert.AreEqual(9.0, v3.Z);
        }

        [TestMethod]
        public void MultiplyOperator2()
        {
            //arrange
            Vector3 v1 = (1.0, 2.0, 3.0);
            double s = 3.0;

            //act
            Vector3 v3 = s * v1;

            //assert
            Assert.AreEqual(3.0, v3.X);
            Assert.AreEqual(6.0, v3.Y);
            Assert.AreEqual(9.0, v3.Z);
        }

        [TestMethod]
        public void DivideOperator()
        {
            //arrange
            Vector3 v1 = (1.0, 2.0, 3.0);
            double s = 2.0;

            //act
            Vector3 v3 = v1 / s;

            //assert
            Assert.AreEqual(0.5, v3.X);
            Assert.AreEqual(1.0, v3.Y);
            Assert.AreEqual(1.5, v3.Z);
        }

        [TestMethod]
        public void Norm()
        {
            //arrange
            Vector3 v1 = (3.0, 4.0, 0.0);

            //acr
            double n = v1.Norm();

            //assert
            Assert.AreEqual(5.0, n);
        }

        [TestMethod]
        public void StringRepresentation1()
        {
            //arrange
            Vector3 v1 = (3.0, 4.0, 0.0);

            //acr
            string s = v1.ToString();

            //assert
            Assert.AreEqual("3; 4; 0", s);
        }

        [TestMethod]
        public void StringRepresentation2()
        {
            //arrange
            Vector3 v1 = (3.01111111, 4.0239999, 0.99999);

            //acr
            string s = v1.ToString();

            //assert
            Assert.AreEqual("3,011; 4,024; 1", s);
        }
    }
}
