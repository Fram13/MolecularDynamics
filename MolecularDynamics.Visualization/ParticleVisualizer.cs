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

        private const string ParticleVertexShaderResourceName = "MolecularDynamics.Visualization.Shaders.ParticleVertexShader.glsl";
        private const string ParticleFragmentShaderResourceName = "MolecularDynamics.Visualization.Shaders.ParticleFragmentShader.glsl";
        private const string SpaceEdgeVertexShaderResourceName = "MolecularDynamics.Visualization.Shaders.SpaceEdgeVertexShader.glsl";
        private const string SpaceEdgeFragmentShaderResourceName = "MolecularDynamics.Visualization.Shaders.SpaceEdgeFragmentShader.glsl";
        private const int Faces = 10;

        private int _particleShaderProgram;
        private int _particleArrayBuffer;
        private int _particleVertexBuffer;
        private int _positionBuffer;
        private int _particleIndexBuffer;

        private int _spaceEdgeShaderProgram;
        private int _spaceEdgeArrayBuffer;
        private int _spaceEdgeVertexBuffer;
        private int _spaceEdgeIndexBuffer;
        private (float X, float Y, float Z) _spaceSize;
        private int _spaceEdgeTotalVertices;

        private double _sphereRadius;        
        private int _particleTotalVertices;
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
        /// <param name="spaceSize">Размеры пространства моделирования.</param>
        /// <param name="sphereRadius">Радиус частицы, нм.</param>
        public ParticleVisualizer(List<Particle> particles, TrajectoryIntegrator integrator, Model.Vector3 spaceSize, double sphereRadius)
        {
            _particles = particles;
            _integrator = integrator;
            _spaceSize.X = (float)spaceSize.X;
            _spaceSize.Y = (float)spaceSize.Y;
            _spaceSize.Z = (float)spaceSize.Z;

            _positions = new float[particles.Count * 3 * 2];
            _nextPositions = new float[particles.Count * 3 * 2];

            for (int i = 0; i < particles.Count; i++)
            {
                Particle particle = particles[i];

                _positions[3 * i] = (float)(particle.Position.X / 10.0);
                _positions[3 * i + 1] = (float)(particle.Position.Y / 10.0);
                _positions[3 * i + 2] = (float)(particle.Position.Z / 10.0);
            }

            _sphereRadius = sphereRadius;
            _modelView = Matrix4.CreateRotationX((float)(-Math.PI / 2.0));
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
            GL.DeleteProgram(_particleShaderProgram);
            GL.DeleteVertexArray(_particleArrayBuffer);
            GL.DeleteBuffer(_particleVertexBuffer);
            GL.DeleteBuffer(_positionBuffer);
            GL.DeleteBuffer(_particleIndexBuffer);

            GL.DeleteProgram(_spaceEdgeShaderProgram);
            GL.DeleteVertexArray(_spaceEdgeArrayBuffer);
            GL.DeleteBuffer(_spaceEdgeVertexBuffer);
            GL.DeleteBuffer(_spaceEdgeIndexBuffer);

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
            int c = 0;
            Random random = new Random();

            while (!ct.IsCancellationRequested)
            {
                if (c == 2500)
                {
                    var p = new Model.Atoms.Wolfram()
                    {
                        Position = (_spaceSize.X * 10.0 * random.NextDouble(), _spaceSize.Y * 10.0 * random.NextDouble(), (_spaceSize.Z - _sphereRadius) * 10.0),
                        Velocity = (0, 0, -0.4)
                    };

                    _particles.Add(p);
                    _integrator.Grid.AddParticle(p);
                    _nextPositions[_particles.Count] = (float)(p.Position.X / 10.0);
                    _nextPositions[_particles.Count + 1] = (float)(p.Position.Y / 10.0);
                    _nextPositions[_particles.Count + 2] = (float)(p.Position.Z / 10.0);

                    lock (_positionsSyncronizer)
                    {
                        _positions[_particles.Count] = _nextPositions[_particles.Count];
                        _positions[_particles.Count + 1] = _nextPositions[_particles.Count + 1];
                        _positions[_particles.Count + 2] = _nextPositions[_particles.Count + 2];
                    }

                    c = 0;
                }

                _integrator.NextStep();
                c++;

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

        private void InitializeParticleRendering()
        {
            Tuple<float[], int[]> sphere = GenerateSphere(_sphereRadius, Faces);
            _particleTotalVertices = sphere.Item2.Length;

            //Создание и заполнение буферов для отрисовки частиц
            string vertexShaderSource = GetEmbeddedResource(ParticleVertexShaderResourceName);
            string fragmentShaderSource = GetEmbeddedResource(ParticleFragmentShaderResourceName);
            _particleShaderProgram = CreateShaderProgram(vertexShaderSource, fragmentShaderSource);

            _particleArrayBuffer = GL.GenVertexArray();
            _positionBuffer = GL.GenBuffer();

            GL.BindVertexArray(_particleArrayBuffer);

            //Копирование в память ГП данных о вершинах (вектора вершин и нормалей совпадают)
            _particleVertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _particleVertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * sphere.Item1.Length, sphere.Item1, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            //Копирование в память ГП данных о порядке отрисовки вершин
            _particleIndexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _particleIndexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * sphere.Item2.Length, sphere.Item2, BufferUsageHint.StaticDraw);
        }

        private void InitializeSpaceEdgeRendering()
        {
            float[] vertices = new float[]
            {
                0.0f, 0.0f, 0.0f,
                _spaceSize.X, 0.0f, 0.0f,
                _spaceSize.X, _spaceSize.Y, 0.0f,
                0.0f, _spaceSize.Y, 0.0f,
                0.0f, 0.0f, _spaceSize.Z,
                _spaceSize.X, 0.0f, _spaceSize.Z,
                _spaceSize.X, _spaceSize.Y, _spaceSize.Z,
                0.0f, _spaceSize.Y, _spaceSize.Z,
            };

            int[] indicies = new int[]
            {
                0, 1, 3, 2, 4, 5, 7, 6,
                0, 3, 1, 2, 4, 7, 5, 6,
                0, 4, 1, 5, 3, 7, 2, 6
            };

            _spaceEdgeTotalVertices = indicies.Length;

            //Создание и заполнение буферов для отрисовки границ пространства
            string vertexShaderSource = GetEmbeddedResource(SpaceEdgeVertexShaderResourceName);
            string fragmentShaderSource = GetEmbeddedResource(SpaceEdgeFragmentShaderResourceName);
            _spaceEdgeShaderProgram = CreateShaderProgram(vertexShaderSource, fragmentShaderSource);

            _spaceEdgeArrayBuffer = GL.GenVertexArray();
            GL.BindVertexArray(_spaceEdgeArrayBuffer);

            //Копирование в память ГП данных о вершинах
            _spaceEdgeVertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _spaceEdgeVertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            //Копирование в память ГП данных о порядке отрисовки вершин
            _spaceEdgeIndexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _spaceEdgeIndexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indicies.Length, indicies, BufferUsageHint.StaticDraw);
        }

        private void RenderParticles()
        {
            GL.UseProgram(_particleShaderProgram);

            int modelViewLocation = GL.GetUniformLocation(_particleShaderProgram, "modelView");
            GL.UniformMatrix4(modelViewLocation, false, ref _modelView);

            Matrix3 inverseModevView = new Matrix3(_modelView.Inverted());
            int inverseModelViewLocation = GL.GetUniformLocation(_particleShaderProgram, "transposeInverseModelView");
            GL.UniformMatrix3(inverseModelViewLocation, true, ref inverseModevView);

            GL.BindVertexArray(_particleArrayBuffer);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _positionBuffer);

            lock (_positionsSyncronizer)
            {
                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * _positions.Length, _positions, BufferUsageHint.StreamDraw);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribDivisor(1, 1);

                GL.DrawElementsInstanced(PrimitiveType.Quads, _particleTotalVertices, DrawElementsType.UnsignedInt, (IntPtr)0, _positions.Length / 3);
            }
        }

        private void RenderSpaceEdges()
        {
            GL.UseProgram(_spaceEdgeShaderProgram);

            int modelViewLocation = GL.GetUniformLocation(_spaceEdgeShaderProgram, "modelView");
            GL.UniformMatrix4(modelViewLocation, false, ref _modelView);

            GL.BindVertexArray(_spaceEdgeArrayBuffer);
            GL.DrawElements(BeginMode.Lines, _spaceEdgeTotalVertices, DrawElementsType.UnsignedInt, 0);
        }

        #endregion Private methods

        #region Event handlers

        private void LoadHandler(Object sender, EventArgs e)
        {
            VSync = VSyncMode.On;
            Width = 800;
            Height = 800;
            Title = "Particle Visualizer";

            InitializeParticleRendering();
            InitializeSpaceEdgeRendering();            

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

            RenderParticles();
            RenderSpaceEdges();
            
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