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

        /// <summary>
        /// Первая компонента вектора.
        /// </summary>
        public double X;

        /// <summary>
        /// Вторая компонента вектора.
        /// </summary>
        public double Y;

        /// <summary>
        /// Третья компонента вектора.
        /// </summary>
        public double Z;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Создает новый экземпляр <see cref="Vector3"/>.
        /// </summary>
        /// <param name="x">Первая компонента вектора.</param>
        /// <param name="y">Вторая компонента вектора.</param>
        /// <param name="z">Третья компонента вектора.</param>
        public Vector3(double x, double y, double z)
        {
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
            return Math.Sqrt(NormSquared());
        }

        /// <summary>
        /// Возвращает вторую норму вектора в квадрате.
        /// </summary>
        public double NormSquared()
        {
            return X * X + Y * Y + Z * Z;
        }

        /// <summary>
        /// Вычисляет орт вектора.
        /// </summary>
        public Vector3 Normalize()
        {
            return Divide(Norm());
        }

        /// <summary>
        /// Нормализует текущий вектор.
        /// </summary>
        public Vector3 NormalizeCurrent()
        {
            DivideToCurrent(Norm());
            return this;
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
        /// Выполняет сложение векторов и записывает результат в текущий вектор.
        /// </summary>
        /// <param name="other">Правый вектор.</param>
        public Vector3 AddToCurrent(Vector3 other)
        {
            this.X += other.X;
            this.Y += other.Y;
            this.Z += other.Z;

            return this;
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
        /// Выполняет вычитание векторов и записывает результат в текущий вектор.
        /// </summary>
        /// <param name="other">Правый вектор.</param>
        public Vector3 SubtractToCurrent(Vector3 other)
        {
            this.X -= other.X;
            this.Y -= other.Y;
            this.Z -= other.Z;

            return this;
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
        /// Выполняет умножение вектора на скаляр и записывает результат в текущий вектор.
        /// </summary>
        /// <param name="other">Правый вектор.</param>
        public Vector3 MultiplyToCurrent(double other)
        {
            X *= other;
            Y *= other;
            Z *= other;

            return this;
        }

        /// <summary>
        /// Выполняет деление вектора на скаляр.
        /// </summary>
        /// <param name="other">Скаляр.</param>
        public Vector3 Divide(double other)
        {
            return new Vector3(X / other, Y / other, Z / other);
        }

        /// <summary>
        /// Выполняет деление вектора на скаляр и записывает результат в текущий вектор.
        /// </summary>
        /// <param name="other">Правый вектор.</param>
        public Vector3 DivideToCurrent(double other)
        {
            X /= other;
            Y /= other;
            Z /= other;

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
        public static implicit operator Vector3((double X, double Y, double Z) tuple)
        {
            return new Vector3(tuple.X, tuple.Y, tuple.Z);
        }

        #endregion Operator overloads
    }
}
