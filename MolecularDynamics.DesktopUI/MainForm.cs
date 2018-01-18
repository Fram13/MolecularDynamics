using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MolecularDynamics.Model;

namespace MolecularDynamics.DesktopUI
{
    public partial class MainForm : Form
    {
        private ParticleTrajectoryIntegrator intergrator;
        private Renderer renderer;        

        private int prevMouseX, prevMouseY;
        private double rotateSpeed = 0.5;
        private double scaleCoefficient = 0.05;
        private double translateCoefficient = 0.01;

        private List<Particle> particles = new List<Particle>()
        {
            new Particle()
            {
                Position = new Vector3(0.0, 0.0, 0.0),
                InitialVelocity = (0.0, 0.0, 0.0),
                Radius = 0.05,
                Mass = 1
            },
            new Particle()
            {
                Position = new Vector3(0.7, 0.0, 0.0),
                InitialVelocity = (-2.5, 0.0, 0.0),
                Radius = 0.05,
                Mass = 1.2
            },
            new Particle()
            {
                Position = new Vector3(0.0, 0.7, 0.0),
                InitialVelocity = (0.0, -2.5, 0.0),
                Radius = 0.1,
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
                renderer.RotateY += (e.X - prevMouseX) * rotateSpeed;
                prevMouseX = e.X;

                renderer.RotateX += (e.Y - prevMouseY) * rotateSpeed;
                prevMouseY = e.Y;

                glControl.Invalidate();
            }
        }

        private void glControl_KeyDown(Object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Add:
                    renderer.Scale += scaleCoefficient;
                    glControl.Invalidate();
                    break;

                case Keys.Subtract:
                    if (renderer.Scale - scaleCoefficient > 0.0)
                    {
                        renderer.Scale -= scaleCoefficient;
                    }
                    glControl.Invalidate();
                    break;

                case Keys.W:
                    renderer.TranslateY += translateCoefficient;
                    glControl.Invalidate();
                    break;

                case Keys.S:
                    renderer.TranslateY -= translateCoefficient;
                    glControl.Invalidate();
                    break;

                case Keys.A:
                    renderer.TranslateX -= translateCoefficient;
                    glControl.Invalidate();
                    break;

                case Keys.D:
                    renderer.TranslateX += translateCoefficient;
                    glControl.Invalidate();
                    break;
            }
        }

        #endregion glControl event handlers

        #endregion Event handlers
    }
}
