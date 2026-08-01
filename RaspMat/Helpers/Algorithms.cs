using RaspMat.Extensions;
using RaspMat.Models;
using System.IO;
using System.Linq;

namespace RaspMat.Helpers
{
    internal static class Algorithms
    {


        /*public static Matrix BasisChangeMatrix(string from, string to)
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

                macierz_do_rozwiazania.GaussianElimination();

                rozwiazane_macierze.Add(macierz_do_rozwiazania);

            }

            var ret = new List<string[]>();

            foreach (Matrix rozwiazana_macierz in rozwiazane_macierze)
            {

                string[] wiersze = new string[rozwiazana_macierz.Rows];

                for (var i = 0; i < rozwiazana_macierz.Rows; i++)
                    wiersze[i] = rozwiazana_macierz[i, rozwiazana_macierz.Columns - 1].ToString();

                ret.Add(wiersze);
            }

            //if (czyPokaz)
            //Interaction.MsgBox("Wszystkie macierze zostały rozwiązane.\nUzyskane wyniki należy wstawić do kolumn nowej macierzy, która będzie macierzą przejścia z bazy B1 do B2.", MsgBoxStyle.OkOnly, "Macierz utworzona - koniec pracy");
            return Matrix.Transpose(new Matrix(ret.ToArray()));
        }*/

        public static Matrix BasisChangeMatrix(Fraction[][] from, Fraction[][] to)
        {
            if (from.Length != to.Length) throw new InvalidDataException(nameof(from.Length));

            var toElminate = from.Select(vecFrom => new Matrix(to[0].Length + 1, to.Length, (row, column) => row == to.Length ? vecFrom[column] : to[column][row]));
            var eliminated = toElminate.Select(matrix => matrix.GaussianElimination().Last().Result).AsParallel().AsOrdered().ToArray();

            return new Matrix(eliminated.Length, eliminated[0].Columns, (row, column) =>
            {
                return 0;
            });
        }

    }
}
