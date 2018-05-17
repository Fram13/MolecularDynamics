using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.Distributions;

namespace MolecularDynamics.Model
{
    public static class Constants
    {
        public const double DissipationCoefficient = 0.06 / 3.615;
        public const double BoltzmannConstant = 8.617332478e-5;
        public const double WolframAtomMass = 183.84; //а. е. м.
        public const double Temperature = 300; //К

        public const double Step = 1e-14; //с
        
        public static readonly double RandomForceLength = Math.Sqrt(2.0 * DissipationCoefficient * BoltzmannConstant *
                                                                    WolframAtomMass * Temperature / Step);

        public static Vector3 RandomNormalVector
        {
            get
            {
                Vector3 vec = (Normal.Sample(0.0, 1.0), Normal.Sample(0.0, 1.0), Normal.Sample(0.0, 1.0));
                double norm = vec.Norm();
                return vec.DivideToCurrent(norm);
            }
        }
    }
}
