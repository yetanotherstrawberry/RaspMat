using Arytmetyka;
using Microsoft.VisualBasic;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;

namespace AlgebraProjekt
{

    public partial class MainWindow : Window
    {

        private const string bladPodalgorytmu = "Algorytm podrzędny został zakończony zanim algorytm wyższego poziomu skończył pracę.";

        // https://en.wikipedia.org/wiki/Gaussian_elimination#Pseudocode
        // https://apollo.astro.amu.edu.pl/PAD/pmwiki.php?n=Dybol.DydaktykaEliminacjaGaussa
        private void EliminacjaGaussa(object obiekt, RoutedEventArgs argumenty)
        {

            bool czyPokaz = (bool)TrybPokazu.IsChecked;

            for (long i = 0, kol = 0; i < macierz.Wiersze() && kol < macierz.Kolumny(); i++, kol++)
            {

                while (kol < macierz.Kolumny() && macierz.Tablica()[i, kol].Licznik() == 0)
                {

                    bool czySameZera = true;

                    long j;
                    for (j = i; j < macierz.Wiersze(); j++)
                        if (macierz.Tablica()[j, kol].Licznik() != 0)
                        {
                            czySameZera = false;
                            break;
                        }

                    if (czySameZera) kol++;

                    if (j != i && i < macierz.Wiersze() && j < macierz.Wiersze())
                    {

                        if (czyPokaz)
                            if (Interaction.MsgBox("Zamień wiersze " + i.ToString() + " i " + j.ToString() + ".\nCzy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Eliminacja Gaussa") == MsgBoxResult.No)
                                if (obiekt != null)
                                    return;
                                else
                                    throw new ConstraintException(bladPodalgorytmu);

                        macierz = Macierz.MacierzZamianyWierszy(macierz, i, j) * macierz;
                        OdswiezMacierz();

                    }

                }

                if (kol < macierz.Kolumny())
                {

                    if (czyPokaz && !new Ulamek(macierz.Tablica()[i, kol].Mianownik(), macierz.Tablica()[i, kol].Licznik()).Equals(new Ulamek(1, 1)))
                        if (Interaction.MsgBox("Pomnóż wiersz " + i.ToString() + " przez (" + new Ulamek(macierz.Tablica()[i, kol].Mianownik(), macierz.Tablica()[i, kol].Licznik()).ToString() + ").\nCzy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Eliminacja Gaussa") == MsgBoxResult.No)
                            if (obiekt != null)
                                return;
                            else
                                throw new ConstraintException(bladPodalgorytmu);

                    macierz = Macierz.MacierzMnozeniaWiersza(macierz, i, new Ulamek(macierz.Tablica()[i, kol].Mianownik(), macierz.Tablica()[i, kol].Licznik())) * macierz;
                    OdswiezMacierz();

                    Ulamek przez_co_mnozyc = new Ulamek(macierz.Tablica()[i, kol].Mianownik(), macierz.Tablica()[i, kol].Licznik());

                    for (long j = i + 1; j < macierz.Wiersze(); j++)
                    {

                        Ulamek co_mnozyc = new Ulamek(macierz.Tablica()[j, kol].Licznik(), macierz.Tablica()[j, kol].Mianownik());
                        Ulamek mnoznik = co_mnozyc * przez_co_mnozyc * new Ulamek(-1, 1);

                        if (czyPokaz && !mnoznik.Equals(new Ulamek(0, 1)))
                            if (Interaction.MsgBox("Dodaj do wiersza " + j.ToString() + " wiersz " + i.ToString() + " pomnożony przez (" + mnoznik.ToString() + ").\nCzy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Eliminacja Gaussa") == MsgBoxResult.No)
                                if (obiekt != null)
                                    return;
                                else
                                    throw new ConstraintException(bladPodalgorytmu);

                        macierz = Macierz.MacierzDodawaniaWierszy(macierz, j, i, mnoznik) * macierz;
                        OdswiezMacierz();

                    }

                }

            }

            for (long i = macierz.Wiersze() - 1; i > 0; i--)
            {

                long kolumna_bez_zera = 0;

                while (kolumna_bez_zera < macierz.Kolumny() && macierz.Tablica()[i, kolumna_bez_zera].Licznik() == 0) kolumna_bez_zera++;

                if (kolumna_bez_zera == macierz.Kolumny()) continue;

                for (long x = i - 1; x >= 0; x--)
                {

                    Ulamek skalar = (new Ulamek(macierz.Tablica()[x, kolumna_bez_zera].Licznik(), macierz.Tablica()[x, kolumna_bez_zera].Mianownik()) / new Ulamek(macierz.Tablica()[i, kolumna_bez_zera].Licznik(), macierz.Tablica()[i, kolumna_bez_zera].Mianownik())) * new Ulamek(-1, 1);

                    if (czyPokaz && !skalar.Equals(new Ulamek(0, 1)))
                        if (Interaction.MsgBox("Dodaj do wiersza " + x.ToString() + " wiersz " + i.ToString() + " pomnożony przez (" + skalar.ToString() + ").\nCzy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Eliminacja Gaussa") == MsgBoxResult.No)
                            if (obiekt != null)
                                return;
                            else
                                throw new ConstraintException(bladPodalgorytmu);

                    macierz = Macierz.MacierzDodawaniaWierszy(macierz, x, i, skalar) * macierz;
                    OdswiezMacierz();

                }

            }

            OdswiezMacierz();
            if (czyPokaz) Interaction.MsgBox("Macierz jest w postaci całkowicie zredukowanej.", MsgBoxStyle.OkOnly, "Eliminacja Gaussa zakończona");

        }

