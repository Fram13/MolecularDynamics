using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace MolecularDynamics.Visualization.GraphicModels
{
    /// <summary>
    /// Представляет модель границ пространства.
    /// </summary>
    public class SpaceBorder : GraphicModel
    {
        private const string VertexShaderResourceName = "MolecularDynamics.Visualization.Shaders.SpaceBorderVertexShader.glsl";
        private const string FragmentShaderResourceName = "MolecularDynamics.Visualization.Shaders.SpaceBorderFragmentShader.glsl";

        private float x;
        private float y;
        private float z;

        private int vertexCount;
        private int shaderProgram;
        private int arrayBuffer;
        private int vertexBuffer;
        private int indexBuffer;

        /// <summary>
        /// Создает новый экземпляр <see cref="SpaceBorder"/>.
        /// </summary>
        /// <param name="spaceSize">Размер пространства, А.</param>
        public SpaceBorder(Model.Vector3 spaceSize)
        {
            x = (float)(spaceSize.X / 10.0);
            y = (float)(spaceSize.Y / 10.0);
            z = (float)(spaceSize.Z / 10.0);
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
        }

        /// <summary>
        /// Инициализирует графическую модель.
        /// </summary>
        public override void Initialize()
        {
            float[] vertices = new float[]
            {
                0.0f, 0.0f, 0.0f,
                x, 0.0f, 0.0f,
                x, y, 0.0f,
                0.0f, y, 0.0f,
                0.0f, 0.0f, z,
                x, 0.0f, z,
                x, y, z,
                0.0f, y, z,
            };

            int[] indicies = new int[]
            {
                0, 1, 3, 2, 4, 5, 7, 6,
                0, 3, 1, 2, 4, 7, 5, 6,
                0, 4, 1, 5, 3, 7, 2, 6
            };

            vertexCount = indicies.Length;

            //Создание и заполнение буферов для отрисовки границ пространства
            string vertexShaderSource = GetEmbeddedResource(VertexShaderResourceName);
            string fragmentShaderSource = GetEmbeddedResource(FragmentShaderResourceName);
            shaderProgram = CreateShaderProgram(vertexShaderSource, fragmentShaderSource);

            arrayBuffer = GL.GenVertexArray();
            GL.BindVertexArray(arrayBuffer);

            //Копирование в память ГП данных о вершинах
            vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            //Копирование в память ГП данных о порядке отрисовки вершин
            indexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indicies.Length, indicies, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        /// <summary>
        /// Отрисовывает графическую модель.
        /// <param name="viewModel">Видовая матрица визуализатора.</param>
        /// <param name="parameters">Дополнительные параметры для отрисовки модели.</param>
        /// </summary>
        public override void Render(ref Matrix4 viewModel, params Object[] parameters)
        {
            GL.UseProgram(shaderProgram);

            int viewModelLocation = GL.GetUniformLocation(shaderProgram, "viewModel");
            GL.UniformMatrix4(viewModelLocation, false, ref viewModel);

            GL.BindVertexArray(arrayBuffer);
            GL.DrawElements(BeginMode.Lines, vertexCount, DrawElementsType.UnsignedInt, 0);
        }
    }
}
