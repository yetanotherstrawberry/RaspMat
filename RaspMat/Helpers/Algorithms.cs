using RaspMat.DTOs;
using RaspMat.Interfaces;
using RaspMat.Models;
using RaspMat.Properties;
using System.Collections.Generic;

namespace RaspMat.Helpers
{
    /// <summary>
    /// Static class for holding algorithms.
    /// </summary>
    internal static class Algorithms
    {

        /// <summary>
        /// Performs Gaussian elimination on a <see cref="Matrix"/>. Returns all steps that were made with matrices. Does not modify the original matrix.
        /// The implementation is based on
        /// <see href="https://en.wikipedia.org/wiki/Gaussian_elimination#Pseudocode">en.wikipedia.org</see> and 
        /// <see href="https://apollo.astro.amu.edu.pl/PAD/pmwiki.php?n=Dybol.DydaktykaEliminacjaGaussa">apollo.astro.amu.edu.pl</see> algorithms.
        /// </summary>
        /// <param name="matrix"><see cref="Matrix"/> to reduce. Will not be modified.</param>
        /// <param name="reducedEchelon">Whether the resulting <see cref="Matrix"/> should be reduced row echelon (<see langword="true"/>) or row echelon (<see langword="false"/>).</param>
        /// <returns>Steps and matrices created during elimination: <see cref="IList{T}"/> where <c>T</c> is <see cref="IAlgorithmResult{T}"/> where <c>T</c> is <see cref="Matrix"/>.</returns>
        public static IList<AlgorithmStepDTO<Matrix>> GaussianElimination(this Matrix matrix, bool reducedEchelon = true)
        {
            AlgorithmStepDTO<Matrix> GenerateStep(string text, Matrix stepMatrix, params object[] interpolation)
            {
                return new AlgorithmStepDTO<Matrix>(string.Format(text, interpolation), stepMatrix);
            }

            var ret = new List<AlgorithmStepDTO<Matrix>>();
            var row = 0;
            var col = 0;

            while (row < matrix.Rows && col < matrix.Columns)
            {
                while (col < matrix.Columns && matrix[row, col] == 0)
                {
                    var onlyZeros = true;

                    var shift = row;
                    while (shift < matrix.Rows)
                    {
                        if (matrix[shift, col] != 0)
                        {
                            onlyZeros = false;
                            break;
                        }
                        shift++;
                    }

                    if (onlyZeros) col++;

                    if (shift != row && row < matrix.Rows && shift < matrix.Rows)
                    {
                        matrix = Matrix.SwapMatrix(matrix, row, shift) * matrix;
                        ret.Add(GenerateStep(Resources.STEP_SWAP_ROWS, matrix, row + 1, shift + 1));
                    }
                }

                if (col < matrix.Columns)
                {
                    var reciprocal = matrix[row, col].Reciprocal();
                    if (reciprocal != 1)
                    {
                        matrix = Matrix.MultiplicationMatrix(matrix.Rows, row, reciprocal) * matrix;
                        ret.Add(GenerateStep(Resources.STEP_MULTIPLY_ROW, matrix, row + 1, reciprocal));
                    }

                    reciprocal = matrix[row, col].Reciprocal();

                    for (var destination = row + 1; destination < matrix.Rows; destination++)
                    {
                        var multiplier = matrix[destination, col] * -reciprocal;

                        if (multiplier != 0)
                        {
                            matrix = Matrix.AddToRowMatrix(matrix, destination, row, multiplier) * matrix;
                            ret.Add(GenerateStep(Resources.STEP_SUM_ROWS, matrix, row + 1, multiplier, destination + 1));
                        }
                    }
                }

                row++;
                col++;
            }

            // Subtract from all rows above the current one its value multiplied by the ratio,
            // so that all rows above have 0 in the column of the leading 1 of the current row.
            if (reducedEchelon)
            {
                for (var currentRow = matrix.Rows - 1; currentRow > 0; currentRow--)
                {
                    var nonZeroCol = 0;

                    while (nonZeroCol < matrix.Columns && matrix[currentRow, nonZeroCol] == 0) nonZeroCol++;

                    if (nonZeroCol == matrix.Columns) continue;

                    for (var destination = currentRow - 1; destination >= 0; destination--)
                    {
                        var multiplier = matrix[destination, nonZeroCol] / -matrix[currentRow, nonZeroCol];

                        if (multiplier != 0)
                        {
                            matrix = Matrix.AddToRowMatrix(matrix, destination, currentRow, multiplier) * matrix;
                            ret.Add(GenerateStep(Resources.STEP_SUM_ROWS, matrix, currentRow + 1, multiplier, destination + 1));
                        }
                    }
                }
            }

            return ret;
        }

