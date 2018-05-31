using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using MolecularDynamics.Model;
using MolecularDynamics.Model.Atoms;
using MolecularDynamics.Visualization.GraphicModels;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace MolecularDynamics.Application
{
    public partial class MainForm : Form
    {
        private List<Particle> particles;
        private SimulationParameters parameters;
        private ParticleGrid particleGrid;
        private TrajectoryIntegrator integrator;

        private double nextParticleTime;
        private double totalTime;
        private Random random;

        public MainForm()
        {
            InitializeComponent();
            random = new Random();
        }

        private void CreateConfigurationToolStripMenuItem_Click(Object sender, EventArgs e)
        {
            CreateConfiguration();
            InitializeComponents();           
            
            SaveConfigurationToolStripMenuItem.Enabled = true;
        }

        private void MainForm_Load(Object sender, EventArgs e)
        {
            saveFileDialog.Filter = "Бинарные файлы (*.bin)|*.bin";
            openFileDialog.Filter = "Бинарные файлы (*.bin)|*.bin";
        }

        private void CreateConfiguration()
        {
            parameters = new SimulationParameters(
                spaceSize: (10.5 * Wolfram.GridConstant, 10.5 * Wolfram.GridConstant, 21 * Wolfram.GridConstant),
                cellCount: (20, 20, 40))
            {
                IntegrationStep = 0.1,
                DissipationCoefficient = 0.06 / 3.615,
                Temperature = 200,
                ParticleMass = Wolfram.AtomMass,
                StaticCellLayerCount = 6,
                InteractionRadius = 4,
                NewParticleVelocity = 0.6,
                Threads = 4
            };

            if (particles == null)
            {
                particles = new List<Particle>();
            }

            particles.Clear();
            particles.AddRange(ParticleGenerator.GenerateWolframGrid((1.5, 1.5, 1.5), (10, 10, 20), parameters.StaticCellLayerCount));
        }

        private void InitializeComponents()
        {
            particleGrid = new ParticleGrid(parameters.SpaceSize, parameters.CellCount, parameters.CellSize, parameters.InteractionRadius, parameters.Threads);
            particleGrid.AddParticles(particles);
            integrator = new TrajectoryIntegrator(particleGrid, parameters);

            Form form = new VisualizationForm(particles, Wolfram.Radius, parameters.SpaceSize, integrator);
            form.Show(this);
        }

        private CancellationTokenSource cts;

        private void startStopIntegrationButton_Click(Object sender, EventArgs e)
        {
            if (cts == null || cts.IsCancellationRequested)
            {
                cts = new CancellationTokenSource();
                Task.Run(() => Integrate(cts.Token));
                startStopIntegrationButton.Text = "Пауза";
            }
            else
            {
                cts.Cancel();
                startStopIntegrationButton.Text = "Старт";
            }
        }

        private void Integrate(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                integrator.NextStep();
                nextParticleTime += parameters.IntegrationStep;
                totalTime += parameters.IntegrationStep;

                if (nextParticleTime > 400)
                {
                    Model.Vector3 position;
                    position.X = parameters.SpaceSize.X * random.NextDouble();
                    position.Y = parameters.SpaceSize.Y * random.NextDouble();
                    position.Z = parameters.SpaceSize.Z - Wolfram.Radius;

                    Particle particle = new Wolfram()
                    {
                        Position = position,
                        Velocity = (0, 0, -parameters.NewParticleVelocity)
                    };

                    particleGrid.AddParticle(particle);
                    particles.Add(particle);

                    nextParticleTime = 0.0;
                } 
            }
        }

        private void SaveConfigurationToolStripMenuItem_Click(Object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (Stream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(stream, parameters);
                    bf.Serialize(stream, particles);
                }
            }
        }

        private void LoadConfigurationToolStripMenuItem_Click(Object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (Stream stream = new FileStream(openFileDialog.FileName, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    parameters = (SimulationParameters)bf.Deserialize(stream);

                    if (particles == null)
                    {
                        particles = new List<Particle>();
                    }

                    particles.Clear();
                    particles.AddRange((List<Particle>)bf.Deserialize(stream));
                }
            }

            InitializeComponents();
        }
    }
}
