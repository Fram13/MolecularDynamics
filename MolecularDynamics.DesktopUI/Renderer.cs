using MolecularDynamics.Model;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MolecularDynamics.DesktopUI
{
    public class Renderer
    {
        public double RotateX { get; set; }

        public double RotateY { get; set; }

        public double Scale { get; set; } = 1.0;

        public double TranslateX { get; set; }

        public double TranslateY { get; set; }

        public Renderer(Color clearColor)
        {
            GL.ClearColor(clearColor);
            GL.Enable(EnableCap.DepthTest);
        }

        public void SetViewport(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1, 1, -1, 1, -1, 1);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        public void Paint(IEnumerable<Particle> particles)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            foreach (var particle in particles)
            {
                PaintParticle(particle);
            }

            GL.Disable(EnableCap.Lighting);
            
            GL.LoadIdentity();

            GL.Rotate(RotateX, 1.0, 0.0, 0.0);
            GL.Rotate(RotateY, 0.0, 1.0, 0.0);

            GL.Scale(Scale, Scale, Scale);

            GL.Translate(TranslateX, TranslateY, 0.0);

            GL.Flush();
            GL.Finish();
        }

        private void PaintParticle(Particle particle)
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

                    double x = particle.Radius * sinTheta * cosPhi;
                    double y = particle.Radius * sinTheta * sinPhi;
                    double z = particle.Radius * cosTheta;

                    GL.Normal3(x, y, z);
                    GL.Vertex3(x + particle.Position.X, y + particle.Position.Y, z + particle.Position.Z);

                    x = particle.Radius * Math.Sin(theta + step) * cosPhi;
                    y = particle.Radius * Math.Sin(theta + step) * sinPhi;
                    z = particle.Radius * Math.Cos(theta + step);

                    GL.Normal3(x, y, z);
                    GL.Vertex3(x + particle.Position.X, y + particle.Position.Y, z + particle.Position.Z);
                }

                GL.End();
            }
        }
    }
}
