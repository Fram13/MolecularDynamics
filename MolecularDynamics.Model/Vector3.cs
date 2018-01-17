using System;
using System.Text;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет трехмерный вектор.
    /// </summary>
    public class Vector3
    {
        #region Fields

        private double[] raw;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Возвращает или задает указанную компоненту вектора.
        /// </summary>
        /// <param name="index">Индекс компоненты.</param>
        /// <exception cref="IndexOutOfRangeException">Индекс находится вне границ массива.</exception>
        public double this[int index]
        {
            get
            {
                return raw[index];
            }
            set
            {
                raw[index] = value;
            }
        }

        /// <summary>
        /// Возвращает или задает первую компоненту вектора.
        /// </summary>
        public double X
        {
            get
            {
                return raw[0];
            }
            set
            {
                raw[0] = value;
            }
        }

        /// <summary>
        /// Возвращает или задает вторую компоненту вектора.
        /// </summary>
        public double Y
        {
            get
            {
                return raw[1];
            }
            set
            {
                raw[1] = value;
            }
        }

        /// <summary>
        /// Возвращает или задает третью компоненту вектора.
        /// </summary>
        public double Z
        {
            get
            {
                return raw[2];
            }
            set
            {
                raw[2] = value;
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Создает новый экземпляр <see cref="Vector3"/>.
        /// </summary>
        public Vector3()
        {
            raw = new double[3];
        }

        /// <summary>
        /// Создает новый экземпляр <see cref="Vector3"/>.
        /// </summary>
        /// <param name="x">Первая компонента вектора.</param>
        /// <param name="y">Вторая компонента вектора.</param>
        /// <param name="z">Третья компонента вектора.</param>
        public Vector3(double x, double y, double z)
        {
            raw = new double[3];

            X = x;
            Y = y;
            Z = z;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Возвращает строковое представление вектора.
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Math.Round(X, 3)).Append("; ").Append(Math.Round(Y, 3)).Append("; ").Append(Math.Round(Z, 3));

            return sb.ToString();
        }

        /// <summary>
        /// Возвращает вторую норму вектора.
        /// </summary>
        public double Norm()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        /// <summary>
        /// Выполняет сложение векторов.
        /// </summary>
        /// <param name="other">Правый вектор.</param>
        public Vector3 Add(Vector3 other)
        {
            return new Vector3(this.X + other.X, this.Y + other.Y, this.Z + other.Z);
        }

        /// <summary>
        /// Выполняет вычитание векторов.
        /// </summary>
        /// <param name="other">Правый вектор.</param>
        public Vector3 Subtract(Vector3 other)
        {
            return new Vector3(this.X - other.X, this.Y - other.Y, this.Z - other.Z);
        }

        /// <summary>
        /// Выполняет умножение вектора на скаляр.
        /// </summary>
        /// <param name="other">Скаляр.</param>
        public Vector3 Multiply(double other)
        {
            return new Vector3(X * other, Y * other, Z * other);
        }

        /// <summary>
        /// Выполняет деление вектора на скаляр.
        /// </summary>
        /// <param name="other">Скаляр.</param>
        public Vector3 Divide(double other)
        {
            return new Vector3(X / other, Y / other, Z / other);
        }

        #endregion Methods

        #region Operator overloads

        /// <summary>
        /// Выполняет сложение векторов.
        /// </summary>
        public static Vector3 operator +(Vector3 left, Vector3 right)
        {
            return left.Add(right);
        }

        /// <summary>
        /// Выполняет вычитание векторов.
        /// </summary>
        public static Vector3 operator -(Vector3 left, Vector3 right)
        {
            return left.Subtract(right);
        }

        /// <summary>
        /// Выполняет умножение вектора на скаляр.
        /// </summary>
        public static Vector3 operator *(Vector3 left, double right)
        {
            return left.Multiply(right);
        }

        /// <summary>
        /// Выполняет умножение вектора на скаляр.
        /// </summary>
        public static Vector3 operator *(double left, Vector3 right)
        {
            return right.Multiply(left);
        }

        /// <summary>
        /// Выполняет деление вектора на скаляр.
        /// </summary>
        public static Vector3 operator /(Vector3 left, double right)
        {
            return left.Divide(right);
        }

        /// <summary>
        /// Создает новый экземпляр <see cref="Vector3"/>.
        /// </summary>
        /// <param name="tuple">Кортеж компонент вектора.</param>
        public static implicit operator Vector3((double X, double Y, double Z) tuple)
        {
            return new Vector3(tuple.X, tuple.Y, tuple.Z);
        }

        #endregion Operator overloads
    }
}
