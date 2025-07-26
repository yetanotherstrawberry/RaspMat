using System;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RaspMat.Models
{
    /// <summary>
    /// A representation of <see cref="Fraction"/>s.
    /// </summary>
    [Serializable]
    internal class Matrix : ISerializable
    {

        /// <summary>
        /// The character used to separate columns.
        /// </summary>
        private const char COLUMN_SEPARATOR = '\t';

        #region Properties
        /// <summary>
        /// <see cref="Fraction"/>s of this <see cref="Matrix"/>.
        /// </summary>
        private Fraction[][] FractionMatrix { get; }

        /// <summary>            
        /// The total number of rows.
        /// </summary>
        public int Rows => FractionMatrix.Length;

        /// <summary>
        /// The total number of columns.
        /// </summary>
        public int Columns => FractionMatrix.FirstOrDefault()?.Length ?? 0;

        /// <summary>
        /// Returns <see langword="true"/> is <see langword="this"/> <see cref="Matrix"/> is square.
        /// </summary>
        public bool IsSquare => Rows == Columns;

        /// <summary>
        /// Gets the specified rows.
        /// </summary>
        /// <param name="row">Zero-based index.</param>
        /// <returns>An array of <see cref="Fraction"/>s.</returns>
        private Fraction[] this[int row]
        {
            get => FractionMatrix[row];
            set => FractionMatrix[row] = value;
        }

        /// <summary>
        /// Gets the specified cell.
        /// </summary>
        /// <param name="row">Zero-based index of the row.</param>
        /// <param name="column">Zero-based index of the column.</param>
        /// <returns>A <see cref="Fraction"/> that is at the given position.</returns>
        public Fraction this[int row, int column]
        {
            get => this[row][column];
            private set => this[row][column] = value;
        }

        #endregion Properties
        #region Constructors

        public Matrix(int rows, int columns, Func<int, int, Fraction> values)
        {
            if ((rows == 0 || columns == 0) && rows != columns) throw new ArgumentOutOfRangeException(nameof(columns));

            FractionMatrix = new Fraction[rows][];

            Parallel.For(0, Rows, row =>
            {
                this[row] = new Fraction[columns];
                Parallel.For(0, this[row].Length, column => this[row][column] = values(row, column));
            });
        }

        protected Matrix(Fraction[][] fractionMatrix)
        {
            FractionMatrix = fractionMatrix ?? throw new ArgumentNullException(nameof(fractionMatrix));
        }

        protected internal Matrix(SerializationInfo info, StreamingContext context) : this(info.GetValue(nameof(FractionMatrix), typeof(Fraction[][])) as Fraction[][]) { }

        public Matrix(int rows, int columns) : this(Enumerable.Range(0, rows).Select(row => new Fraction[columns]).ToArray()) { }

        #endregion Constructors
        #region StaticMethods

        /// <summary>
        /// Creates a <see cref="Matrix"/> with ones inside and zeros outside its diagonal.
        /// </summary>
        /// <param name="size">Size of a square <see cref="Matrix"/>.</param>
        /// <returns>A <see langword="new"/> <see cref="Matrix"/>.</returns>
        public static Matrix Identity(int size)
        {
            var newMatrix = new Matrix(size, size);
            for (var cell = 0; cell < size; cell++) newMatrix[cell, cell] = 1;
            return newMatrix;
        }

        /// <summary>
        /// Creates a <see cref="Matrix"/> that can be used to swap rows or columns of another <see cref="Matrix"/> by multiplication.
        /// </summary>
        /// <param name="size">Size of a square multiplication <see cref="Matrix"/>.</param>
        /// <param name="first">Zero-based index of the first row to swap.</param>
        /// <param name="second">Zero-based index of the second row to swap.</param>
        /// <returns>A <see langword="new"/> square <see cref="Matrix"/> of the specified <paramref name="size"/>.</returns>
        public static Matrix SwapMatrix(int size, int first, int second)
        {
            var newMatrix = Identity(size);

            newMatrix[first, first] = 0;
            newMatrix[second, second] = 0;

            newMatrix[first, second] = 1;
            newMatrix[second, first] = 1;

            return newMatrix;
        }

        public static Matrix AddToRowMatrix(Matrix matrix, int destination, int source, Fraction srcMultiplication)
        {
            var ret = Identity(matrix.Rows);

            ret[destination, source] = srcMultiplication;

            return ret;
        }

        /// <summary>
        /// Creates a <see cref="Matrix"/> that can be used (by multiplication) to scale a row or column of another <see cref="Matrix"/>.
        /// </summary>
        /// <param name="diagonal">Number of rows and columns.</param>
        /// <param name="index">Index to multiply by the <paramref name="scalar"/>.</param>
        /// <param name="scalar">The multiplicand.</param>
        /// <returns>A <see langword="new"/> <see cref="Matrix"/>.</returns>
        public static Matrix MultiplicationMatrix(int diagonal, int index, Fraction scalar)
        {
            var newMatrix = Identity(diagonal);
            newMatrix[index, index] = scalar;
            return newMatrix;
        }

        public static Matrix operator *(Fraction scalar, Matrix matrix)
        {
            var newValue = new Matrix(matrix.Rows, matrix.Columns);

            Parallel.For(0, matrix.Rows, row =>
            {
                Parallel.For(0, matrix.Columns, column =>
                {
                    newValue[row, column] = matrix[row, column] * scalar;
                });
            });

            return newValue;
        }

        public static Matrix operator *(Matrix matrix, Fraction scale) => scale * matrix;

        public static Matrix operator *(Matrix left, Matrix right)
        {
            if (left.Columns != right.Rows) throw new InvalidOperationException(nameof(left.Columns));

            var ret = new Matrix(left.Rows, right.Columns);

            Parallel.For(0, ret.Rows, row =>
            {
                Parallel.For(0, ret.Columns, column =>
                {
                    ret[row, column] = Enumerable.Range(0, left.Columns).Select(cell => left[row, cell] * right[cell, column]).Aggregate((x, y) => x + y);
                });
            });

            return ret;
        }

        /// <summary>
        /// Creates a <see cref="Matrix"/> that has its cells populated with data from the <paramref name="dataTable"/>.
        /// </summary>
        /// <param name="dataTable">Provides data for the <see cref="Matrix"/>.</param>
        /// <returns>A <see langword="new"/> <see cref="Matrix"/> based on the <paramref name="dataTable"/>.</returns>
        public static Matrix Parse(DataTable dataTable)
        {
            return new Matrix(dataTable.Rows.Count, dataTable.Columns.Count, (row, column) => Fraction.Parse(dataTable.Rows[row][column].ToString()));
        }

        #endregion StaticMethods
        #region Methods

        /// <summary>
        /// Copies selected half of <see langword="this"/> to a <see langword="new"/> <see cref="Matrix"/>.
        /// </summary>
        /// <param name="removeLeft">Indicated which half of <see langword="this"/> should not be copied.</param>
        /// <returns>A <see langword="new"/> <see cref="Matrix"/>.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Columns"/> is odd.</exception>
        public Matrix Slice(bool removeLeft)
        {
            if (Columns % 2 != 0) throw new InvalidOperationException(nameof(Columns));

            var halfIndex = Columns / 2;
            return new Matrix(Rows, halfIndex, (row, column) => removeLeft ? this[row, column + halfIndex] : this[row, column]);
        }

        /// <summary>
        /// Creates a <see cref="Matrix"/> that will have its rows swapped with its columns.
        /// </summary>
        /// <returns>A <see langword="new"/> <see cref="Matrix"/>.</returns>
        public Matrix Transpose()
        {
            return new Matrix(Columns, Rows, (row, column) => this[column, row]);
        }

        /// <summary>
        /// Creates a <see cref="Matrix"/> that has identity <see cref="Matrix"/> added to it.
        /// </summary>
        /// <param name="onLeft">Indicates whether the identity should (<see langword="true"/>) be added to the left side of the <see cref="Matrix"/>.</param>
        /// <returns>A <see langword="new"/> <see cref="Matrix"/>.</returns>
        /// <exception cref="ArgumentException"><see cref="IsSquare"/> is <see langword="false"/>.</exception>
        public Matrix WithIdentity(bool onLeft)
        {
            if (!IsSquare) throw new ArgumentException(nameof(IsSquare));

            var newMatrix = new Matrix(Rows, Columns * 2);

            // Copies the original matrix to the left side of the returned matrix if onRight is true.
            for (int row = 0; row < Rows; row++)
                for (int column = 0, shift = newMatrix.Rows; column < Columns; column++, shift++)
                    newMatrix[row, onLeft ? shift : column] = this[row, column];

            // Assigns I to the matrix; shift is used in case I is to be added on the right side of a square matrix.
            for (int row = 0, shift = Rows; row < newMatrix.Rows; row++, shift++)
                newMatrix[row, onLeft ? row : shift] = 1;

            return newMatrix;
        }

        /// <summary>
        /// Creates a <see cref="DataTable"/> with its cells (<see langword="as"/> <see cref="string"/>) populated based on <see langword="this"/> <see cref="Matrix"/>.
        /// </summary>
        /// <returns>A <see langword="new"/> <see cref="DataTable"/>.</returns>
        public DataTable ToDataTable()
        {
            var dataTable = new DataTable();
            var columns = Enumerable.Range(1, Columns).Select(columnIndex => dataTable.Columns.Add(columnIndex.ToString(), typeof(string))).ToArray();
            var rows = Enumerable.Range(0, Rows).Select(rowIndex => dataTable.Rows.Add(this[rowIndex].Cast<object>().ToArray())).ToArray();
            return dataTable;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder(Rows * Columns * 4);

            for (var row = 0; row < Rows; row++)
            {
                for (var column = 0; column < Columns; column++)
                {
                    stringBuilder.Append(this[row, column]);
                    if (column < Columns - 1) stringBuilder.Append(COLUMN_SEPARATOR);
                }

                if (row < Rows - 1) stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }

        public override bool Equals(object compared)
        {
            if (!(compared is Matrix matrix) || Columns != matrix.Columns || Rows != matrix.Rows) return false;

            var differences = 0;

            Parallel.For(0, Rows, (row, rowLoop) =>
            {
                Parallel.For(0, Columns, (column, columnLoop) =>
                {
                    if (this[row, column] != matrix[row, column])
                    {
                        Interlocked.Increment(ref differences);
                        rowLoop.Stop();
                    }

                    if (differences > 0) columnLoop.Stop();
                });
            });

            return differences == 0;
        }

        public override int GetHashCode()
        {
            var iterateColumns = Columns > Rows;
            var stop = iterateColumns ? Columns : Rows;
            var total = 0L;

            for (var index = 0; index < stop; index++)
            {
                var cell = iterateColumns ? this[0, index] : this[index, 0];
                total = (cell.GetHashCode() + total) % int.MaxValue;
            }

            return (int)total;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(FractionMatrix), FractionMatrix, FractionMatrix.GetType());
        }

        #endregion Methods

    }
}
