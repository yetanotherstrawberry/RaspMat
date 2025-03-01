using Newtonsoft.Json;
using RaspMat.Helpers;
using RaspMat.Properties;
using System;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RaspMat.Models
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class Matrix : ISerializable
    {

        private const char COLUMN_SEPARATOR = '\t';

        #region Properties
        /// <summary>
        /// <see cref="Fraction"/>s of this <see cref="Matrix"/>.
        /// </summary>
        [JsonProperty]
        private Fraction[][] FractionMatrix { get; }

        /// <summary>
        /// The total number of rows.
        /// </summary>
        public int Rows => FractionMatrix.Length;

        /// <summary>
        /// The total number of columns.
        /// </summary>
        public int Columns => FractionMatrix[0].Length;

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
            FractionMatrix = new Fraction[rows][];

            Parallel.For(0, Rows, row =>
            {
                this[row] = new Fraction[columns];
                Parallel.For(0, Columns, column => this[row][column] = values(row, column));
            });
        }

        /// <summary>
        /// Constructor used by the serializer.
        /// </summary>
        /// <param name="fractionMatrix">A representation of a <see cref="Matrix"/>.</param>
        [JsonConstructor]
        protected Matrix(Fraction[][] fractionMatrix)
        {
            FractionMatrix = fractionMatrix;
        }

        public Matrix(int rows, int columns) : this(Enumerable.Range(0, rows).Select(row => new Fraction[columns]).ToArray()) { }
        #endregion Constructors

        #region StaticMethods
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

        public static Matrix MultiplicationMatrix(int diagonal, int index, Fraction multiplier)
        {
            var newMatrix = Identity(diagonal);
            newMatrix[index, index] = multiplier;
            return newMatrix;
        }

        public static Matrix WithIdentity(Matrix matrix, bool onLeft)
        {
            if (matrix.Rows != matrix.Columns)
                throw new ArgumentException(message: Resources.ERR_MAT_NO_SQUARE, paramName: nameof(matrix));

            var newMatrix = new Matrix(matrix.Rows, matrix.Columns * 2);

            // Copies the original matrix to the left side of the returned matrix if onRight is true.
            for (int row = 0; row < matrix.Rows; row++)
                for (int column = 0, shift = newMatrix.Rows; column < matrix.Columns; column++, shift++)
                    newMatrix[row, onLeft ? shift : column] = matrix[row, column];

            // Assigns I to the matrix; shift is used in case I is to be added on the right side of a square matrix.
            for (int row = 0, shift = matrix.Rows; row < newMatrix.Rows; row++, shift++)
                newMatrix[row, onLeft ? row : shift] = 1;

            return newMatrix;
        }

        public static Matrix Slice(Matrix matrix, bool removeLeft)
        {
            if (matrix.Columns % 2 != 0)
                throw new ArgumentException(message: Resources.ERR_MAT_BAD_SHAPE, paramName: nameof(matrix));

            var colHalf = matrix.Columns / 2;

            return new Matrix(matrix.Rows, colHalf, (row, column) => removeLeft ? matrix[row, column + colHalf] : matrix[row, column]);
        }

        public static Matrix operator *(Fraction scale, Matrix matrix)
        {
            var newValue = new Matrix(matrix.Rows, matrix.Columns);

            Parallel.For(0, matrix.Rows, row =>
            {
                Parallel.For(0, matrix.Columns, column =>
                {
                    newValue[row, column] = matrix[row, column] * scale;
                });
            });

            return newValue;
        }

        public static Matrix operator *(Matrix matrix, Fraction scale) => scale * matrix;

        public static Matrix operator *(Matrix left, Matrix right)
        {
            if (left.Columns != right.Rows)
                throw new ArgumentException(Resources.ERR_MULTIPLY_MAT_MISMATCH);

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

        public static Matrix Transpose(Matrix matrix)
        {
            return new Matrix(matrix.Columns, matrix.Rows, (row, column) => matrix[column, row]);
        }
        #endregion StaticMethods

        #region Methods
        public static Matrix Parse(DataTable dataTable)
        {
            return new Matrix(dataTable.Rows.Count, dataTable.Columns.Count, (row, column) => Fraction.Parse(dataTable.Rows[row][column].ToString()));
        }

        /// <summary>
        /// Creates a <see cref="DataTable"/> with its cells (<see langword="as"/> <see cref="string"/>) populated based on <see langword="this"/> <see cref="Matrix"/>.
        /// </summary>
        /// <returns>A <see langword="new"/> <see cref="DataTable"/>.</returns>
        public DataTable ToDataTable() => DataTableHelpers.CreateStrDataTable((row, column) => this[row, column], Rows, Columns);

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
            var total = 0;

            for (var index = 0; index < stop; index++)
            {
                var cell = iterateColumns ? this[0, index] : this[index, 0];
                var bigInt = cell.Numerator * cell.Denominator;
                var totalAndBigInt = bigInt + total;
                total = (int)(totalAndBigInt > int.MaxValue ? totalAndBigInt % int.MaxValue : totalAndBigInt);
            }

            return total;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

            throw new NotImplementedException();
        }
        #endregion Methods

    }
}
