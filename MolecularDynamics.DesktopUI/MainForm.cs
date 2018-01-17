using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using MolecularDynamics.Model;

namespace MolecularDynamics.DesktopUI
{
    public partial class MainForm : Form
    {
        private ParticleTrajectoryIntegrator intergrator;
        private Renderer renderer;
        private List<Particle> objects;

        public MainForm()
        {
            InitializeComponent();
        }

        #region Event handlers

        private void MainForm_Load(Object sender, EventArgs e)
        {
            renderer = new Renderer();

            objects = new List<Particle>()
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
                    Position = new Vector3(0.7, 0.0, 0.0),
                    InitialVelocity = (0.0, 0.0, 0.0),
                    Radius = 0.2,
                    Mass = 1.2
                },
                new Particle()
                {
                    Position = new Vector3(0.0, 0.7, 0.0),
                    InitialVelocity = (0.0, 0.0, 0.0),
                    Radius = 0.2,
                    Mass = 1
                }
            };

            intergrator = new ParticleTrajectoryIntegrator(objects, 0.5, 10);
            intergrator.InitialStep();

            GL.ClearColor(0.9f, 0.9f, 0.9f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            MainForm_Resize(sender, e);
            timer1.Interval = 1000;
            timer1.Enabled = true;
        }

        private void MainForm_Resize(Object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1, 1, -1, 1, -1, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            glControl.Invalidate();
        }

        double AngleX, AngleY;//, AngleZ;

        private void GLControl_Paint(Object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            foreach (var s in intergrator.Particles)
            {
                renderer.Draw(s);
            }

            GL.Disable(EnableCap.Lighting);

            // поворот изображения
            GL.LoadIdentity();
            GL.Rotate(AngleX, 1.0, 0.0, 0.0);
            GL.Rotate(AngleY, 0.0, 1.0, 0.0);
            //GL.Rotate(AngleZ, 0.0, 0.0, 1.0);

            GL.Scale(scale, scale, scale);

            // формирование осей координат
            GL.Flush();
            GL.Finish();
            glControl.SwapBuffers();
        }

        private void GlControl_MouseMove(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                AngleX += e.X / 100f;
                AngleY += e.Y / 100f;
                glControl.Invalidate();
            }
        }

        private double scale = 1.0;

        private void GlControl_MouseWheel(Object sender, MouseEventArgs e)
        {
            scale += e.Delta / 1000.0;
            glControl.Invalidate();
        }

        private void Timer1_Tick(Object sender, EventArgs e)
        {
            intergrator.NextStep();
            glControl.Invalidate();
        }

        #endregion Event handlers
    }
}
