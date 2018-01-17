using System;
using System.Text;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет трехмерный вектор.
    /// </summary>
    public unsafe struct Vector3
    {
        #region Fields

        private fixed double raw[3];

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
                if (index < 0 || index >= 3)
                {
                    throw new IndexOutOfRangeException("Индекс находится вне границ массива.");
                }

                fixed (double* ptr = raw)
                {
                    return ptr[index];
                }
            }
            set
            {
                if (index < 0 || index >= 3)
                {
                    throw new IndexOutOfRangeException("Индекс находится вне границ массива.");
                }

                fixed (double* ptr = raw)
                {
                    ptr[index] = value;
                }
            }
        }

        /// <summary>
        /// Возвращает или задает первую компоненту вектора.
        /// </summary>
        public double X
        {
            get
            {
                return this[0];
            }
            set
            {
                this[0] = value;
            }
        }

        /// <summary>
        /// Возвращает или задает вторую компоненту вектора.
        /// </summary>
        public double Y
        {
            get
            {
                return this[1];
            }
            set
            {
                this[1] = value;
            }
        }

        /// <summary>
        /// Возвращает или задает третью компоненту вектора.
        /// </summary>
        public double Z
        {
            get
            {
                return this[2];
            }
            set
            {
                this[2] = value;
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Создает новый экземпляр <see cref="Vector3"/>.
        /// </summary>
        /// <param name="x">Первая компонента вектора.</param>
        /// <param name="y">Вторая компонента вектора.</param>
        /// <param name="z">Третья компонента вектора.</param>
        public Vector3(double x, double y, double z)
        {
            fixed (double* ptr = raw)
            {
                ptr[0] = x;
                ptr[1] = y;
                ptr[2] = z;
            }
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
            fixed (double* ptr = raw)
            {
                return Math.Sqrt(ptr[0] * ptr[0] + ptr[1] * ptr[1] + ptr[2] * ptr[2]);
            }
        }

        /// <summary>
        /// Выполняет сложение векторов.
        /// </summary>
        /// <param name="other">Правый вектор.</param>
        public Vector3 Add(Vector3 other)
        {
            fixed (double* left = raw)
            {
                return new Vector3(left[0] + other.raw[0], left[1] + other.raw[1], left[2] + other.raw[2]);
            }
        }

        /// <summary>
        /// Выполняет вычитание векторов.
        /// </summary>
        /// <param name="other">Правый вектор.</param>
        public Vector3 Subtract(Vector3 other)
        {
            fixed (double* left = raw)
            {
                return new Vector3(left[0] - other.raw[0], left[1] - other.raw[1], left[2] - other.raw[2]);
            }
        }

        /// <summary>
        /// Выполняет умножение вектора на скаляр.
        /// </summary>
        /// <param name="other">Скаляр.</param>
        public Vector3 Multiply(double other)
        {
            fixed (double* left = raw)
            {
                return new Vector3(left[0] * other, left[1] * other, left[2] * other);
            }
        }

        /// <summary>
        /// Выполняет деление вектора на скаляр.
        /// </summary>
        /// <param name="other">Скаляр.</param>
        public Vector3 Divide(double other)
        {
            fixed (double* left = raw)
            {
                return new Vector3(left[0] / other, left[1] / other, left[2] / other);
            }
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
