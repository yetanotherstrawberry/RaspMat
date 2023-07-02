using Microsoft.VisualBasic;
using RaspMat.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;

namespace RaspMat.Views
{
        /*
    public partial class GaussianWindow : Window
    {

        private const string bladPodalgorytmu = "Algorytm podrzędny został zakończony zanim algorytm wyższego poziomu skończył pracę.";



        private void PrzeksztalcenieLiniowe(object obiekt, RoutedEventArgs argumenty)
        {

            try
            {

                bool czyPokaz = (bool)TrybPokazu.IsChecked;

                string tekst_z = Interaction.InputBox("Wprowadź bazę, z której chcesz przejść w formacie (1,2,3);(3,2,1).", "Baza B1", "(1,0,0);(1,1,0);(1,1,1)");
                if (tekst_z.Length == 0) return;

                string tekst_do = Interaction.InputBox("Wprowadź bazę, do której chcesz przejść w formacie (1,2,3);(3,2,1).", "Baza B2", "(1,0,0);(0,1,0);(0,0,1)");
                if (tekst_do.Length == 0) return;

                string wzor = Interaction.InputBox("Podaj przekształcenie liniowe.", "Wzór przekształcenia T", "3*x1 + x2, x1 + x3, x1 - x3");
                if (wzor.Length == 0) return;

                string[] przeksztalcenia = wzor.Split(',');
                string[] baza_z = (from wektor in tekst_z.Split(';') select wektor.Replace("(", "").Replace(")", "")).ToArray();
                string[] baza_do = (from wektor in tekst_do.Split(';') select wektor.Replace("(", "").Replace(")", "")).ToArray();
                string[][] wyniki = new string[baza_z.Length][];

                for (int i = 0; i < baza_z.Length; i++)
                {

                    string[] wynik_wektora = new string[przeksztalcenia.Length];
                    List<Argument> argumenty_rownania = new List<Argument>();
                    string[] elementy_wektora_z = baza_z[i].Split(',');

                    for (int j = 1; j <= elementy_wektora_z.Length; j++)
                        argumenty_rownania.Add(new Argument("x" + j.ToString(), elementy_wektora_z[j - 1]));

                    for (int j = 0; j < przeksztalcenia.Length; j++)
                        wynik_wektora[j] = mXparser.toFractionString(new org.mariuszgromada.math.mxparser.Expression(przeksztalcenia[j], argumenty_rownania.ToArray()).calculate());

                    if (czyPokaz)
                        if (Interaction.MsgBox("Należy wykonać przekształcenie bazy B1 przez podaną funkcję:\nT((" + baza_z[i] + ")) = (" + wzor + ") = (" + string.Join(", ", wynik_wektora) + ")\nCzy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Przekształcenie liniowe") == MsgBoxResult.No)
                            if (obiekt != null)
                                return;
                            else
                                throw new ConstraintException(bladPodalgorytmu);

                    wyniki[i] = wynik_wektora;

                }

                List<Matrix> macierze_przejscia = new List<Matrix>();

                for (int i = 0; i < wyniki.Length; i++)
                {

                    string[][] do_redukcji = new string[baza_do.Length + 1][];

                    for (int j = 0; j < baza_do.Length; j++)
                        do_redukcji[j] = baza_do[j].Split(',');

                    do_redukcji[baza_do.Length] = wyniki[i];

                    macierze_przejscia.Add(Matrix.Transpose(new Matrix(do_redukcji)));

                }

                List<Matrix> rozwiazane_macierze = new List<Matrix>();

                foreach (Matrix do_rozwiazania in macierze_przejscia)
                {

                    matrix = do_rozwiazania;
                    OdswiezMacierz();
                    if (czyPokaz)
                        if (Interaction.MsgBox("Należy rozwiązać poniższą macierz przejścia do bazy B2 za pomocą eliminacji:\n" + matrix.ToString() + "\nW kolumnach zostały wstawione poszczególne wektory bazy, a w ostatniej jeden z wektorów po przekształceniu liniowym. Czy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Przekształcenie liniowe") == MsgBoxResult.No)
                            if (obiekt != null)
                                return;
                            else
                                throw new ConstraintException(bladPodalgorytmu);

                    TotalGaussianElimination(null, argumenty);

                    rozwiazane_macierze.Add(matrix);

                }

                List<string[]> ret = new List<string[]>();

                foreach (Matrix rozwiazana_macierz in rozwiazane_macierze)
                {

                    string[] wiersze = new string[rozwiazana_macierz.Rows];

                    for (int i = 0; i < rozwiazana_macierz.Rows; i++)
                        wiersze[i] = rozwiazana_macierz[i, rozwiazana_macierz.Columns - 1].ToString();

                    ret.Add(wiersze);

                }

                matrix = Matrix.Transpose(new Matrix(ret.ToArray()));
                OdswiezMacierz();

                if (czyPokaz)
                    Interaction.MsgBox("Wszystkie macierze zostały rozwiązane.\nUzyskane wyniki należy wstawić do kolumn nowej macierzy, która będzie macierzą przekształcenia liniowego.", MsgBoxStyle.OkOnly, "Macierz utworzona - koniec pracy");

            }
            catch (Exception wyjatek)
            {

                if (wyjatek is ArgumentNullException || wyjatek is ArgumentException)
                    Blad(wyjatek.Message);
                else if (wyjatek is ArgumentOutOfRangeException || wyjatek is IndexOutOfRangeException || wyjatek is FormatException)
                    Blad("Wygląda na to, że nieprawidłowo określono przekształcenie.\nPamiętaj, że mnożenie musi być jawne (np. 2*x1, ale nie 2x1).\nZwróć uwagę, że baza B1 musi być z przestrzeni sprzed przekształcenia, a B2 po.\nWyjątek:\n" + wyjatek.Message);
                else
                    throw wyjatek;

            }

        }

    }
*/
}
/*
private void Odwrotnosc(object obiekt, RoutedEventArgs argumenty)
{

    bool czyPokaz = (bool)TrybPokazu.IsChecked;

    if (czyPokaz)
        if (Interaction.MsgBox("Dopisz I z prawej strony macierzy." + Environment.NewLine + "Czy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Odwracanie macierzy") == MsgBoxResult.No)
            return;
    matrix = Matrix.AddI(matrix, true);
    OdswiezMacierz();

    if (czyPokaz)
        if (Interaction.MsgBox("Wykonaj całkowitą redukcję." + Environment.NewLine + "Czy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Odwracanie macierzy") == MsgBoxResult.No)
            return;
    TotalGaussianElimination(null, argumenty);
    OdswiezMacierz();

    for (long h = 0; h < matrix.Columns; h++)
    {

        bool czySameZera = true;

        for (long j = 0; j < matrix.Rows; j++)
            if (matrix[j, h].Numerator != 0)
            {
                czySameZera = false;
                break;
            }

        if (czySameZera)
            throw new ArgumentException(bladWyznacznik);

    }

    for (long h = 0; h < matrix.Rows; h++)
    {

        bool czySameZera = true;

        for (long j = 0; j < matrix.Columns / 2; j++)
            if (matrix[h, j].Numerator != 0)
            {
                czySameZera = false;
                break;
            }

        if (czySameZera)
            throw new ArgumentException(bladWyznacznik);

    }

    if (czyPokaz)
        if (Interaction.MsgBox("Usuń I z lewej strony." + Environment.NewLine + "Czy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Odwracanie macierzy") == MsgBoxResult.No)
            return;
    matrix = Matrix.Slice(matrix, true);
    OdswiezMacierz();

    if (czyPokaz)
        Interaction.MsgBox("Macierz została odwrócona.", MsgBoxStyle.OkOnly, "Koniec pracy algorytmu");
}
*/