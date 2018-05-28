using System;
using System.IO;
using System.Reflection;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace MolecularDynamics.Visualization
{
    /// <summary>
    /// Представляет базовый класс графической модели.
    /// </summary>
    public abstract class GraphicModel : IDisposable
    {
        /// <summary>
        /// Инициализирует графическую модель.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Отрисовывает графическую модель.
        /// <param name="viewModel">Видовая матрица визуализатора.</param>
        /// <param name="parameters">Дополнительные параметры для отрисовки модели.</param>
        /// </summary>
        public abstract void Render(ref Matrix4 viewModel, GraphicModelRenderingParameters parameters = null);

        /// <summary>
        /// Освобождает неуправляемые ресурсы.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Считывает ресурс, встроенный в сборку.
        /// </summary>
        /// <param name="resourceName">Имя встроенного ресурса.</param>
        /// <returns></returns>
        protected string GetEmbeddedResource(string resourceName)
        {
            using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Создает шейдер.
        /// </summary>
        /// <param name="type">Тип шейдера.</param>
        /// <param name="shaderSource">Исходный код шейдера.</param>
        /// <returns></returns>
        protected int CreateShader(ShaderType type, string shaderSource)
        {
            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, shaderSource);
            GL.CompileShader(shader);

            string infoLog = GL.GetShaderInfoLog(shader);

            if (!String.IsNullOrEmpty(infoLog))
            {
                throw new ArgumentException("Произошла ошибка во время создания шейдера. " + infoLog);
            }

            return shader;
        }

        /// <summary>
        /// Создает шейдерную программу, исполняемую на графическом процессоре.
        /// </summary>
        /// <param name="vertexShaderSource">Исходный код вершинного шейдера.</param>
        /// <param name="fragmentShaderSource">Исходный код фрагментного шейдера.</param>
        /// <returns></returns>
        protected int CreateShaderProgram(string vertexShaderSource, string fragmentShaderSource)
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
                throw new ArgumentException("Произошла ошибка во время создания шейдерной программы. " + infoLog);
            }

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return shaderProgram;
        }
    }
}