        private void ZmianaBazy(object obiekt, RoutedEventArgs argumenty)
        {

            bool czyPokaz = (bool)TrybPokazu.IsChecked;

            string tekst_z = Interaction.InputBox("Wprowadź bazę, z której chcesz przejść w formacie (1,2,3);(3,2,1).", "Baza B1", "(1,0,0);(1,1,0);(1,1,1)");
            if (tekst_z.Length == 0) return;

            string tekst_do = Interaction.InputBox("Wprowadź bazę, do której chcesz przejść w formacie (1,2,3);(3,2,1).", "Baza B2", "(1,1,7);(3,2,1);(0,0,1)");
            if (tekst_do.Length == 0) return;

            List<string[][]> temp_macierze_do_rozwiazania = new List<string[][]>();

            foreach (string wektor_z in tekst_z.Split(';'))
            {

                List<string[]> temp = new List<string[]>();

                foreach (string wektory_do in tekst_do.Split(';'))
                    temp.Add((from wektor in wektory_do.Replace("(", "").Replace(")", "").Split(',') select wektor).ToArray());

                temp.Add((from wektor in wektor_z.Replace("(", "").Replace(")", "").Split(',') select wektor).ToArray());

                temp_macierze_do_rozwiazania.Add(temp.ToArray());

                if (czyPokaz)
                    if (Interaction.MsgBox("Należy przedstawić wektor (" + wektor_z + ") jako kombinację wektorów bazy {" + tekst_do + "} za pomocą macierzy, której kolumnami są poszczególne wektory.\nCzy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Eliminacja Gaussa") == MsgBoxResult.No)
                        if (obiekt != null)
                            return;
                        else
                            throw new ConstraintException(bladPodalgorytmu);

            }

            if (tekst_z.Split(';').Count() != tekst_do.Split(';').Count())
                throw new ArgumentException("Bazy są z różnych przestrzeni.");

            List<Macierz> macierze_do_rozwiazania = (from macierz in temp_macierze_do_rozwiazania select Macierz.Transpozycja(new Macierz(macierz))).ToList();

            if (czyPokaz)
                if (Interaction.MsgBox("Należy rozwiązać " + macierze_do_rozwiazania.Count() + " macierz/e(-y) za pomocą całkowitej eliminacji.\nCzy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Eliminacja Gaussa") == MsgBoxResult.No)
                    if (obiekt != null)
                        return;
                    else
                        throw new ConstraintException(bladPodalgorytmu);

            List<Macierz> rozwiazane_macierze = new List<Macierz>();

            foreach (Macierz macierz_do_rozwiazania in macierze_do_rozwiazania)
            {

                macierz = macierz_do_rozwiazania;

                if (czyPokaz)
                    if (Interaction.MsgBox("Należy rozwiązać następującą macierz:\n" + macierz_do_rozwiazania.ToString() + "\nskładającą się z wektorów bazy docelowej i jednego z wektorów z bazy pierwotnej w ostatniej kolumnie.\nCzy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Eliminacja Gaussa") == MsgBoxResult.No)
                        if (obiekt != null)
                            return;
                        else
                            throw new ConstraintException(bladPodalgorytmu);

                EliminacjaGaussa(null, argumenty);

                rozwiazane_macierze.Add(macierz);

            }

            List<string[]> ret = new List<string[]>();

            foreach (Macierz rozwiazana_macierz in rozwiazane_macierze)
            {

                string[] wiersze = new string[rozwiazana_macierz.Wiersze()];

                for (int i = 0; i < rozwiazana_macierz.Wiersze(); i++)
                    wiersze[i] = rozwiazana_macierz.Tablica()[i, rozwiazana_macierz.Kolumny() - 1].ToString();

                ret.Add(wiersze);

            }

            macierz = Macierz.Transpozycja(new Macierz(ret.ToArray()));
            OdswiezMacierz();

            if (czyPokaz)
                Interaction.MsgBox("Wszystkie macierze zostały rozwiązane.\nUzyskane wyniki należy wstawić do kolumn nowej macierzy, która będzie macierzą przejścia z bazy B1 do B2.", MsgBoxStyle.OkOnly, "Macierz utworzona - koniec pracy");

        }

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

