namespace MolecularDynamics.Model.Atoms
{
    public class TestAtom : Particle
    {
        public const double Radius = 0.141;

        public TestAtom()
        {
            Mass = 184.84;
        }

        public override double PairForce(double distance)
        {
            if (distance < 2)
            {
                return -1;
            }

            if (distance < 4)
            {
                return 1;
            }

            return 0;
        }
    }
}
