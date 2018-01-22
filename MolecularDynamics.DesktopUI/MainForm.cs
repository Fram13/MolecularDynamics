using MolecularDynamics.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MolecularDynamics.DesktopUI
{
    public partial class MainForm : Form
    {
        private ParticleTrajectoryIntegrator intergrator;
        private Renderer renderer;        

        private int prevMouseX, prevMouseY;
        private double rotateSpeed = 0.1;
        private double scaleCoefficient = 0.05;
        private double translateCoefficient = 0.1;

        private List<Particle> particles = new List<Particle>()
        {
            new Particle()
            {
                Position = new Vector3(0.0, 0.0, 0.0),
                InitialVelocity = (0.0, 0.0, 0.0),
                Radius = 0.2,
                Mass = 1
            },
            new Particle()
            {
                Position = new Vector3(0.5, 0.0, 0.0),
                InitialVelocity = (-2.5, 0.0, 0.0),
                Radius = 0.2,
                Mass = 1.2
            },
            new Particle()
            {
                Position = new Vector3(0.0, 0.5, 0.0),
                InitialVelocity = (0.0, -2.5, 0.0),
                Radius = 0.2,
                Mass = 1
            }
        };

        public MainForm()
        {
            InitializeComponent();
        }

        #region Event handlers

        private void MainForm_Load(Object sender, EventArgs e)
        {
            intergrator = new ParticleTrajectoryIntegrator(particles, 0.0025, 10);
            intergrator.InitialStep();

            renderer = new Renderer(Color.White);

            MainForm_Resize(sender, e);
            //timer.Enabled = true;
        }

        private void MainForm_Resize(Object sender, EventArgs e)
        {
            renderer.SetViewport(glControl.Width, glControl.Height);
            glControl.Invalidate();
        }

        private void Timer_Tick(Object sender, EventArgs e)
        {
            intergrator.NextStep();
            glControl.Invalidate();
        }

        private void button1_Click(Object sender, EventArgs e)
        {
            timer.Enabled = true;
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
                renderer.RotateY((e.X - prevMouseX) * rotateSpeed * Math.PI / 180.0);
                prevMouseX = e.X;

                renderer.RotateX((e.Y - prevMouseY) * rotateSpeed * Math.PI / 180.0);
                prevMouseY = e.Y;

                glControl.Invalidate();
            }
        }

        private void glControl_KeyDown(Object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.E:
                    renderer.Scale(1.0 + scaleCoefficient);
                    glControl.Invalidate();
                    break;

                case Keys.Q:
                    renderer.Scale(1.0 - scaleCoefficient);
                    glControl.Invalidate();
                    break;

                case Keys.W:
                    //renderer.Translate(0.0, translateCoefficient, 0.0);
                    renderer.RotateX(5.0 / 180.0 * Math.PI);
                    glControl.Invalidate();
                    break;

                case Keys.S:
                    //renderer.Translate(0.0, -translateCoefficient, 0.0);
                    renderer.RotateX(-5.0 / 180.0 * Math.PI);
                    glControl.Invalidate();
                    break;

                case Keys.A:
                    //renderer.Translate(-translateCoefficient, 0.0, 0.0);
                    renderer.RotateY(5.0 / 180.0 * Math.PI);
                    glControl.Invalidate();
                    break;

                case Keys.D:
                    //renderer.Translate(translateCoefficient, 0.0, 0.0);
                    renderer.RotateY(-5.0 / 180.0 * Math.PI);
                    glControl.Invalidate();
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
