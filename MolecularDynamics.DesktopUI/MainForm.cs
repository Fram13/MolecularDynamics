using MolecularDynamics.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MolecularDynamics.DesktopUI
{
    public partial class MainForm : Form
    {
        private bool glControlLoaded;
        private ParticleTrajectoryIntegrator intergrator;
        private Renderer renderer;

        private readonly double toRadians = Math.PI / 180.0;        
        private int prevMouseX, prevMouseY;
        private double rotateCoefficient = 0.15;
        private double scaleCoefficient = 0.1;
        private double translateCoefficient = 0.05;

        private List<Particle> particles = new List<Particle>()
        {
            new Particle()
            {
                Position = new Vector3(0.0, 0.0, 0.0),
                InitialVelocity = (1.0, 0.0, 0.0),
                Radius = 0.2,
                Mass = 1
            },
            new Particle()
            {
                Position = new Vector3(0.5, 0.0, 0.0),
                InitialVelocity = (1.1, -0.2, 0.0),
                Radius = 0.2,
                Mass = 1
            },
            new Particle()
            {
                Position = new Vector3(0.0, 0.5, 0.0),
                InitialVelocity = (1.3, 0.2, 0.0),
                Radius = 0.2,
                Mass = 1
            },
            new Particle()
            {
                Position = new Vector3(0.7, 0.5, 0.0),
                InitialVelocity = (-1.3, 0.2, 0.0),
                Radius = 0.2,
                Mass = 1
            },new Particle()
            {
                Position = new Vector3(-0.5, 0.5, 0.0),
                InitialVelocity = (1.3, -0.2, 0.0),
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
            glControlLoaded = true;
            renderer = new Renderer(Color.White);
            intergrator = new ParticleTrajectoryIntegrator(particles, 0.00125, 1);
            intergrator.InitialStep();

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
                    renderer.Translate(0.0, -translateCoefficient);
                    glControl.Invalidate();
                    break;

                case Keys.S:
                    renderer.Translate(0.0, translateCoefficient);
                    glControl.Invalidate();
                    break;

                case Keys.A:
                    renderer.Translate(translateCoefficient, 0.0);
                    glControl.Invalidate();
                    break;

                case Keys.D:
                    renderer.Translate(-translateCoefficient, 0.0);
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
