using RaspMat.Extensions;
using System;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RaspMat.Models
{
    /// <summary>
    /// A representation of <see cref="Fraction"/>s.
    /// </summary>
    [Serializable]
    internal class Matrix : ISerializable
    {

        #region StaticFields

        /// <summary>
        /// Column separator.
        /// </summary>
        private readonly static string _columnSeparator = "\t";

        /// <summary>
        /// The preferred separator between rows.
        /// </summary>
        private readonly static string _rowSeparator = Environment.NewLine;

        /// <summary>
        /// Allowed separators between rows when parsing a <see langword="string"/>.
        /// </summary>
        private readonly static string[] _rowSeparators = new[] {
            _rowSeparator,
        };

        /// <summary>
        /// Allowed separators between columns when parsing a <see langword="string"/>.
        /// </summary>
        private readonly static string[] _columnSeparators = new[] {
            _columnSeparator,
            " ",
        };

        #endregion
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
        /// Returns <see langword="true"/> if <see langword="this"/> <see cref="Matrix"/> is square.
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

        private Matrix(Fraction[][] fractionMatrix) => FractionMatrix = fractionMatrix.ThrowIfNull();

        public Matrix(int rows, int columns, Func<int, int, Fraction> values)
            : this(ParallelEnumerable.Range(0, rows).AsOrdered().Select(row => ParallelEnumerable.Range(0, columns).AsOrdered().Select(column => values(row, column)).ToArray()).ToArray()) { }

        protected internal Matrix(SerializationInfo info, StreamingContext context)
            : this((Fraction[][])info.GetValue(nameof(FractionMatrix), typeof(Fraction[][]))) { }

        #endregion Constructors
        #region StaticMethods

        /// <summary>
        /// Creates a <see cref="Matrix"/> with ones inside and zeros outside its diagonal.
        /// </summary>
        /// <param name="size">Size of a square <see cref="Matrix"/>.</param>
        /// <returns>A <see langword="new"/> <see cref="Matrix"/>.</returns>
        public static Matrix Identity(int size)
        {
            return new Matrix(size, size, (row, column) => row == column ? 1 : 0);
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
            return new Matrix(matrix.Rows, matrix.Columns, (row, column) => matrix[row, column] * scalar);
        }

        public static Matrix operator *(Matrix matrix, Fraction scalar) => scalar * matrix;

        public static Matrix operator *(Matrix left, Matrix right)
        {
            if (left.Columns != right.Rows) throw new InvalidOperationException(nameof(left.Columns));

            return new Matrix(left.Rows, right.Columns, (row, column) =>
            {
                return ParallelEnumerable.Range(0, left.Columns).Select(cell => left[row, cell] * right[cell, column]).Aggregate((x, y) => x + y);
            });
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

        /// <summary>
        /// Creates a <see cref="Matrix"/> that has its cells populated with data from the <paramref name="input"/>.
        /// </summary>
        /// <param name="input">Provides data for the <see cref="Matrix"/>.</param>
        /// <returns>A <see langword="new"/> <see cref="Matrix"/> based on the <paramref name="input"/>.</returns>
        public static Matrix Parse(string input)
        {
            return new Matrix(input.Split(_rowSeparators, StringSplitOptions.RemoveEmptyEntries).Select(row => row.Split(_columnSeparators, StringSplitOptions.RemoveEmptyEntries).Select(cell => Fraction.Parse(cell)).ToArray()).ToArray());
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
        /// Creates a <see cref="Matrix"/> that will have two rows swapped with each other.
        /// </summary>
        /// <param name="first">Index of the row to swap with <paramref name="second"/>.</param>
        /// <param name="second">Index of the row to swap with <paramref name="first"/>.</param>
        /// <returns>A <see langword="new"/> <see cref="Matrix"/>.</returns>
        public Matrix SwapRows(int first, int second)
        {
            return new Matrix(Rows, Columns, (row, column) =>
            {
                if (row == first) return this[second, column];
                else return row == second ? this[first, column] : this[row, column];
            });
        }

        /// <summary>
        /// Adds one row to another.
        /// </summary>
        /// <param name="source">The row to take the values from. Will remain unchanged.</param>
        /// <param name="destination">The row to add the values to.</param>
        /// <param name="multiplier">Multiplier to </param>
        /// <returns></returns>
        public Matrix AddRows(int source, int destination, Fraction multiplier)
        {
            return new Matrix(Rows, Columns, (row, column) => row == destination ? multiplier * this[source, column] + this[row, column] : this[row, column]);
        }

        /// <summary>
        /// Creates a <see cref="Matrix"/> that will have two columns swapped with each other.
        /// </summary>
        /// <param name="first">Index of the column to swap with <paramref name="second"/></param>
        /// <param name="second">Index of the column to swap with <paramref name="first"/>.</param>
        /// <returns>A <see langword="new"/> <see cref="Matrix"/>.</returns>
        public Matrix SwapColumns(int first, int second)
        {
            return new Matrix(Rows, Columns, (row, column) =>
            {
                if (column == first) return this[row, second];
                else return column == second ? this[row, first] : this[row, column];
            });
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

            return new Matrix(Rows, Columns * 2, (row, column) =>
            {
                if (onLeft)
                {
                    if (column < Columns) return row == column ? 1 : 0;
                    else return this[row, column - Columns];
                }
                else
                {
                    if (column < Columns) return this[row, column];
                    else return row == column - Columns ? 1 : 0;
                }
            });
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

        /// <inheritdoc/>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder(Rows * 2 + Columns * 2 - 2);

            for (var row = 0; row < Rows; row++)
            {
                for (var column = 0; column < Columns; column++)
                {
                    stringBuilder.Append(this[row, column].ToString());
                    if (column < Columns - 1) stringBuilder.Append(_columnSeparator);
                }

                if (row < Rows - 1) stringBuilder.Append(_rowSeparator);
            }

            return stringBuilder.ToString();
        }

        /// <inheritdoc/>
        public override bool Equals(object compared)
        {
            return
                compared is Matrix matrix &&
                Columns == matrix.Columns &&
                Rows == matrix.Rows &&
                !FractionMatrix.AsParallel().Where((cells, row) => cells.AsParallel().Where((cell, column) => cell != matrix[row, column]).Any()).Any()
                ;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return
                Enumerable.Range(0, Math.Max(Columns, Rows))
                .Select(index => this[Math.Min(index, Rows - 1), Math.Min(index, Columns - 1)].GetHashCode())
                .Aggregate((left, right) => unchecked(left + right))
                ;
        }

        /// <inheritdoc/>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(FractionMatrix), FractionMatrix, FractionMatrix.GetType());
        }

        #endregion Methods

    }
}