        public static IList<AlgorithmStepDTO<Matrix>> InverseMatrix(this Matrix matrix)
        {
            var ret = new List<AlgorithmStepDTO<Matrix>>();
            var mat = Matrix.AddI(matrix, onLeft: false);
            mat.GaussianElimination(reducedEchelon: true);
            mat = Matrix.Slice(mat, removeLeft: true);

            return ret;
        }

        /*
        private static string[] StrToStrVecs(string vec, char vectorsSplitter = ';')
            => Array.ConvertAll(vec.Split(vectorsSplitter), str => str.TrimStart('(').TrimEnd(')'));

        private static IList<Matrix> BasisStrToMatrix(string[] from, string[] to, char vectorSplitter = ',')
        {
            if (from.Length != to.Length)
                throw new ArgumentOutOfRangeException(nameof(from));

            var ret = new List<Fraction[][]>();

            foreach (var vecFrom in from)
            {
                var temp = new Fraction[to.Length + 1][];

                for (long i = 0; i < to.Length; i++)
                {
                    temp[i] = Array.ConvertAll(to[i].Split(vectorSplitter), str => Fraction.Parse(str));
                }

                temp[to.Length] = Array.ConvertAll(vecFrom.Split(vectorSplitter), str => Fraction.Parse(str));

                ret.Add(temp);
            }
            //                    if (Interaction.MsgBox("Należy przedstawić wektor (" + wektor_z + ") jako kombinację wektorów bazy {" + tekst_do + "} za pomocą macierzy, której kolumnami są poszczególne wektory.\nCzy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Eliminacja Gaussa") == MsgBoxResult.No)
            return ret.ConvertAll(matFrac2D => Matrix.Transpose(new Matrix(matFrac2D)));
        }

        public static Matrix BasisChangeMatrix(string from, string to)
        {
            var macierze_do_rozwiazania = BasisStrToMatrix(StrToStrVecs(from), StrToStrVecs(to));
            //if (czyPokaz)
            //   if (Interaction.MsgBox("Należy rozwiązać " + macierze_do_rozwiazania.Count() + " macierz/e(-y) za pomocą całkowitej eliminacji.\nCzy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Eliminacja Gaussa") == MsgBoxResult.No)
            //      if (obiekt != null)
            //         return;
            //    else
            //       throw new ConstraintException(bladPodalgorytmu);

            List<Matrix> rozwiazane_macierze = new List<Matrix>();

            foreach (Matrix macierz_do_rozwiazania in macierze_do_rozwiazania)
            {

                //if (czyPokaz)
                //   if (Interaction.MsgBox("Należy rozwiązać następującą macierz:\n" + macierz_do_rozwiazania.ToString() + "\nskładającą się z wektorów bazy docelowej i jednego z wektorów z bazy pierwotnej w ostatniej kolumnie.\nCzy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Eliminacja Gaussa") == MsgBoxResult.No)
                //      if (obiekt != null)
                //         return;
                //    else
                //       throw new ConstraintException(bladPodalgorytmu);

                TotalGaussianElimination(macierz_do_rozwiazania);

                rozwiazane_macierze.Add(macierz_do_rozwiazania);

            }

            var ret = new List<string[]>();

            foreach (Matrix rozwiazana_macierz in rozwiazane_macierze)
            {

                string[] wiersze = new string[rozwiazana_macierz.Rows];

                for (long i = 0; i < rozwiazana_macierz.Rows; i++)
                    wiersze[i] = rozwiazana_macierz[i, rozwiazana_macierz.Columns - 1].ToString();

                ret.Add(wiersze);
            }

            //if (czyPokaz)
            //Interaction.MsgBox("Wszystkie macierze zostały rozwiązane.\nUzyskane wyniki należy wstawić do kolumn nowej macierzy, która będzie macierzą przejścia z bazy B1 do B2.", MsgBoxStyle.OkOnly, "Macierz utworzona - koniec pracy");
            return Matrix.Transpose(new Matrix(ret.ToArray()));
        }
        */
    }
}
