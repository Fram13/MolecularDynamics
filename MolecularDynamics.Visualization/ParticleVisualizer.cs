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

        private ParticlesModel _particlesModel;
        private SpaceBorder _spaceBorder;
        
        private Matrix4 _viewModel;
        private CancellationTokenSource _cts;       

        #endregion Fields

        /// <summary>
        /// Обновляет характеристики визуализируемых частиц.
        /// </summary>
        public Action<List<Particle>> UpdateParticlesExternally
        {
            set => _particlesModel.UpdateExternally = value;
        }
        
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
            _particlesModel = new ParticlesModel(sphereRadius, particles);
            _spaceBorder = new SpaceBorder(spaceSize);

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
            _particlesModel.Dispose();
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

            _particlesModel.Initialize();
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

            _particlesModel.Render(ref _viewModel);
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
        /// Запускает или останавливает задачи, выполняющиеся параллельно визуализации.
        /// </summary>
        private void SwitchParallelTasks()
        {
            if (_cts == null || _cts.IsCancellationRequested)
            {
                _cts = new CancellationTokenSource();
                Task.Run(() => _particlesModel.Update(_cts.Token));
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