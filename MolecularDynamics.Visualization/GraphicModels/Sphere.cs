using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace MolecularDynamics.Visualization.GraphicModels
{
    /// <summary>
    /// Представляет модель сферы.
    /// </summary>
    public class Sphere : GraphicModel
    {
        private const string VertexShaderResourceName = "MolecularDynamics.Visualization.Shaders.SphereVertexShader.glsl";
        private const string FragmentShaderResourceName = "MolecularDynamics.Visualization.Shaders.SphereFragmentShader.glsl";
        private const int Layers = 10;

        private double sphereRadius;        

        private int vertexCount;
        private int shaderProgram;
        private int arrayBuffer;
        private int positionBuffer;
        private int vertexBuffer;
        private int indexBuffer;

        /// <summary>
        /// Создает новый экземпляр <see cref="Sphere"/>.
        /// </summary>
        /// <param name="sphereRadius">Радиус сферы.</param>
        public Sphere(double sphereRadius)
        {
            this.sphereRadius = sphereRadius;
        }

        /// <summary>
        /// Освобождает неуправляемые ресурсы.
        /// </summary>
        public override void Dispose()
        {
            GL.DeleteProgram(shaderProgram);
            GL.DeleteVertexArray(arrayBuffer);
            GL.DeleteBuffer(positionBuffer);
            GL.DeleteBuffer(vertexBuffer);
            GL.DeleteBuffer(indexBuffer);
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
            positionBuffer = GL.GenBuffer();

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
        }

        /// <summary>
        /// Отрисовывает графическую модель.
        /// <param name="viewModel">Видовая матрица визуализатора.</param>
        /// <param name="parameters">Дополнительные параметры для отрисовки модели.</param>
        /// </summary>
        public override void Render(ref Matrix4 viewModel, GraphicModelRenderingParameters parameters = null)
        {
            RenderingParameters rp = (RenderingParameters)parameters;

            GL.UseProgram(shaderProgram);

            int viewModelLocation = GL.GetUniformLocation(shaderProgram, "viewModel");
            GL.UniformMatrix4(viewModelLocation, false, ref viewModel);

            Matrix3 inverseViewModel = new Matrix3(viewModel.Inverted());
            int inverseViewModelLocation = GL.GetUniformLocation(shaderProgram, "transposeInverseViewModel");
            GL.UniformMatrix3(inverseViewModelLocation, true, ref inverseViewModel);

            GL.BindVertexArray(arrayBuffer);

            GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);

            lock (rp.PositionSynchronizer)
            {
                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * rp.InstanceCount * 3, rp.Positions, BufferUsageHint.StreamDraw); 
            }

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribDivisor(1, 1);

            GL.DrawElementsInstanced(PrimitiveType.Quads, vertexCount, DrawElementsType.UnsignedInt, (IntPtr)0, rp.InstanceCount);
        }

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
        /// Представляет параметры отрисовки сферы.
        /// </summary>
        public class RenderingParameters : GraphicModelRenderingParameters
        {
            /// <summary>
            /// Массив компонент положений сфер.
            /// </summary>
            public float[] Positions { get; set; }

            /// <summary>
            /// Количество сфер.
            /// </summary>
            public int InstanceCount { get; set; }

            /// <summary>
            /// Объект синхронизации массива компонент положений сфер.
            /// </summary>
            public Object PositionSynchronizer { get; set; }
        }
    }
}
