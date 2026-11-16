using RaspMat.Extensions;
using RaspMat.Models;
using System.IO;
using System.Linq;

namespace RaspMat.Helpers
{
    internal static class Algorithms
    {

        public static Matrix BasisChangeMatrix(Matrix from, Matrix to)
        {
            if (from.Rows != to.Rows || from.Columns != to.Columns || !from.IsSquare)
            {
                throw new InvalidDataException(nameof(from.Rows));
            }

            from = from.Transpose();
            to = to.Transpose();

            return new Matrix(from.Rows, 2 * from.Rows, (row, column) => column < from.Rows ? to[column, row] : from[column - from.Rows, row])
                .GaussianElimination().Last().Result
                .Slice(removeLeft: true);
        }

    }
}
