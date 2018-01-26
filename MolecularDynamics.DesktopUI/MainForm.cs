using MolecularDynamics.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MolecularDynamics.DesktopUI
{
    public partial class MainForm : Form
    {
        private ParticleGenerator generator;
        private ParticleTrajectoryIntegrator intergrator;
        private bool glControlLoaded;
        private Renderer renderer;

        private readonly double toRadians = Math.PI / 180.0;        
        private int prevMouseX, prevMouseY;
        private double rotateCoefficient = 0.15;
        private double scaleCoefficient = 0.1;
        private double translateCoefficient = 0.05;

        private IList<Particle> particles;

        public MainForm()
        {
            InitializeComponent();
        }

        #region Event handlers

        private void MainForm_Load(Object sender, EventArgs e)
        {
            glControlLoaded = true;

            generator = new ParticleGenerator();
            particles = generator.Generate2DGrid(10, 10);
            intergrator = new ParticleTrajectoryIntegrator(particles, 0.00125, 0.1, 1);

            renderer = new Renderer(Color.White, particles[0].Radius);

            MainForm_Resize(sender, e);
        }

        private void MainForm_Resize(Object sender, EventArgs e)
        {
            if (glControlLoaded)
            {
                renderer.SetViewport(glControl.Width, glControl.Height);
                glControl.Invalidate(); 
            }
        }

        private void Timer_Tick(Object sender, EventArgs e)
        {
            intergrator.NextStep();
            glControl.Invalidate();
        }

        #region glControl event handlers

        private void GlControl_Paint(Object sender, PaintEventArgs e)
        {
            renderer.Paint(particles);
            glControl.SwapBuffers();
        }

        private void GlControl_MouseDown(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                prevMouseX = e.X;
                prevMouseY = e.Y;
            }
        }

        private void GlControl_MouseMove(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                renderer.RotateY((e.X - prevMouseX) * rotateCoefficient * toRadians);
                prevMouseX = e.X;

                renderer.RotateX((e.Y - prevMouseY) * rotateCoefficient * toRadians);
                prevMouseY = e.Y;

                if (!timer.Enabled)
                {
                    glControl.Invalidate();
                }
            }
        }

        private void GLControl_KeyDown(Object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.E:
                    renderer.Scale(1.0 + scaleCoefficient);

                    if (!timer.Enabled)
                    {
                        glControl.Invalidate();
                    }

                    break;

                case Keys.Q:
                    renderer.Scale(1.0 - scaleCoefficient);

                    if (!timer.Enabled)
                    {
                        glControl.Invalidate();
                    }

                    break;

                case Keys.W:
                    renderer.Translate(0.0, -translateCoefficient);

                    if (!timer.Enabled)
                    {
                        glControl.Invalidate();
                    }

                    break;

                case Keys.S:
                    renderer.Translate(0.0, translateCoefficient);

                    if (!timer.Enabled)
                    {
                        glControl.Invalidate();
                    }

                    break;

                case Keys.A:
                    renderer.Translate(translateCoefficient, 0.0);

                    if (!timer.Enabled)
                    {
                        glControl.Invalidate();
                    }

                    break;

                case Keys.D:
                    renderer.Translate(-translateCoefficient, 0.0);

                    if (!timer.Enabled)
                    {
                        glControl.Invalidate();
                    }

                    break;

                case Keys.Space:
                    timer.Enabled = !timer.Enabled;
                    break;
            }
        }

        #endregion glControl event handlers

        #endregion Event handlers
    }
}
