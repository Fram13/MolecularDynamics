using System;
using MolecularDynamics.Model;
using OpenTK.Graphics.OpenGL;

namespace MolecularDynamics.DesktopUI
{
    public class Renderer
    {
        public void Draw(Particle sphere)
        {
            double max = 2.0 * Math.PI;
            double step = max / 50;

            for (double theta = 0.0; theta < max; theta += step)
            {
                GL.Begin(BeginMode.QuadStrip);

                double sinTheta = Math.Sin(theta);
                double cosTheta = Math.Cos(theta);

                for (double phi = 0.0; phi < max; phi += step)
                {
                    double sinPhi = Math.Sin(phi);
                    double cosPhi = Math.Cos(phi);

                    double x = sphere.Radius * sinTheta * cosPhi;
                    double y = sphere.Radius * sinTheta * sinPhi;
                    double z = sphere.Radius * cosTheta;

                    GL.Normal3(x, y, z);
                    GL.Vertex3(x + sphere.Position.X, y + sphere.Position.Y, z + sphere.Position.Z);

                    x = sphere.Radius * Math.Sin(theta + step) * cosPhi;
                    y = sphere.Radius * Math.Sin(theta + step) * sinPhi;
                    z = sphere.Radius * Math.Cos(theta + step);

                    GL.Normal3(x, y, z);
                    GL.Vertex3(x + sphere.Position.X, y + sphere.Position.Y, z + sphere.Position.Z);
                }

                GL.End();
            }
        }
    }
}
