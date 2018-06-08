using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using MolecularDynamics.Model;
using MolecularDynamics.Model.Atoms;
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
        private Form visualizationForm;

        private Random random;
        private CancellationTokenSource cts;

        public MainForm()
        {
            InitializeComponent();
            random = new Random();
        }

        private void InitializeSimulationComponents()
        {
            particleGrid = new ParticleGrid(parameters);
            particleGrid.AddParticles(particles);
            integrator = new TrajectoryIntegrator(particleGrid, parameters);

            if (visualizationForm != null)
            {
                visualizationForm.Close();
            }

            visualizationForm = new VisualizationForm(particles, Wolfram.Radius, parameters.SpaceSize, integrator);
            visualizationForm.FormClosed += VisualizationFormClosed;
            visualizationForm.Show();
        }

        private void Integrate(CancellationToken ct)
        {
            double nextParticleTime = 0.0;

            while (!ct.IsCancellationRequested)
            {
                integrator.NextStep();
                nextParticleTime += parameters.IntegrationStep;
                parameters.TotalTime += parameters.IntegrationStep;

                if (nextParticleTime > parameters.ParticleAppearancePeriod)
                {
                    Vector3 position;
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

        private void VisualizationFormClosed(Object sender, EventArgs e)
        {
            visualizationForm = null;
        }

        private void timer_Tick(Object sender, EventArgs e)
        {
            dataGridView.Rows.Add(parameters.TotalTime, particles.Count, Math.Round(particles.Temperature(), 4));
            List<double> density = particles.LayersDensity(parameters);
            Object[] cells = new Object[density.Count];

            for (int i = 0; i < density.Count; i++)
            {
                cells[i] = Math.Round(density[i], 3);
            }

            densityDataGridView.Rows.Add(cells);
        }

        private void CreateConfiguration(SimulationParameters p, (int, int, int) cellCount)
        {
            parameters = p;
            particles = ParticleGenerator.GenerateWolframGrid((0.7, 0.7, 0.7), cellCount, parameters.StaticCellLayerCount);
            CreateColumns();
        }

        private void CreateColumns()
        {
            int layers = (int)Math.Ceiling(parameters.SpaceSize.Z / Wolfram.GridConstant);

            for (int i = 0; i < layers; i++)
            {
                densityDataGridView.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    HeaderText = $"Слой {i + 1}",
                    Name = $"Layer{i + 1}Column",
                    ReadOnly = true
                });
            }
        }

        #region ToolStrip event handlers

        private void CreateConfigurationToolStripMenuItem_Click(Object sender, EventArgs e)
        {
            Form setupParamatersForm = new ParametersSetupForm(CreateConfiguration);
            setupParamatersForm.ShowDialog();
            InitializeSimulationComponents();

            SaveConfigurationToolStripMenuItem.Enabled = true;
        }

        private void LoadConfigurationToolStripMenuItem_Click(Object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (Stream stream = new FileStream(openFileDialog.FileName, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    parameters = (SimulationParameters)bf.Deserialize(stream);
                    particles = (List<Particle>)bf.Deserialize(stream);
                }
            }

            InitializeSimulationComponents();
            CreateColumns();

            SaveConfigurationToolStripMenuItem.Enabled = true;
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

        private void StartStopIntegrationButton_Click(Object sender, EventArgs e)
        {
            if (cts == null || cts.IsCancellationRequested)
            {
                cts = new CancellationTokenSource();
                Task.Run(() => Integrate(cts.Token));
                timer.Enabled = true;
                startStopIntegrationButton.Text = "Пауза";
            }
            else
            {
                cts.Cancel();
                timer.Enabled = false;
                startStopIntegrationButton.Text = "Старт";
            }
        }

        private void changeVisualizationToolStripButton_Click(Object sender, EventArgs e)
        {
            if (visualizationForm == null)
            {
                visualizationForm = new VisualizationForm(particles, Wolfram.Radius, parameters.SpaceSize, integrator);
                visualizationForm.FormClosed += VisualizationFormClosed;
                visualizationForm.Show();
            }
        }

        #endregion ToolStrip event handlers
    }
}