                List<Macierz> macierze_przejscia = new List<Macierz>();

                for (int i = 0; i < wyniki.Length; i++)
                {

                    string[][] do_redukcji = new string[baza_do.Length + 1][];

                    for (int j = 0; j < baza_do.Length; j++)
                        do_redukcji[j] = baza_do[j].Split(',');

                    do_redukcji[baza_do.Length] = wyniki[i];

                    macierze_przejscia.Add(Macierz.Transpozycja(new Macierz(do_redukcji)));

                }

                List<Macierz> rozwiazane_macierze = new List<Macierz>();

                foreach (Macierz do_rozwiazania in macierze_przejscia)
                {

                    macierz = do_rozwiazania;
                    OdswiezMacierz();
                    if (czyPokaz)
                        if (Interaction.MsgBox("Należy rozwiązać poniższą macierz przejścia do bazy B2 za pomocą eliminacji:\n" + macierz.ToString() + "\nW kolumnach zostały wstawione poszczególne wektory bazy, a w ostatniej jeden z wektorów po przekształceniu liniowym. Czy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Przekształcenie liniowe") == MsgBoxResult.No)
                            if (obiekt != null)
                                return;
                            else
                                throw new ConstraintException(bladPodalgorytmu);

                    EliminacjaGaussa(null, argumenty);

                    rozwiazane_macierze.Add(macierz);

                }

                List<string[]> ret = new List<string[]>();

                foreach (Macierz rozwiazana_macierz in rozwiazane_macierze)
                {

                    string[] wiersze = new string[rozwiazana_macierz.Wiersze()];

                    for (int i = 0; i < rozwiazana_macierz.Wiersze(); i++)
                        wiersze[i] = rozwiazana_macierz.Tablica()[i, rozwiazana_macierz.Kolumny() - 1].ToString();

                    ret.Add(wiersze);

                }

                macierz = Macierz.Transpozycja(new Macierz(ret.ToArray()));
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

}
