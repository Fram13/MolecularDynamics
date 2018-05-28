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

        private Matrix4 _viewModel;

        private Object _positionsSynchronizer;
        private float[] _positions;
        private float[] _nextPositions;

        private List<Particle> _particles;
        private CancellationTokenSource _cts;

        private GraphicModel _sphere;
        private GraphicModel _spaceBorder;

        private Sphere.RenderingParameters _sphereRenderingParameters;

        #endregion Fields

        /// <summary>
        /// Обновляет частицы.
        /// </summary>
        public Action<List<Particle>> UpdateParticlesExternally { get; set; }

        /// <summary>
        /// Обновляет статистику.
        /// </summary>
        public Action<CancellationToken> UpdateStatistics { get; set; }

        /// <summary>
        /// Создает новый экземпляр <see cref="ParticleVisualizer"/>.
        /// </summary>
        /// <param name="particles">Список визуализируемых частиц.</param>
        /// <param name="spaceSize">Размеры пространства моделирования, А.</param>
        /// <param name="sphereRadius">Радиус частицы, нм.</param>
        public ParticleVisualizer(List<Particle> particles, Model.Vector3 spaceSize, double sphereRadius)
        {
            _particles = particles;            
            _sphere = new Sphere(sphereRadius);
            _spaceBorder = new SpaceBorder(spaceSize);
            _sphereRenderingParameters = new Sphere.RenderingParameters();
            _positionsSynchronizer = new Object();

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

        #region Event handlers

        private void LoadHandler(Object sender, EventArgs e)
        {
            VSync = VSyncMode.On;
            Width = 800;
            Height = 800;
            Title = "Визуализатор частиц";

            AllocatePositionBuffers();
            UpdatePositionBuffer();

            _sphere.Initialize();
            _spaceBorder.Initialize();

            _sphereRenderingParameters.Positions = _positions;
            _sphereRenderingParameters.InstanceCount = _particles.Count;
            _sphereRenderingParameters.PositionSynchronizer = _positionsSynchronizer;

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

            _sphere.Render(ref _viewModel, _sphereRenderingParameters);
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
                    SwitchParallelTasks();
                    break;
            }
        }

        #endregion Event handlers

        #region Private methods

        /// <summary>
        /// Обменивает буферы компонент позиций частиц.
        /// </summary>
        private void SwapPositionsBuffers()
        {
            lock (_positionsSynchronizer)
            {
                float[] temp = _positions;
                _positions = _nextPositions;
                _nextPositions = temp;
                _sphereRenderingParameters.Positions = _positions;
            }
        }

        /// <summary>
        /// Выполняет обновление списка частиц.
        /// </summary>
        /// <param name="ct">Объект, распространяющий уведомление о том, что операцию следует отменить.</param>
        private void UpdateParticles(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                int count = _particles.Count;
                int capacity = _particles.Capacity;

                UpdateParticlesExternally(_particles);

                if (_particles.Capacity != capacity)
                {
                    AllocatePositionBuffers();
                }

                if (_particles.Count != count)
                {
                    _sphereRenderingParameters.InstanceCount = _particles.Count;
                }

                UpdatePositionBuffer();
                SwapPositionsBuffers();
            }
        }

        /// <summary>
        /// Выделяет память для буферов компонент положений частиц.
        /// </summary>
        private void AllocatePositionBuffers()
        {
            _positions = new float[_particles.Capacity * 3];
            _nextPositions = new float[_particles.Capacity * 3];
        }

        /// <summary>
        /// Обновляет текущий буфер компонент положений частиц.
        /// </summary>
        private void UpdatePositionBuffer()
        {
            for (int i = 0; i < _particles.Count; i++)
            {
                Particle particle = _particles[i];

                _positions[3 * i] = (float)(particle.Position.X / 10.0);
                _positions[3 * i + 1] = (float)(particle.Position.Y / 10.0);
                _positions[3 * i + 2] = (float)(particle.Position.Z / 10.0);
            }
        }

        /// <summary>
        /// Запускает или останавливает задачи, выполняющиеся параллельно визуализации.
        /// </summary>
        private void SwitchParallelTasks()
        {
            if (_cts == null || _cts.IsCancellationRequested)
            {
                _cts = new CancellationTokenSource();
                Task.Run(() => UpdateParticles(_cts.Token));
                Task.Run(() => UpdateStatistics(_cts.Token));
            }
            else
            {
                _cts.Cancel();
            }
        }

        #endregion Private methods
    }
}