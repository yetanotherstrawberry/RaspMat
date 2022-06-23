using Arytmetyka;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AlgebraProjekt
{

    public partial class MainWindow : Window
    {

        private const string guzikWprowadzanieTekt = "Wprowadź macierz", zerowaGuzikTekst = "Edytuj zerową", bladWyznacznik = "Wyznacznik macierzy jest równy 0, zatem nie można jej odwrócić.";

        private Macierz macierz;
        private DataTable wprowadzanie = null;

        private void Blad(string tekst)
        {

            TextBlock blad = new TextBlock
            {

                Text = "Wystąpił błąd/wyjątek:" + Environment.NewLine + tekst,
                TextWrapping = TextWrapping.WrapWithOverflow,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center

            };

            if (macierz != null)
            {

                Button odzyskiwanie = new Button
                {
                    Content = "Pokaż ostatnią macierz",
                    Width = 150,
                    Height = 20,
                    Margin = new Thickness(0, 2, 0, 0)
                };

                odzyskiwanie.Click += Odzyskiwanie;

                StackPanel panel = new StackPanel
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                panel.Children.Add(blad);
                panel.Children.Add(odzyskiwanie);

                okienko.Children.Clear();
                okienko.Children.Add(panel);

            }
            else
            {

                okienko.Children.Clear();
                okienko.Children.Add(blad);

            }

            ResetujInterface();

        }

        private void ResetujInterface()
        {

            ZablokujInterface();
            WprowadzGuzik.Content = guzikWprowadzanieTekt;
            ZerowaGuzik.Content = zerowaGuzikTekst;
            wprowadzanie = null;
            WprowadzGuzik.IsEnabled = true;
            ZerowaGuzik.IsEnabled = true;

        }

        private void Odzyskiwanie(object obiekt, RoutedEventArgs argumenty)
        {

            OdblokujInterface();
            OdswiezMacierz();

        }

        private void ZablokujInterface()
        {

            GuzikEksport.IsEnabled = false;
            Operacje.IsEnabled = false;
            foreach (Button guzik in Algorytmy.Children)
                if (guzik.Name != "ZmianaBazyGuzik" && guzik.Name != "PrzeksztalcenieGuzik")
                    guzik.IsEnabled = false;

        }

        private void OdblokujInterface()
        {

            GuzikEksport.IsEnabled = true;
            Operacje.IsEnabled = true;
            foreach (Button guzik in Algorytmy.Children)
                guzik.IsEnabled = true;

        }

        private void NowyDataGrid(long wiersze, long kolumny, bool czyDodawanie, bool dodajZera = false)
        {

            try
            {

                if (czyDodawanie) wprowadzanie = Macierz.DataTablePusta(wiersze, kolumny, dodajZera);
                else wprowadzanie = null;

                okienko.Children.Clear();
                okienko.Children.Add(new DataGrid
                {
                    ColumnWidth = DataGridLength.SizeToCells,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HeadersVisibility = DataGridHeadersVisibility.None,
                    IsEnabled = czyDodawanie,
                    CanUserAddRows = false,
                    ItemsSource = czyDodawanie ? wprowadzanie.DefaultView : macierz.DataTable().DefaultView,
                    FontSize = 20
                });

            }
            catch (Exception wyjatek)
            {

                if (wyjatek is ArgumentNullException || wyjatek is ArgumentException || wyjatek is Win32Exception || wyjatek is FormatException || wyjatek is JsonSerializationException)
                    Blad(wyjatek.Message);
                else
                    throw wyjatek;

            }

        }

        private void OdswiezMacierz()
        {

            NowyDataGrid(macierz.Wiersze(), macierz.Kolumny(), false);
            OdblokujInterface();

        }

        private void NowaMacierz(object obiekt, RoutedEventArgs argumenty)
        {

            try
            {

                if (wprowadzanie == null)
                {

                    string tekst = Interaction.InputBox("Wprowadź rozmiar macierzy w formacie WIERSZExKOLUMNY.", "Rozmiar macierzy", "3x3");

                    if (tekst.Length > 0)
                    {

                        long[] rozmiar = tekst.Split('x').Select(x => long.Parse(x)).ToArray();
                        NowyDataGrid(rozmiar[0], rozmiar[1], true);

                        ZablokujInterface();
                        ZerowaGuzik.IsEnabled = false;
                        WprowadzGuzik.Content = "Zatwierdź";

                    }

                }
                else
                {

                    string[][] wejscie = wprowadzanie.AsEnumerable().Select(x => (from element in x.ItemArray select element.ToString()).ToArray()).ToArray();
                    macierz = new Macierz(wejscie);
                    wprowadzanie = null;
                    OdswiezMacierz();
                    ZerowaGuzik.IsEnabled = true;
                    WprowadzGuzik.Content = guzikWprowadzanieTekt;

                }

            }
            catch (Exception wyjatek)
            {

                if (wyjatek is ArgumentNullException || wyjatek is ArgumentException || wyjatek is Win32Exception || wyjatek is FormatException || wyjatek is JsonSerializationException)
                    Blad(wyjatek.Message);
                else
                    throw wyjatek;

            }

        }

        private void NowaZerowa(object obiekt, RoutedEventArgs argumenty)
        {

            try
            {

                if (wprowadzanie == null)
                {

                    string tekst = Interaction.InputBox("Wprowadź rozmiar macierzy zerowej w formacie WIERSZExKOLUMNY.", "Rozmiar macierzy", "3x3");

                    if (tekst.Length > 0)
                    {

                        long[] rozmiar = tekst.Split('x').Select(x => long.Parse(x)).ToArray();
                        NowyDataGrid(rozmiar[0], rozmiar[1], true, true);

                        ZablokujInterface();
                        WprowadzGuzik.IsEnabled = false;
                        ZerowaGuzik.Content = "Zatwierdź";

                    }

                }
                else
                {

                    string[][] wejscie = wprowadzanie.AsEnumerable().Select(x => (from element in x.ItemArray select element.ToString()).ToArray()).ToArray();
                    macierz = new Macierz(wejscie);
                    wprowadzanie = null;
                    OdswiezMacierz();
                    WprowadzGuzik.IsEnabled = true;
                    ZerowaGuzik.Content = zerowaGuzikTekst;

                }

            }
            catch (Exception wyjatek)
            {

                if (wyjatek is ArgumentNullException || wyjatek is ArgumentException || wyjatek is FormatException)
                    Blad(wyjatek.Message);
                else
                    throw wyjatek;

            }

        }

        public MainWindow()
        {

            InitializeComponent();
            ZablokujInterface();

        }

        private void ZamianaWierszy(object obiekt, RoutedEventArgs argumenty)
        {

            try
            {

                macierz = Macierz.MacierzZamianyWierszy(macierz, long.Parse(ZamianaA.Text), long.Parse(ZamianaB.Text)) * macierz;
                OdswiezMacierz();

            }
            catch (Exception wyjatek)
            {

                if (wyjatek is ArgumentNullException || wyjatek is ArgumentException || wyjatek is FormatException)
                    Blad(wyjatek.Message);
                else
                    throw wyjatek;

            }

        }

        private void ZamianaKolumn(object obiekt, RoutedEventArgs argumenty)
        {

            try
            {

                macierz *= Macierz.MacierzZamianyKolumn(macierz, long.Parse(ZamianaA.Text), long.Parse(ZamianaB.Text));
                OdswiezMacierz();

            }
            catch (Exception wyjatek)
            {

                if (wyjatek is ArgumentNullException || wyjatek is ArgumentException || wyjatek is FormatException)
                    Blad(wyjatek.Message);
                else
                    throw wyjatek;

            }

        }

        private void DodawanieWierszy(object obiekt, RoutedEventArgs argumenty)
        {

            try
            {

                macierz = Macierz.MacierzDodawaniaWierszy(macierz, long.Parse(DodawanieX.Text), long.Parse(DodawanieY.Text), DodawanieSkalar.Text) * macierz;
                OdswiezMacierz();

            }
            catch (Exception wyjatek)
            {

                if (wyjatek is ArgumentNullException || wyjatek is ArgumentException || wyjatek is FormatException)
                    Blad(wyjatek.Message);
                else
                    throw wyjatek;

            }

        }

        private void DodawanieKolumn(object obiekt, RoutedEventArgs argumenty)
        {

            try
            {

                macierz *= Macierz.MacierzDodawaniaKolumn(macierz, long.Parse(DodawanieX.Text), long.Parse(DodawanieY.Text), DodawanieSkalar.Text);
                OdswiezMacierz();

            }
            catch (Exception wyjatek)
            {

                if (wyjatek is ArgumentNullException || wyjatek is ArgumentException || wyjatek is FormatException)
                    Blad(wyjatek.Message);
                else
                    throw wyjatek;

            }

        }

        private void MnozenieMacierzy(object obiekt, RoutedEventArgs argumenty)
        {

            try
            {

                macierz = new Ulamek(SkalarMacierzy.Text) * macierz;
                OdswiezMacierz();

            }
            catch (Exception wyjatek)
            {

                if (wyjatek is ArgumentNullException || wyjatek is ArgumentException || wyjatek is FormatException)
                    Blad(wyjatek.Message);
                else
                    throw wyjatek;

            }

        }

        private void MnozenieWiersza(object obiekt, RoutedEventArgs argumenty)
        {

            try
            {

                macierz = Macierz.MacierzMnozeniaWiersza(macierz, long.Parse(MnozenieWiersz.Text), SkalarWiersza.Text) * macierz;
                OdswiezMacierz();

            }
            catch (Exception wyjatek)
            {

                if (wyjatek is ArgumentNullException || wyjatek is ArgumentException || wyjatek is FormatException)
                    Blad(wyjatek.Message);
                else
                    throw wyjatek;

            }

        }

        private void MnozenieKolumny(object obiekt, RoutedEventArgs argumenty)
        {

            try
            {

                macierz *= Macierz.MacierzMnozeniaKolumny(macierz, long.Parse(MnozenieWiersz.Text), SkalarWiersza.Text);
                OdswiezMacierz();

            }
            catch (Exception wyjatek)
            {

                if (wyjatek is ArgumentNullException || wyjatek is ArgumentException || wyjatek is FormatException)
                    Blad(wyjatek.Message);
                else
                    throw wyjatek;

            }

        }

        private void EksportujMacierz(object obiekt, RoutedEventArgs argumenty)
        {

            try
            {

                SaveFileDialog dialog = new SaveFileDialog
                {
                    Filter = "Macierz w pliku JSON | *.json"
                };

                bool? wynik = dialog.ShowDialog();

                if (wynik == true && dialog.FileName != null)
                {

                    string json = JsonConvert.SerializeObject(macierz);
                    File.WriteAllText(dialog.FileName, json);

                }

            }
            catch (Exception wyjatek)
            {

                if (wyjatek is ArgumentNullException || wyjatek is ArgumentException || wyjatek is Win32Exception || wyjatek is FormatException || wyjatek is JsonSerializationException)
                    Blad(wyjatek.Message);
                else
                    throw wyjatek;

            }

        }

        private void ImportujMacierz(object obiekt, RoutedEventArgs argumenty)
        {

            try
            {

                OpenFileDialog dialog = new OpenFileDialog
                {
                    Filter = "Macierz w pliku JSON | *.json",
                    Multiselect = false
                };

                bool? wynik = dialog.ShowDialog();

                if (wynik == true && dialog.FileNames.Single() != null)
                {

                    macierz = JsonConvert.DeserializeObject<Macierz>(File.ReadAllText(dialog.FileNames.Single()));
                    OdswiezMacierz();

                    ZerowaGuzik.IsEnabled = true;
                    WprowadzGuzik.Content = guzikWprowadzanieTekt;
                    WprowadzGuzik.IsEnabled = true;
                    ZerowaGuzik.Content = zerowaGuzikTekst;

                }

            }
            catch (Exception wyjatek)
            {

                if (wyjatek is ArgumentNullException || wyjatek is ArgumentException || wyjatek is Win32Exception || wyjatek is FormatException || wyjatek is JsonSerializationException)
                    Blad(wyjatek.Message);
                else
                    throw wyjatek;

            }

        }

        private void Odwrotnosc(object obiekt, RoutedEventArgs argumenty)
        {

            try
            {

                bool czyPokaz = (bool)TrybPokazu.IsChecked;

                if (czyPokaz)
                    if (Interaction.MsgBox("Dopisz I z prawej strony macierzy.\nCzy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Odwracanie macierzy") == MsgBoxResult.No)
                        return;
                macierz = Macierz.Dopisana(macierz, true);
                OdswiezMacierz();

                if (czyPokaz)
                    if (Interaction.MsgBox("Wykonaj całkowitą redukcję.\nCzy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Odwracanie macierzy") == MsgBoxResult.No)
                        return;
                EliminacjaGaussa(null, argumenty);
                OdswiezMacierz();

                for (long h = 0; h < macierz.Kolumny(); h++)
                {

                    bool czySameZera = true;

                    for (long j = 0; j < macierz.Wiersze(); j++)
                        if (macierz.Tablica()[j, h].Licznik() != 0)
                        {
                            czySameZera = false;
                            break;
                        }

                    if (czySameZera)
                        throw new ArgumentException(bladWyznacznik);

                }

                for (long h = 0; h < macierz.Wiersze(); h++)
                {

                    bool czySameZera = true;

                    for (long j = 0; j < macierz.Kolumny() / 2; j++)
                        if (macierz.Tablica()[h, j].Licznik() != 0)
                        {
                            czySameZera = false;
                            break;
                        }

                    if (czySameZera)
                        throw new ArgumentException(bladWyznacznik);

                }

                if (czyPokaz)
                    if (Interaction.MsgBox("Usuń I z lewej strony.\nCzy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Odwracanie macierzy") == MsgBoxResult.No)
                        return;
                macierz = Macierz.Obcieta(macierz, true);
                OdswiezMacierz();

                if (czyPokaz)
                    Interaction.MsgBox("Macierz została odwrócona.", MsgBoxStyle.OkOnly, "Koniec pracy algorytmu");

            }
            catch (Exception wyjatek)
            {

                if (wyjatek is ArgumentNullException || wyjatek is ArgumentException || wyjatek is FormatException)
                    Blad(wyjatek.Message);
                else if (wyjatek is ConstraintException)
                    return;
                else
                    throw wyjatek;

            }

        }

        private void Licencje(object obiekt, RoutedEventArgs argumenty)
        {

            MessageBox.Show(Properties.Resources.ParserLic, "Licencje autorów trzecich i programu");

        }

        private void DopiszILewo(object obiekt, RoutedEventArgs argumenty)
        {

            try
            {

                macierz = Macierz.Dopisana(macierz, false);
                OdswiezMacierz();

            }
            catch (Exception wyjatek)
            {

                if (wyjatek is ArgumentNullException || wyjatek is ArgumentException || wyjatek is FormatException)
                    Blad(wyjatek.Message);
                else
                    throw wyjatek;

            }

        }

        private void UsunLewo(object obiekt, RoutedEventArgs argumenty)
        {

            try
            {

                macierz = Macierz.Obcieta(macierz, true);
                OdswiezMacierz();

            }
            catch (Exception wyjatek)
            {

                if (wyjatek is ArgumentNullException || wyjatek is ArgumentException || wyjatek is FormatException)
                    Blad(wyjatek.Message);
                else
                    throw wyjatek;

            }

        }

        private void DopiszIPrawo(object obiekt, RoutedEventArgs argumenty)
        {

            try
            {

                macierz = Macierz.Dopisana(macierz, true);
                OdswiezMacierz();

            }
            catch (Exception wyjatek)
            {

                if (wyjatek is ArgumentNullException || wyjatek is ArgumentException || wyjatek is FormatException)
                    Blad(wyjatek.Message);
                else
                    throw wyjatek;

            }

        }

        private void UsunPrawo(object obiekt, RoutedEventArgs argumenty)
        {

            try
            {

                macierz = Macierz.Obcieta(macierz, false);
                OdswiezMacierz();

            }
            catch (Exception wyjatek)
            {

                if (wyjatek is ArgumentNullException || wyjatek is ArgumentException || wyjatek is FormatException)
                    Blad(wyjatek.Message);
                else
                    throw wyjatek;

            }

        }

        private void Transpozycja(object obiekt, RoutedEventArgs argumenty)
        {

            try
            {

                macierz = Macierz.Transpozycja(macierz);
                OdswiezMacierz();

            }
            catch (Exception wyjatek)
            {

                if (wyjatek is ArgumentNullException || wyjatek is ArgumentException || wyjatek is FormatException)
                    Blad(wyjatek.Message);
                else
                    throw wyjatek;

            }

        }

    }

}
