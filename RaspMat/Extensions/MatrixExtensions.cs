using RaspMat.Models;
using RaspMat.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RaspMat.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="Matrix"/> <see langword="class"/>.
    /// </summary>
    internal static class MatrixExtensions
    {

        /// <summary>
        /// Performs Gaussian elimination on a <see cref="Matrix"/>. Returns all steps that were made with matrices. Does not modify the provided <paramref name="matrix"/>.
        /// The implementation is based on
        /// <see href="https://en.wikipedia.org/wiki/Gaussian_elimination#Pseudocode">en.wikipedia.org</see> and 
        /// <see href="https://apollo.astro.amu.edu.pl/PAD/pmwiki.php?n=Dybol.DydaktykaEliminacjaGaussa">apollo.astro.amu.edu.pl</see> algorithms.
        /// </summary>
        /// <param name="matrix"><see cref="Matrix"/> to reduce. Will not be modified.</param>
        /// <param name="reducedEchelon">Whether the resulting <see cref="Matrix"/> should be reduced row echelon (<see langword="true"/>) or row echelon (<see langword="false"/>).</param>
        /// <returns>An <see cref="IList{T}"/> of <see cref="AlgorithmStep{T}"/> of <see cref="Matrix"/>.</returns>
        public static IList<AlgorithmStep<Matrix>> GaussianElimination(this Matrix matrix, bool reducedEchelon = true)
        {
            AlgorithmStep<Matrix> GenerateStep(Matrix stepMatrix, string text, params object[] interpolation)
            {
                return new AlgorithmStep<Matrix>(string.Format(text, interpolation), stepMatrix);
            }

            var steps = new List<AlgorithmStep<Matrix>>()
            {
                GenerateStep(matrix, string.Empty),
            };
            var row = 0;
            var column = 0;

            while (row < matrix.Rows && column < matrix.Columns)
            {
                while (column < matrix.Columns && matrix[row, column].IsZero)
                {
                    var onlyZeros = true;
                    var shift = row;
                    while (shift < matrix.Rows)
                    {
                        if (!matrix[shift, column].IsZero)
                        {
                            onlyZeros = false;
                            break;
                        }
                        shift++;
                    }
                    if (onlyZeros) column++;

                    if (shift != row && row < matrix.Rows && shift < matrix.Rows)
                    {
                        matrix = matrix.SwapRows(row, shift);
                        steps.Add(GenerateStep(matrix, Resources.STEP_SWAP_ROWS, row + 1, shift + 1));
                    }
                }

                if (column < matrix.Columns)
                {
                    var reciprocal = matrix[row, column].Reciprocal();
                    if (!reciprocal.IsOne)
                    {
                        matrix = Matrix.MultiplicationMatrix(matrix.Rows, row, reciprocal) * matrix;
                        steps.Add(GenerateStep(matrix, Resources.STEP_MULTIPLY_ROW, row + 1, reciprocal));
                    }

                    reciprocal = matrix[row, column].Reciprocal();
                    for (var destination = row + 1; destination < matrix.Rows; destination++)
                    {
                        var multiplier = matrix[destination, column] * -reciprocal;

                        if (!multiplier.IsZero)
                        {
                            matrix = matrix.AddRows(row, destination, multiplier);
                            steps.Add(GenerateStep(matrix, Resources.STEP_SUM_ROWS, row + 1, multiplier, destination + 1));
                        }
                    }
                }

                row++;
                column++;
            }

            /*
             * Subtract from all rows above the current one its value multiplied by the ratio,
             * so that all rows above have 0 in the column of the leading 1 of the current row.
             */
            if (reducedEchelon)
            {
                for (var currentRow = matrix.Rows - 1; currentRow > 0; currentRow--)
                {
                    var nonZeroColumn = 0;
                    while (nonZeroColumn < matrix.Columns && matrix[currentRow, nonZeroColumn].IsZero) nonZeroColumn++;
                    if (nonZeroColumn == matrix.Columns) continue;

                    for (var destination = currentRow - 1; destination >= 0; destination--)
                    {
                        var multiplier = matrix[destination, nonZeroColumn] / -matrix[currentRow, nonZeroColumn];

                        if (!multiplier.IsZero)
                        {
                            matrix = matrix.AddRows(currentRow, destination, multiplier);
                            steps.Add(GenerateStep(matrix, Resources.STEP_SUM_ROWS, currentRow + 1, multiplier, destination + 1));
                        }
                    }
                }
            }

            return steps;
        }

    }
}
