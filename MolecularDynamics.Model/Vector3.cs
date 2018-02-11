using System;
using System.Text;

namespace MolecularDynamics.Model
{
    /// <summary>
    /// Представляет трехмерный вектор.
    /// </summary>
    public struct Vector3
    {
        #region Fields

        private double x;
        private double y;
        private double z;

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
                switch (index)
                {
                    case 0:
                        return x;

                    case 1:
                        return y;

                    case 2:
                        return z;

                    default:
                        throw new IndexOutOfRangeException("Индекс находится вне границ массива.");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;

                    case 1:
                        y = value;
                        break;

                    case 2:
                        z = value;
                        break;

                    default:
                        throw new IndexOutOfRangeException("Индекс находится вне границ массива.");
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
                return x;
            }
            set
            {
                x = value;
            }
        }

        /// <summary>
        /// Возвращает или задает вторую компоненту вектора.
        /// </summary>
        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        /// <summary>
        /// Возвращает или задает третью компоненту вектора.
        /// </summary>
        public double Z
        {
            get
            {
                return z;
            }
            set
            {
                z = value;
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
            this.x = x;
            this.y = y;
            this.z = z;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Возвращает строковое представление вектора.
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Math.Round(x, 3)).Append("; ").Append(Math.Round(y, 3)).Append("; ").Append(Math.Round(z, 3));

            return sb.ToString();
        }

        /// <summary>
        /// Возвращает вторую норму вектора.
        /// </summary>
        public double Norm()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        /// <summary>
        /// Выполняет сложение векторов.
        /// </summary>
        /// <param name="other">Правый вектор.</param>
        public Vector3 Add(Vector3 other)
        {
            return new Vector3(this.x + other.x, this.y + other.y, this.z + other.z);
        }

        /// <summary>
        /// Выполняет сложение векторов и записывает результат в текущий вектор.
        /// </summary>
        /// <param name="other">Правый вектор.</param>
        public Vector3 AddToCurrent(Vector3 other)
        {
            this.x += other.x;
            this.y += other.y;
            this.z += other.z;

            return this;
        }

        /// <summary>
        /// Выполняет вычитание векторов.
        /// </summary>
        /// <param name="other">Правый вектор.</param>
        public Vector3 Subtract(Vector3 other)
        {
            return new Vector3(this.x - other.x, this.y - other.y, this.z - other.z);
        }

        /// <summary>
        /// Выполняет вычитание векторов и записывает результат в текущий вектор.
        /// </summary>
        /// <param name="other">Правый вектор.</param>
        public Vector3 SubtractToCurrent(Vector3 other)
        {
            this.x -= other.x;
            this.y -= other.y;
            this.z -= other.z;

            return this;
        }

        /// <summary>
        /// Выполняет умножение вектора на скаляр.
        /// </summary>
        /// <param name="other">Скаляр.</param>
        public Vector3 Multiply(double other)
        {
            return new Vector3(x * other, y * other, z * other);
        }

        /// <summary>
        /// Выполняет умножение вектора на скаляр и записывает результат в текущий вектор.
        /// </summary>
        /// <param name="other">Правый вектор.</param>
        public Vector3 MultiplyToCurrent(double other)
        {
            x *= other;
            y *= other;
            z *= other;

            return this;
        }

        /// <summary>
        /// Выполняет деление вектора на скаляр.
        /// </summary>
        /// <param name="other">Скаляр.</param>
        public Vector3 Divide(double other)
        {
            return new Vector3(x / other, y / other, z / other);
        }

        /// <summary>
        /// Выполняет деление вектора на скаляр и записывает результат в текущий вектор.
        /// </summary>
        /// <param name="other">Правый вектор.</param>
        public Vector3 DivideToCurrent(double other)
        {
            x /= other;
            y /= other;
            z /= other;

            return this;
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
        public static implicit operator Vector3((double x, double y, double z) tuple)
        {
            return new Vector3(tuple.x, tuple.y, tuple.z);
        }

        #endregion Operator overloads
    }
}
