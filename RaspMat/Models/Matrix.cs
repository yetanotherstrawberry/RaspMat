using Newtonsoft.Json;
using RaspMat.Helpers;
using RaspMat.Interfaces;
using RaspMat.Properties;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspMat.Models
{
    internal class Matrix : IMatrix<Fraction>
    {

        private const int MinCols = 1, MinRows = 1;

        #region Properties
        [JsonProperty]
        private Fraction[][] FractionMatrix { get; }

        [JsonIgnore]
        public int Rows => FractionMatrix.Length;

        [JsonIgnore]
        public int Columns => FractionMatrix.First().Length;

        private Fraction[] this[int row]
        {
            get => FractionMatrix[row];
            set => FractionMatrix[row] = value;
        }

        public Fraction this[int row, int col]
        {
            get => this[row][col];
            private set => this[row][col] = value;
        }
        #endregion Properties

        #region Constructors
        public Matrix(int rows, int cols, Func<int, int, Fraction> values)
        {
            if (rows < MinRows || cols < MinCols)
                throw new ArgumentException(
                    message: string.Format(Resources.ERR_TOO_SMALL_MAT, MinCols, MinRows),
                    paramName: rows < MinRows ? nameof(rows) : nameof(cols));

            FractionMatrix = JaggedArrayHelper.Create(rows, cols, values);
        }

        private Matrix(Fraction[][] fractionMatrix, bool copy)
        {
            if (copy)
                FractionMatrix = fractionMatrix.Copy();
            else
                FractionMatrix = fractionMatrix;
        }

        public Matrix(string[][] input)
        {
            FractionMatrix = input.Convert(str => Fraction.Parse(str));
        }

        [JsonConstructor]
        public Matrix(Fraction[][] fractionMatrix) : this(fractionMatrix, copy: true) { }
        public Matrix(int rows, int cols) : this(rows, cols, (row, col) => Fraction.Zero) { }
        public Matrix(int size) : this(size, size) { }
        #endregion Constructors

        #region StaticMethods
        public static Matrix Identity(int size)
        {
            return RowEchelon(size, size);
        }

        public static Matrix RowEchelon(int rows, int columns)
        {
            return new Matrix(rows, columns, (row, col) => row == col ? 1 : 0);
        }

        public static Matrix SwapMatrix(Matrix matrix, int a, int b)
        {
            var ret = Identity(matrix.Rows);

            ret[a, a] = 0;
            ret[b, b] = 0;

            ret[a, b] = 1;
            ret[b, a] = 1;

            return ret;
        }

        public static Matrix AddToRowMatrix(Matrix matrix, int destination, int source, Fraction srcMultiplication)
        {
            var ret = Identity(matrix.Rows);

            ret[destination, source] = srcMultiplication;

            return ret;
        }

        public static Matrix AdditionMatrix(Matrix matrix, int destination, int source, Fraction scale)
        {
            var ret = Identity(matrix.Columns);

            ret[source, destination] = scale;

            return ret;
        }

        public static Matrix MultiplicationMatrix(int diagonal, int index, Fraction multiplier)
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
            for (int row = 0; row < matrix.Rows; row++)
                for (int col = 0, shift = ret.Rows; col < matrix.Columns; col++, shift++)
                    ret[row, onLeft ? shift : col] = matrix[row, col];

            /*
             * Assigns I to the matrix.
             * shift is used in case we add I on the right side of the square matrix.
             */
            for (int row = 0, shift = matrix.Rows; row < ret.Rows; row++, shift++)
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

                    for (int cell = 0; cell < left.Columns; cell++)
                        cellValue += left[row, cell] * right[cell, col];

                    ret[row, col] = cellValue;
                });
            });

            return ret;
        }

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

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    retSB.Append(this[i, j]);
                    if (j < Columns - 1)
                        retSB.Append("\t");
                }
                retSB.AppendLine();
            }

            return retSB.ToString();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Matrix mat) || Columns != mat.Columns || Rows != mat.Rows) return false;

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (this[row, col] != mat[row, col]) return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            var ret = 0;
            for (int row = 0; row < Rows; row++)
            {
                ret = (int)((ret + (this[row, 0].Numerator % int.MaxValue)) % int.MaxValue);
            }
            for (int column = 0; column < Columns; column++)
            {
                ret = (int)((ret + (this[0, column].Denominator % int.MaxValue)) % int.MaxValue);
            }
            return ret;
        }
        #endregion Methods

    }
}
