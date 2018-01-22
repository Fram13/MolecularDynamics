using MolecularDynamics.Model;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MolecularDynamics.DesktopUI
{
    public class Renderer
    {
        private Matrix4d view;

        public Renderer(Color clearColor)
        {
            GL.ClearColor(clearColor);
            GL.Enable(EnableCap.DepthTest);
            view = Matrix4d.Identity;
        }

        public void Translate(double dx, double dy, double dz)
        {
            Vector3d center = new Vector3d(dx, dy, -1.0 + dz);
            Vector3d eye = new Vector3d(dx, dy, dz);
            Vector3d up = new Vector3d(dx, dy + 1.0, dz);

            //view = Matrix4d.LookAt(eye, center, up) * view;
        }

        public void Scale(double coefficient)
        {
            Vector3d center = new Vector3d(0.0, 0.0, -coefficient);
            Vector3d eye = new Vector3d(0.0, 0.0, 0.0);
            Vector3d up = new Vector3d(0.0, coefficient, 0.0);

            //view = Matrix4d.LookAt(eye, center, up) * view;
        }

        public void RotateX(double angle)
        {
            view = view * Matrix4d.CreateRotationX(angle);
        }

        public void RotateY(double angle)
        {
            view = view * Matrix4d.CreateRotationY(angle);
        }

        public void RotateZ(double angle)
        {
            
        }

        public void SetViewport(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
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

            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            //GL.Ortho(-1.0, 1.0, -1.0, 1.0, -1.0, 1.0);

            //GL.Begin(BeginMode.Lines);

            //GL.Color3(Color.Red);
            //GL.Vertex3(0.0, 0.0, 0.0);
            //GL.Vertex3(0.8, 0.0, 0.0);

            //GL.Color3(Color.Green);
            //GL.Vertex3(0.0, 0.0, 0.0);
            //GL.Vertex3(0.0, 0.8, 0.0);

            //GL.Color3(Color.Blue);
            //GL.Vertex3(0.0, 0.0, 0.0);
            //GL.Vertex3(0.0, 0.0, 0.8);

            //GL.End();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref view);
        }

        private void PaintParticle(Particle particle)
        {
            double max = 2.0 * Math.PI;
            double step = max / 50;

            GL.Begin(BeginMode.QuadStrip);

            for (double theta = 0.0; theta < max; theta += step)
            {
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
            }

            GL.End();
        }
    }
}
