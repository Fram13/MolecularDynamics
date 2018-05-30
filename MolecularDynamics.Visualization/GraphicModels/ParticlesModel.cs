using System;
using System.Collections.Generic;
using System.Threading;
using MolecularDynamics.Model;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace MolecularDynamics.Visualization.GraphicModels
{
    /// <summary>
    /// Представляет графическую модель частиц.
    /// </summary>
    public class ParticlesModel : GraphicModel
    {
        private const string VertexShaderResourceName = "MolecularDynamics.Visualization.Shaders.SphereVertexShader.glsl";
        private const string FragmentShaderResourceName = "MolecularDynamics.Visualization.Shaders.SphereFragmentShader.glsl";
        private const int Layers = 10;
        private const double MaxEnergy = 100.0;

        private double sphereRadius;        

        private int vertexCount;
        private int shaderProgram;
        private int arrayBuffer;
        private int vertexBuffer;
        private int indexBuffer;
        private int positionBuffer;
        private int colorBuffer;

        private float[] positions;
        private float[] nextPositions;
        private float[] colors;
        private float[] nextColors;
        private Object synchronizer;

        private List<Particle> particles;

        /// <summary>
        /// Обновляет характеристики визуализируемых частиц.
        /// </summary>
        public Action<List<Particle>> UpdateExternally { get; set; }

        /// <summary>
        /// Создает новый экземпляр <see cref="ParticlesModel"/>.
        /// </summary>
        /// <param name="sphereRadius">Радиус сферы.</param>
        /// <param name="particles">Список визуализируемых частиц.</param>
        public ParticlesModel(double sphereRadius, List<Particle> particles)
        {
            this.sphereRadius = sphereRadius;
            this.particles = particles;
        }

        /// <summary>
        /// Освобождает неуправляемые ресурсы.
        /// </summary>
        public override void Dispose()
        {
            GL.DeleteProgram(shaderProgram);
            GL.DeleteVertexArray(arrayBuffer);
            GL.DeleteBuffer(vertexBuffer);
            GL.DeleteBuffer(indexBuffer);
            GL.DeleteBuffer(positionBuffer);
            GL.DeleteBuffer(colorBuffer);
        }

        /// <summary>
        /// Инициализирует графическую модель.
        /// </summary>
        public override void Initialize()
        {
            var (verticesComponents, indicies) = GenerateSphere();
            vertexCount = indicies.Length;

            //Создание и заполнение буферов для отрисовки частиц
            string vertexShaderSource = GetEmbeddedResource(VertexShaderResourceName);
            string fragmentShaderSource = GetEmbeddedResource(FragmentShaderResourceName);
            shaderProgram = CreateShaderProgram(vertexShaderSource, fragmentShaderSource);

            arrayBuffer = GL.GenVertexArray();
            GL.BindVertexArray(arrayBuffer);

            //Копирование в память ГП данных о вершинах (вектора вершин и нормалей совпадают)
            vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * verticesComponents.Length, verticesComponents, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            //Копирование в память ГП данных о порядке отрисовки вершин
            indexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indicies.Length, indicies, BufferUsageHint.StaticDraw);
            
            positionBuffer = GL.GenBuffer();
            colorBuffer = GL.GenBuffer();
        }

        /// <summary>
        /// Отрисовывает частицы.
        /// <param name="viewModel">Видовая матрица визуализатора.</param>
        /// </summary>
        public override void Render(ref Matrix4 viewModel)
        {
            GL.UseProgram(shaderProgram);

            int viewModelLocation = GL.GetUniformLocation(shaderProgram, "viewModel");
            GL.UniformMatrix4(viewModelLocation, false, ref viewModel);

            Matrix3 inverseViewModel = new Matrix3(viewModel.Inverted());
            int inverseViewModelLocation = GL.GetUniformLocation(shaderProgram, "transposeInverseViewModel");
            GL.UniformMatrix3(inverseViewModelLocation, true, ref inverseViewModel);

            GL.BindVertexArray(arrayBuffer);

            lock (synchronizer)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * particles.Count * 3, positions, BufferUsageHint.StreamDraw); 
                
                GL.BindBuffer(BufferTarget.ArrayBuffer, colorBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * particles.Count * 3, colors, BufferUsageHint.StreamDraw);
            }

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribDivisor(1, 1);
            
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribDivisor(2, 1);

            GL.DrawElementsInstanced(PrimitiveType.Quads, vertexCount, DrawElementsType.UnsignedInt, (IntPtr)0, particles.Count);
        }

        /// <summary>
        /// Генерирует сферу, представляющую частицу.
        /// </summary>
        private (float[] VerticesComponents, int[] Indicies) GenerateSphere()
        {
            Dictionary<Vector3d, int> vertices = new Dictionary<Vector3d, int>();
            List<int> indicies = new List<int>();

            void AddVertex(double theta, double phi)
            {
                double x = sphereRadius * Math.Sin(theta) * Math.Cos(phi);
                double y = sphereRadius * Math.Sin(theta) * Math.Sin(phi);
                double z = sphereRadius * Math.Cos(theta);

                Vector3d vertex = new Vector3d(x, y, z);

                if (!vertices.ContainsKey(vertex))
                {
                    vertices.Add(vertex, vertices.Count);
                }

                indicies.Add(vertices[vertex]);
            }

            double max = 2.0 * Math.PI;
            double step = max / Layers;

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

            return (verticesComponents, indicies.ToArray());
        }

        /// <summary>
        /// Выделяет память для хранения позиций и цветов частиц. 
        /// </summary>
        private void AllocateBuffers()
        {
            positions = new float[particles.Capacity * 3];
            nextPositions = new float[particles.Capacity * 3];
            colors = new float[particles.Capacity * 3];
            nextColors = new float[particles.Capacity * 3];
        }

        /// <summary>
        /// Обновляет буферы позиций и цветов из списка частиц.
        /// </summary>
        private void UpdateBuffers()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                Particle particle = particles[i];

                positions[3 * i] = (float)(particle.Position.X / 10.0);
                positions[3 * i + 1] = (float)(particle.Position.Y / 10.0);
                positions[3 * i + 2] = (float)(particle.Position.Z / 10.0);

                float intensity = (float)(particle.Energy() / MaxEnergy);

                colors[3 * i] = intensity;
                colors[3 * i + 1] = 0.0f;
                colors[3 * i + 2] = 1.0f - intensity;
            }
        }

        /// <summary>
        /// Обменивает буферы позиций и цветов.
        /// </summary>
        private void SwapBuffers()
        {
            lock (synchronizer)
            {
                float[] temp = positions;
                positions = nextPositions;
                nextPositions = temp;

                temp = colors;
                colors = nextColors;
                nextColors = temp;
            }
        }

        /// <summary>
        /// Обновляет характеристики частиц.
        /// </summary>
        /// <param name="ct">Объект, сигнализирующий о необходимости прекратить выполнение операции.</param>
        public void Update(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                int capacity = particles.Capacity;

                UpdateExternally(particles);

                if (particles.Capacity != capacity)
                {
                    AllocateBuffers();
                }

                UpdateBuffers();
                SwapBuffers();
            }
        }
    }
}
