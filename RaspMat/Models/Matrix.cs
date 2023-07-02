using RaspMat.Helpers;
using RaspMat.Properties;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RaspMat.Models
{
    internal class Matrix
    {

        private const long MinCols = 1, MinRows = 1;

        #region Properties
        [JsonInclude]
        public Fraction[][] FractionMatrix { get; }

        [JsonIgnore]
        public long Rows => FractionMatrix.LongLength;
        [JsonIgnore]
        public long Columns => FractionMatrix[0].LongLength;

        private Fraction[] this[long row]
        {
            get => FractionMatrix[row];
            set => FractionMatrix[row] = value;
        }

        public Fraction this[long row, long col]
        {
            get => this[row][col];
            private set => this[row][col] = value;
        }
        #endregion Properties

        #region Constructors
        public Matrix(long rows, long cols, Func<long, long, Fraction> values)
        {
            if (rows < MinRows || cols < MinCols)
                throw new ArgumentException(
                    message: string.Format(Resources.ERR_TOO_SMALL_MAT, MinCols, MinRows),
                    paramName: rows < MinRows ? nameof(rows) : nameof(cols));

            FractionMatrix = JaggedArrayHelper.Create(rows, cols, values);
        }

        private Matrix(Fraction[][] inputFracMat, bool copy)
        {
            if (copy)
                FractionMatrix = inputFracMat.Copy();
            else
                FractionMatrix = inputFracMat;
        }

        public Matrix(string[][] input)
        {
            FractionMatrix = input.Convert(str => Fraction.Parse(str));
        }

        public Matrix(long rows, long cols) : this(rows, cols, (row, col) => Fraction.Zero) { }
        [JsonConstructor]
        public Matrix(Fraction[][] fractionMatrix) : this(fractionMatrix, copy: true) { }
        public Matrix(long size) : this(size, size) { }
        #endregion Constructors

        #region StaticMethods
        public static Matrix Identity(long size)
        {
            return new Matrix(size, size, (row, col) => row == col ? 1 : 0);
        }

        public static Matrix SwapMatrix(Matrix matrix, long a, long b)
        {
            var ret = Identity(matrix.Rows);

            ret[a, a] = 0;
            ret[b, b] = 0;

            ret[a, b] = 1;
            ret[b, a] = 1;

            return ret;
        }

        public static Matrix AddToRowMatrix(Matrix matrix, long destination, long source, Fraction srcMultiplication)
        {
            var ret = Identity(matrix.Rows);

            ret[destination, source] = srcMultiplication;

            return ret;
        }

        public static Matrix AdditionMatrix(Matrix matrix, long destination, long source, Fraction scale)
        {
            var ret = Identity(matrix.Columns);

            ret[source, destination] = scale;

            return ret;
        }

        public static Matrix MultiplicationMatrix(long diagonal, long index, Fraction multiplier)
        {
            var ret = Identity(diagonal);

            ret[index, index] = multiplier;

            return ret;
        }

        public static Matrix AddI(Matrix matrix, bool onLeft)
        {
            if (matrix.Rows != matrix.Columns)
                throw new ArgumentException(message: Resources.ERR_MAT_NO_SQUARE, paramName: nameof(matrix));

            var ret = new Matrix(matrix.Rows, matrix.Columns * 2);

            // Copies the original matrix to the left side of the returned matrix if onRight is true.
            for (long row = 0; row < matrix.Rows; row++)
                for (long col = 0, shift = ret.Rows; col < matrix.Columns; col++, shift++)
                    ret[row, onLeft ? shift : col] = matrix[row, col];

            /*
             * Assigns I to the matrix.
             * shift is used in case we add I on the right side of the square matrix.
             */
            for (long row = 0, shift = matrix.Rows; row < ret.Rows; row++, shift++)
                ret[row, onLeft ? row : shift] = 1;

            return ret;
        }

        public static Matrix Slice(Matrix matrix, bool removeFromLeft)
        {
            if (matrix.Columns % 2 != 0 || matrix.Rows != (matrix.Columns / 2))
                throw new ArgumentException(message: Resources.ERR_MAT_BAD_SHAPE, paramName: nameof(matrix));

            var colHalf = matrix.Columns / 2;

            return new Matrix(JaggedArrayHelper.Copy(matrix.FractionMatrix, removeFromLeft ? colHalf : 0, colHalf));
        }

        public static Matrix operator *(Fraction scale, Matrix matrix)
        {
            var temp = new Matrix(matrix.Rows, matrix.Columns);

            Parallel.For(0, matrix.Rows, row =>
            {
                Parallel.For(0, matrix.Columns, col =>
                {
                    temp[row, col] = matrix[row, col] * scale;
                });
            });

            return temp;
        }

        public static Matrix operator *(Matrix matrix, Fraction scale) => scale * matrix;

        public static Matrix operator *(Matrix left, Matrix right)
        {
            if (left.Columns != right.Rows)
                throw new ArgumentException(Resources.ERR_MULTIPLY_MAT_MISMATCH);

            var ret = new Matrix(left.Rows, right.Columns);

            Parallel.For(0, ret.Rows, row =>
            {
                Parallel.For(0, ret.Columns, col =>
                {
                    var cellValue = Fraction.Zero;

                    for (long cell = 0; cell < left.Columns; cell++)
                        cellValue += left[row, cell] * right[cell, col];

                    ret[row, col] = cellValue;
                });
            });

            return ret;
        }

        public static bool operator !=(Matrix left, Matrix right)
        {
            if (left.Columns != right.Columns || left.Rows != right.Rows) return true;

            for (long row = 0; row < left.Rows; row++)
            {
                for (long col = 0; col < right.Columns; col++)
                {
                    if (left[row, col] != right[row, col]) return true;
                }
            }

            return false;
        }

        public static bool operator ==(Matrix left, Matrix right) => !(left != right);

        public static Matrix Transpose(Matrix matrix)
        {
            return new Matrix(matrix.Columns, matrix.Rows, (row, col) => matrix[col][row]);
        }
        #endregion StaticMethods

        #region Methods
        public static Matrix Parse(DataTable dataTable)
        {
            return new Matrix(
                dataTable.AsEnumerable()
                .Select(
                    row => row.ItemArray
                    .Select(cell => cell.ToString())
                    .ToArray()).ToArray());
        }

        public DataTable ToDataTable() => DataTableHelpers.CreateDataTable(Rows, Columns, (row, col) => this[row, col]);

        public override string ToString()
        {
            var retSB = new StringBuilder();

            for (long i = 0; i < Rows; i++)
            {
                for (long j = 0; j < Columns; j++)
                {
                    retSB.Append(this[i, j]);
                    if (j < Columns - 1)
                        retSB.Append("\t");
                }
                retSB.AppendLine();
            }

            return retSB.ToString();
        }

        public override bool Equals(object obj) => obj is Matrix mat && this == mat;

        public override int GetHashCode()
        {
            var ret = 0;
            for (long row = 0; row < Rows; row++)
            {
                ret = (int)((ret + (this[row, 0].Numerator % int.MaxValue)) % int.MaxValue);
            }
            for (long column = 0; column < Columns; column++)
            {
                ret = (int)((ret + (this[0, column].Denominator % int.MaxValue)) % int.MaxValue);
            }
            return ret;
        }
        #endregion Methods

    }
}
