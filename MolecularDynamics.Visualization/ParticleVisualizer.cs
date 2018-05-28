using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using MolecularDynamics.Model;
using MolecularDynamics.Visualization.GraphicModels;

namespace MolecularDynamics.Visualization
{
    /// <summary>
    /// Представляет визуализатор частиц.
    /// </summary>
    public class ParticleVisualizer : GameWindow 
    {
        #region Fields

        private const int Faces = 10;

        private Matrix4 _viewModel;

        private Object _positionsSyncronizer;
        private float[] _positions;
        private float[] _nextPositions;

        private List<Particle> _particles;
        private TrajectoryIntegrator _integrator;
        private CancellationTokenSource _cts;

        private GraphicModel _sphere;
        private GraphicModel _spaceBorder;        

        #endregion Fields

        /// <summary>
        /// Создает новый экземпляр <see cref="ParticleVisualizer"/>.
        /// </summary>
        /// <param name="particles">Список визуализируемых частиц.</param>
        /// <param name="integrator">Интегратор траекторий движения частиц.</param>
        /// <param name="spaceSize">Размеры пространства моделирования.</param>
        /// <param name="sphereRadius">Радиус частицы, нм.</param>
        public ParticleVisualizer(List<Particle> particles, TrajectoryIntegrator integrator, Model.Vector3 spaceSize, double sphereRadius)
        {
            _particles = particles;
            _integrator = integrator;

            _positionsSyncronizer = new Object();
            _sphere = new Sphere(sphereRadius, Faces);
            _spaceBorder = new SpaceBorder(spaceSize);

            _positions = new float[particles.Count * 3];
            _nextPositions = new float[particles.Count * 3];

            for (int i = 0; i < particles.Count; i++)
            {
                Particle particle = particles[i];

                _positions[3 * i] = (float)(particle.Position.X / 10.0);
                _positions[3 * i + 1] = (float)(particle.Position.Y / 10.0);
                _positions[3 * i + 2] = (float)(particle.Position.Z / 10.0);
            }

            _viewModel = Matrix4.CreateRotationX((float)(-Math.PI / 2.0));            
            
            Load += LoadHandler;
            Resize += ResizeHandler;
            RenderFrame += RenderFrameHandler;
            MouseMove += MouseMoveHandler;
            MouseWheel += MouseWheelHandler;
            KeyDown += KeyDownHandler;
        }

        /// <summary>
        /// Освобождает неуправляемые ресурсы.
        /// <paramref name="manual"/>True - если метод был вызван приложением, false - если метод был вызван финализирующим потоком.
        /// </summary>
        protected override void Dispose(bool manual)
        {
            _sphere.Dispose();
            _spaceBorder.Dispose();

            base.Dispose(manual);
        }

        #region Private methods

        /// <summary>
        /// Обменивает буферы компонент позиций частиц.
        /// </summary>
        private void SwapPositionsBuffers()
        {
            lock (_positionsSyncronizer)
            {
                float[] temp = _positions;
                _positions = _nextPositions;
                _nextPositions = temp;
            }
        }

        private void Integrate(CancellationToken ct)
        {
            //int c = 0;
            //Random random = new Random();

            while (!ct.IsCancellationRequested)
            {
                //if (c == 2500)
                //{
                //    var p = new Model.Atoms.Wolfram()
                //    {
                //        Position = (_spaceSize.X * 10.0 * random.NextDouble(), _spaceSize.Y * 10.0 * random.NextDouble(), (_spaceSize.Z - _sphereRadius) * 10.0),
                //        Velocity = (0, 0, -0.4)
                //    };

                //    _particles.Add(p);
                //    _integrator.Grid.AddParticle(p);
                //    _nextPositions[_particles.Count] = (float)(p.Position.X / 10.0);
                //    _nextPositions[_particles.Count + 1] = (float)(p.Position.Y / 10.0);
                //    _nextPositions[_particles.Count + 2] = (float)(p.Position.Z / 10.0);

                //    lock (_positionsSyncronizer)
                //    {
                //        _positions[_particles.Count] = _nextPositions[_particles.Count];
                //        _positions[_particles.Count + 1] = _nextPositions[_particles.Count + 1];
                //        _positions[_particles.Count + 2] = _nextPositions[_particles.Count + 2];
                //    }

                //    c = 0;
                //}

                _integrator.NextStep();
                //c++;

                for (int i = 0; i < _particles.Count; i++)
                {
                    _nextPositions[3 * i] = (float)(_particles[i].Position.X / 10.0);
                    _nextPositions[3 * i + 1] = (float)(_particles[i].Position.Y / 10.0);
                    _nextPositions[3 * i + 2] = (float)(_particles[i].Position.Z / 10.0);
                }

                SwapPositionsBuffers();
            }
        }

        private void ShowStatistics(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                Thread.Sleep(5000);
                Console.WriteLine(_particles.Temperature());
            }
        }

        #endregion Private methods

        #region Event handlers

        private void LoadHandler(Object sender, EventArgs e)
        {
            VSync = VSyncMode.On;
            Width = 800;
            Height = 800;
            Title = "Particle Visualizer";

            _sphere.Initialize();
            _spaceBorder.Initialize();

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
        }

        private void ResizeHandler(Object sender, EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
        }

        private void RenderFrameHandler(Object sender, FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            lock (_positionsSyncronizer)
            {
                _sphere.Render(ref _viewModel, _positions);
            }
            
            _spaceBorder.Render(ref _viewModel);
            
            GL.BindVertexArray(0);
            
            SwapBuffers();
        }

        private void MouseMoveHandler(Object sender, MouseMoveEventArgs e)
        {
            if (e.Mouse[MouseButton.Left])
            {
                if (e.XDelta != 0)
                {
                    _viewModel = _viewModel * Matrix4.CreateRotationY((float)(Math.Sign(e.XDelta) * -2.0 * Math.PI / 180.0));
                }
                
                if (e.YDelta != 0)
                {
                    _viewModel = _viewModel * Matrix4.CreateRotationX((float)(Math.Sign(e.YDelta) * -2.0 * Math.PI / 180.0));
                }
            }
        }

        private void MouseWheelHandler(Object sender, MouseWheelEventArgs e)
        {
            if (e.Delta != 0)
            {
                _viewModel = _viewModel * Matrix4.CreateScale(e.Delta > 0 ? 1.05f : 0.95f);
            }
        }

        private void KeyDownHandler(Object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    _viewModel = _viewModel * Matrix4.CreateTranslation(-0.05f, 0.0f, 0.0f);
                    break;

                case Key.Right:
                    _viewModel = _viewModel * Matrix4.CreateTranslation(0.05f, 0.0f, 0.0f);
                    break;

                case Key.Up:
                    _viewModel = _viewModel * Matrix4.CreateTranslation(0.0f, 0.05f, 0.0f);
                    break;

                case Key.Down:
                    _viewModel = _viewModel * Matrix4.CreateTranslation(0.0f, -0.05f, 0.0f);
                    break;

                case Key.Space:
                    if (_cts == null || _cts.IsCancellationRequested)
                    {
                        _cts = new CancellationTokenSource();
                        Task.Run(() => Integrate(_cts.Token));
                        Task.Run(() => ShowStatistics(_cts.Token));
                    }
                    else
                    {
                        _cts.Cancel();
                    }
                    break;
            }
        }
        
        #endregion Event handlers
    }
}