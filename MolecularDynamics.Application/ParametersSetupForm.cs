using MolecularDynamics.Model;
using MolecularDynamics.Model.Atoms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MolecularDynamics.Application
{
    public partial class ParametersSetupForm : Form
    {
        private Action<SimulationParameters, (int, int, int)> setupParameters;

        public ParametersSetupForm(Action<SimulationParameters, (int, int, int)> setupParameters)
        {
            InitializeComponent();
            this.setupParameters = setupParameters;
        }

        private void createButton_Click(Object sender, EventArgs e)
        {
            if (!Int32.TryParse(particleCountXTextBox.Text, out int countX) || countX <= 0)
            {
                MessageBox.Show("Введите целое положительное число большее нуля.", "Ошибка ввода");
                particleCountXTextBox.Clear();
                particleCountXTextBox.Focus();
                return;
            }

            if (!Int32.TryParse(particleCountYTextBox.Text, out int countY) || countY <= 0)
            {
                MessageBox.Show("Введите целое положительное число.", "Ошибка ввода");
                particleCountYTextBox.Clear();
                particleCountYTextBox.Focus();
                return;
            }

            if (!Int32.TryParse(particleCountZTextBox.Text, out int countZ) || countZ <= 0)
            {
                MessageBox.Show("Введите целое положительное число большее нуля.", "Ошибка ввода");
                particleCountZTextBox.Clear();
                particleCountZTextBox.Focus();
                return;
            }

            if (!Int32.TryParse(staticLayerCountTextBox.Text, out int staticLayerCount) || staticLayerCount <= 0)
            {
                MessageBox.Show("Введите целое положительное число большее нуля.", "Ошибка ввода");
                staticLayerCountTextBox.Clear();
                staticLayerCountTextBox.Focus();
                return;
            }

            if (staticLayerCount > countZ)
            {
                MessageBox.Show("Число неподвижных слоев должно быть меньше либо равно числу частиц по оси Oz.", "Ошибка ввода");
                staticLayerCountTextBox.Clear();
                staticLayerCountTextBox.Focus();
                return;
            }

            if (!Double.TryParse(stepTextBox.Text, out double step) || Math.Sign(step) < 1)
            {
                MessageBox.Show("Введите вещественное положительное число большее нуля.", "Ошибка ввода");
                stepTextBox.Clear();
                stepTextBox.Focus();
                return;
            }

            if (!Double.TryParse(periodTextBox.Text, out double period) || Math.Sign(period) < 1)
            {
                MessageBox.Show("Введите вещественное положительное число большее нуля.", "Ошибка ввода");
                periodTextBox.Clear();
                periodTextBox.Focus();
                return;
            }

            if (!Double.TryParse(temperatureTextBox.Text, out double temperature) || Math.Sign(temperature) < 1)
            {
                MessageBox.Show("Введите вещественное положительное число большее нуля.", "Ошибка ввода");
                temperatureTextBox.Clear();
                temperatureTextBox.Focus();
                return;
            }

            if (!Int32.TryParse(threadsTextBox.Text, out int threads) || threads <= 0)
            {
                MessageBox.Show("Введите целое положительное число большее нуля.", "Ошибка ввода");
                threadsTextBox.Clear();
                threadsTextBox.Focus();
                return;
            }

            Vector3 spaceSize;
            spaceSize.X = countX * Wolfram.GridConstant;
            spaceSize.Y = countY * Wolfram.GridConstant;
            spaceSize.Z = countZ * Wolfram.GridConstant * 1.5;

            var cellCount = (countX * 2, countY * 2, countZ * 3);

            SimulationParameters parameters = new SimulationParameters(spaceSize, cellCount)
            {
                IntegrationStep = step,
                DissipationCoefficient = 0.06 / 3.615,
                Temperature = temperature,
                ParticleMass = Wolfram.AtomMass,
                StaticCellLayerCount = staticLayerCount,
                InteractionRadius = 5,
                NewParticleVelocity = Math.Sqrt(3.0 * Constants.BoltzmannConstant * Wolfram.MeltingPoint / Wolfram.AtomMass),
                ParticleAppearancePeriod = period,
                Threads = threads
            };

            setupParameters(parameters, (countX, countY, countZ));

            Close();
        }
    }
}
