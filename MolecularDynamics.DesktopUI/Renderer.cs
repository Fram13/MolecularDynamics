using MolecularDynamics.Model;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MolecularDynamics.DesktopUI
{
    public class Renderer
    {
        private List<Vector3d> sphereVertices;
        private Matrix4d view;
        private int faces = 20;

        public Renderer(Color clearColor, double identityRadius)
        {
            sphereVertices = new List<Vector3d>();
            InitializeSphereVertices(identityRadius);

            GL.ClearColor(clearColor);
            GL.Enable(EnableCap.DepthTest);

            view = Matrix4d.Identity;
        }

        private void InitializeSphereVertices(double radius)
        {
            double max = 2.0 * Math.PI;
            double step = max / faces;

            for (double theta = 0.0; theta < max; theta += step)
            {
                double sinTheta = Math.Sin(theta);
                double cosTheta = Math.Cos(theta);

                for (double phi = 0.0; phi < max; phi += step)
                {
                    double sinPhi = Math.Sin(phi);
                    double cosPhi = Math.Cos(phi);

                    double x = radius * sinTheta * cosPhi;
                    double y = radius * sinTheta * sinPhi;
                    double z = radius * cosTheta;

                    sphereVertices.Add(new Vector3d(x, y, z));

                    x = radius * Math.Sin(theta + step) * cosPhi;
                    y = radius * Math.Sin(theta + step) * sinPhi;
                    z = radius * Math.Cos(theta + step);

                    sphereVertices.Add(new Vector3d(x, y, z));
                }
            }
        }

        public void Translate(double dx, double dy)
        {
            view = view * Matrix4d.CreateTranslation(dx, dy, 0.0);
        }

        public void Scale(double coefficient)
        {
            Matrix4d scale = Matrix4d.Identity;
            scale.M11 = scale.M22 = scale.M33 = coefficient;
            view = view * scale;
        }

        public void RotateX(double angle)
        {
            view = view * Matrix4d.CreateRotationX(angle);
        }

        public void RotateY(double angle)
        {
            view = view * Matrix4d.CreateRotationY(angle);
        }

        public void SetViewport(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, -1.0, 1.0);
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

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref view);
            GL.Flush();
            GL.Finish();
        }

        private void PaintParticle(Particle particle)
        {
            GL.Begin(BeginMode.QuadStrip);

            for (int i = 0; i < sphereVertices.Count; i++)
            {
                GL.Normal3(sphereVertices[i]);
                GL.Vertex3(sphereVertices[i].X + particle.Position.X, sphereVertices[i].Y + particle.Position.Y, sphereVertices[i].Z + particle.Position.Z);
            }

            GL.End();
        }
    }
}
