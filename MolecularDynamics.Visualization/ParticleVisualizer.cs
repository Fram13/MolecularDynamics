using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using MolecularDynamics.Model;

namespace MolecularDynamics.Visualization
{
    /// <summary>
    /// Представляет визуализатор частиц.
    /// </summary>
    public class ParticleVisualizer : GameWindow 
    {
        #region Fields

        private const string VertexShaderResourceName = "MolecularDynamics.Visualization.Shaders.VertexShader.glsl";
        private const string FragmentShaderResourceName = "MolecularDynamics.Visualization.Shaders.FragmentShader.glsl";
        private const int Faces = 10;

        private int _shaderProgram;
        private int _arrayBuffer;
        private int _vertexBuffer;
        private int _positionBuffer;
        private int _indexBuffer;

        private double _sphereRadius;        
        private int _totalVertices;
        private Matrix4 _modelView;

        private Object _positionsSyncronizer;
        private float[] _positions;
        private float[] _nextPositions;

        private List<Particle> _particles;
        private TrajectoryIntegrator _integrator;
        private CancellationTokenSource _cts;

        #endregion Fields
        
        /// <summary>
        /// Создает новый экземпляр <see cref="ParticleVisualizer"/>.
        /// </summary>
        /// <param name="particles">Список визуализируемых частиц.</param>
        /// <param name="integrator">Интегратор траекторий движения частиц.</param>
        /// <param name="sphereRadius">Радиус частицы, нм.</param>
        public ParticleVisualizer(List<Particle> particles, TrajectoryIntegrator integrator, double sphereRadius)
        {
            _particles = particles;
            _integrator = integrator;

            _positions = new float[particles.Count * 3];
            _nextPositions = new float[particles.Count * 3];

            for (int i = 0; i < particles.Count; i++)
            {
                Particle particle = particles[i];

                _positions[3 * i] = (float)(particle.Position.X / 10.0);
                _positions[3 * i + 1] = (float)(particle.Position.Y / 10.0);
                _positions[3 * i + 2] = (float)(particle.Position.Z / 10.0);
            }

            _sphereRadius = sphereRadius;
            _modelView = Matrix4.Identity;
            _positionsSyncronizer = new Object();
            
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
            GL.DeleteProgram(_shaderProgram);
            GL.DeleteVertexArray(_arrayBuffer);
            GL.DeleteBuffer(_vertexBuffer);
            GL.DeleteBuffer(_positionBuffer);
            GL.DeleteBuffer(_indexBuffer);
            
            base.Dispose(manual);
        }

        #region Private methods
        
