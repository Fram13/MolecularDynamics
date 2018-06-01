using MolecularDynamics.Model;
using MolecularDynamics.Visualization.GraphicModels;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MolecularDynamics.Application
{
    public partial class VisualizationForm : Form
    {
        private int prevMouseX, prevMouseY;

        private ParticlesModel particlesModel;
        private SpaceBorder spaceBorder;

        private Matrix4 viewModel;

        public VisualizationForm(List<Particle> particles, double sphereRadius, Model.Vector3 spaceSize, TrajectoryIntegrator integrator)
        {
            InitializeComponent();
            particlesModel = new ParticlesModel(sphereRadius, particles);
            spaceBorder = new SpaceBorder(spaceSize);

            integrator.StepComplete += particlesModel.Update;

            float translateX = (float)(-spaceSize.X / 20.0);
            float translateZ = (float)(-spaceSize.Z / 20.0);
            viewModel = Matrix4.CreateTranslation(translateX, 0.0f, translateZ) * Matrix4.CreateRotationX((float)(-Math.PI / 2.0));
        }

        #region glControl event handlers

        private void GlControl_Load(Object sender, EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Viewport(0, 0, glControl.Width, glControl.Height);

            particlesModel.Initialize();
            spaceBorder.Initialize();

            timer.Enabled = true;
        }

        private void GlControl_Paint(Object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            particlesModel.Render(ref viewModel);
            spaceBorder.Render(ref viewModel);

            GL.BindVertexArray(0);

            glControl.SwapBuffers();
        }

        private void GlControl_MouseMove(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int dx = e.X - prevMouseX;
                int dy = e.Y - prevMouseY;

                if (dx != 0)
                {
                    viewModel = viewModel * Matrix4.CreateRotationY((float)(Math.Sign(dx) * -2.0 * Math.PI / 180.0));
                }

                if (dy != 0)
                {
                    viewModel = viewModel * Matrix4.CreateRotationX((float)(Math.Sign(dy) * -2.0 * Math.PI / 180.0));
                }
            }

            prevMouseX = e.X;
            prevMouseY = e.Y;
        }

        private void GlControl_MouseWheel(Object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                viewModel = viewModel * Matrix4.CreateScale(e.Delta > 0 ? 1.1f : 0.9f);
            }
        }

        private void glControl_PreviewKeyDown(Object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Left:
                    viewModel = viewModel * Matrix4.CreateTranslation(-0.05f, 0.0f, 0.0f);
                    break;

                case Keys.Right:
                    viewModel = viewModel * Matrix4.CreateTranslation(0.05f, 0.0f, 0.0f);
                    break;

                case Keys.Up:
                    viewModel = viewModel * Matrix4.CreateTranslation(0.0f, 0.05f, 0.0f);
                    break;

                case Keys.Down:
                    viewModel = viewModel * Matrix4.CreateTranslation(0.0f, -0.05f, 0.0f);
                    break;
            }
        }

        #endregion glControl event handlers

        private void timer1_Tick(Object sender, EventArgs e)
        {
            glControl.Invalidate();
        }
    }
}