        private string GetEmbeddedResource(string resourceName)
        {
            using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)))
            {
                return reader.ReadToEnd();
            }
        }
        
        private int CreateShaderProgram(string vertexShaderSource, string fragmentShaderSource)
        {
            int shaderProgram = GL.CreateProgram();
            int vertexShader = CreateShader(ShaderType.VertexShader, vertexShaderSource);
            int fragmentShader = CreateShader(ShaderType.FragmentShader, fragmentShaderSource);

            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);

            string infoLog = GL.GetProgramInfoLog(shaderProgram);

            if (!String.IsNullOrEmpty(infoLog))
            {
                throw new ArgumentException("An error occurred during creation of shader program. " + infoLog);
            }

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return shaderProgram;
        }

        private int CreateShader(ShaderType type, string shaderSource)
        {
            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, shaderSource);
            GL.CompileShader(shader);

            string infoLog = GL.GetShaderInfoLog(shader);

            if (!String.IsNullOrEmpty(infoLog))
            {
                throw new ArgumentException("An error occurred during creation of shader. " + infoLog);
            }
            
            return shader;
        }

        private Tuple<float[], int[]> GenerateSphere(double radius, int faces)
        {
            Dictionary<Vector3d, int> vertices = new Dictionary<Vector3d, int>();
            List<int> indicies = new List<int>();

            void AddVertex(double theta, double phi)
            {
                double x = radius * Math.Sin(theta) * Math.Cos(phi);
                double y = radius * Math.Sin(theta) * Math.Sin(phi);
                double z = radius * Math.Cos(theta);
                            
                Vector3d vertex = new Vector3d(x, y, z);

                if (!vertices.ContainsKey(vertex))
                {
                    vertices.Add(vertex, vertices.Count);
                }

                indicies.Add(vertices[vertex]);
            }
            
            double max = 2.0 * Math.PI;
            double step = max / faces;

            for (double theta = 0.0; theta < Math.PI; theta += step)
            {
                for (double phi = 0.0; phi < max; phi += step)
                {
                    AddVertex(theta, phi);
                    AddVertex(theta, phi + step);
                    AddVertex(theta + step, phi + step);
                    AddVertex(theta + step, phi);
                }
            }
            
            float[] verticesComponents = new float[vertices.Count * 3];
            int counter = 0;

            foreach (Vector3d vertex in vertices.Keys)
            {
                verticesComponents[counter] = (float)vertex.X;
                verticesComponents[counter + 1] = (float)vertex.Y;
                verticesComponents[counter + 2] = (float)vertex.Z;
                counter += 3;
            }

            return Tuple.Create(verticesComponents, indicies.ToArray());
        }

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
            while (!ct.IsCancellationRequested)
            {
                _integrator.NextStep();

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

            string vertexShaderSource = GetEmbeddedResource(VertexShaderResourceName);
            string fragmentShaderSource = GetEmbeddedResource(FragmentShaderResourceName);
            
            _shaderProgram = CreateShaderProgram(vertexShaderSource, fragmentShaderSource);
            
            Tuple<float[], int[]> sphere = GenerateSphere(_sphereRadius, Faces);
            _totalVertices = sphere.Item2.Length;

            _arrayBuffer = GL.GenVertexArray();
            _positionBuffer = GL.GenBuffer();
           
            GL.BindVertexArray(_arrayBuffer);
            
            //Копирование в память ГП данных о вершинах (вектора вершин и нормалей совпадают)
            _vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * sphere.Item1.Length, sphere.Item1, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);          
            
            //Копирование в память ГП данных о порядке отрисовки вершин
            _indexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * sphere.Item2.Length, sphere.Item2, BufferUsageHint.StaticDraw);
            
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
            
            GL.UseProgram(_shaderProgram);
            
            int modelViewLocation = GL.GetUniformLocation(_shaderProgram, "modelView");
            GL.UniformMatrix4(modelViewLocation, false, ref _modelView);
            
            Matrix3 inverseModevView = new Matrix3(_modelView.Inverted());
            int inverseModelViewLocation = GL.GetUniformLocation(_shaderProgram, "transposeInverseModelView");
            GL.UniformMatrix3(inverseModelViewLocation, true, ref inverseModevView);
            
            GL.BindVertexArray(_arrayBuffer);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, _positionBuffer);

            lock (_positionsSyncronizer)
            {
                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * _positions.Length, _positions, BufferUsageHint.StreamDraw);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribDivisor(1, 1);

                GL.DrawElementsInstanced(PrimitiveType.Quads, _totalVertices, DrawElementsType.UnsignedInt, (IntPtr)0, _positions.Length / 3); 
            }
            
            GL.BindVertexArray(0);
            
            SwapBuffers();
        }

        private void MouseMoveHandler(Object sender, MouseMoveEventArgs e)
        {
            if (e.Mouse[MouseButton.Left])
            {
                if (e.XDelta != 0)
                {
                    _modelView = _modelView * Matrix4.CreateRotationY((float)(Math.Sign(e.XDelta) * -2.0 * Math.PI / 180.0));
                }
                
                if (e.YDelta != 0)
                {
                    _modelView = _modelView * Matrix4.CreateRotationX((float)(Math.Sign(e.YDelta) * -2.0 * Math.PI / 180.0));
                }
            }
        }

        private void MouseWheelHandler(Object sender, MouseWheelEventArgs e)
        {
            if (e.Delta != 0)
            {
                _modelView = _modelView * Matrix4.CreateScale(e.Delta > 0 ? 1.05f : 0.95f);
            }
        }

        private void KeyDownHandler(Object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    _modelView = _modelView * Matrix4.CreateTranslation(-0.05f, 0.0f, 0.0f);
                    break;

                case Key.Right:
                    _modelView = _modelView * Matrix4.CreateTranslation(0.05f, 0.0f, 0.0f);
                    break;

                case Key.Up:
                    _modelView = _modelView * Matrix4.CreateTranslation(0.0f, 0.05f, 0.0f);
                    break;

                case Key.Down:
                    _modelView = _modelView * Matrix4.CreateTranslation(0.0f, -0.05f, 0.0f);
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