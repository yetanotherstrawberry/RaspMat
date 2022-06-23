using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;

namespace Arytmetyka
{

    public class Macierz
    {

        [JsonProperty(Required = Required.Always)]
        private readonly Ulamek[,] macierz;

        public Ulamek[,] Tablica() => macierz;
        public long Wiersze() => Tablica().GetLength(0);
        public long Kolumny() => Tablica().GetLength(1);

        [JsonConstructor]
        private Macierz()
        {

        }

        public Macierz(long wiersze, long kolumny)
        {

            macierz = new Ulamek[wiersze, kolumny];

            for (long i = 0; i < Wiersze(); i++)
                for (long j = 0; j < Kolumny(); j++)
                    macierz[i, j] = new Ulamek(0, 1);

        }

        public Macierz(string[][] wejscie)
        {

            macierz = new Ulamek[wejscie.Length, wejscie[0].Length];

            for (long i = 0; i < Wiersze(); i++)
                for (long j = 0; j < Kolumny(); j++)
                    macierz[i, j] = new Ulamek(wejscie[i][j]);

        }

        public static Macierz Transpozycja(Macierz macierz)
        {

            Macierz ret = new Macierz(macierz.Kolumny(), macierz.Wiersze());

            for (int i = 0; i < macierz.Wiersze(); i++)
                for (int j = 0; j < macierz.Kolumny(); j++)
                    ret.Ustaw(new Ulamek(macierz.Tablica()[i, j].Licznik(), macierz.Tablica()[i, j].Mianownik()), j, i);

            return ret;

        }

        public static Macierz Jednostkowa(long rozmiar)
        {

            Macierz ret = new Macierz(rozmiar, rozmiar);

            for (long i = 0; i < rozmiar; i++)
                ret.Ustaw(new Ulamek(1, 1), i, i);

            return ret;

        }

        public static Macierz MacierzZamianyWierszy(Macierz macierz, long zamienW1, long zamienW2)
        {

            Macierz ret = Jednostkowa(macierz.Wiersze());

            ret.Ustaw(new Ulamek(0, 1), zamienW1, zamienW1);
            ret.Ustaw(new Ulamek(0, 1), zamienW2, zamienW2);

            ret.Ustaw(new Ulamek(1, 1), zamienW1, zamienW2);
            ret.Ustaw(new Ulamek(1, 1), zamienW2, zamienW1);

            return ret;

        }

        public static Macierz MacierzZamianyKolumn(Macierz macierz, long zamienK1, long zamienK2)
        {

            Macierz ret = Jednostkowa(macierz.Kolumny());

            ret.Ustaw(new Ulamek(0, 1), zamienK1, zamienK1);
            ret.Ustaw(new Ulamek(0, 1), zamienK2, zamienK2);

            ret.Ustaw(new Ulamek(1, 1), zamienK2, zamienK1);
            ret.Ustaw(new Ulamek(1, 1), zamienK1, zamienK2);

            return ret;

        }

        public static Macierz MacierzDodawaniaWierszy(Macierz macierz, long do_czego, long co, Ulamek ile)
        {

            Macierz ret = Jednostkowa(macierz.Wiersze());

            ret.Ustaw(ile, do_czego, co);

            return ret;

        }

        public static Macierz MacierzDodawaniaWierszy(Macierz macierz, long do_czego, long co, string ile)
            => MacierzDodawaniaWierszy(macierz, do_czego, co, new Ulamek(ile));

        public static Macierz MacierzDodawaniaKolumn(Macierz macierz, long do_czego, long co, string ile)
        {

            Macierz ret = Jednostkowa(macierz.Kolumny());

            ret.Ustaw(new Ulamek(ile), co, do_czego);

            return ret;

        }

        public static Macierz MacierzMnozeniaWiersza(Macierz macierz, long ktory, Ulamek przez_co)
        {

            Macierz ret = Jednostkowa(macierz.Wiersze());

            ret.Ustaw(przez_co, ktory, ktory);

            return ret;

        }

        public static Macierz MacierzMnozeniaWiersza(Macierz macierz, long ktory, string przez_co)
            => MacierzMnozeniaWiersza(macierz, ktory, new Ulamek(przez_co));

        public static Macierz MacierzMnozeniaKolumny(Macierz macierz, long ktory, string przez_co)
        {

            Macierz ret = Jednostkowa(macierz.Kolumny());

            ret.Ustaw(new Ulamek(przez_co), ktory, ktory);

            return ret;

        }

        public static Macierz Dopisana(Macierz macierz, bool zPrawej)
        {

            if (macierz.Wiersze() != macierz.Kolumny())
                throw new ArgumentException("Macierz identyczności można dopisać tylko do macierzy kwadratowej.");

            Macierz ret = new Macierz(macierz.Wiersze(), macierz.Kolumny() * 2);

            if (zPrawej)
            {

                for (long i = 0; i < macierz.Wiersze(); i++)
                    for (long j = 0; j < macierz.Kolumny(); j++)
                        ret.Ustaw(new Ulamek(macierz.Tablica()[i, j].Licznik(), macierz.Tablica()[i, j].Mianownik()), i, j);

                for (long j = 0, i = macierz.Wiersze(); j < ret.Wiersze(); j++, i++)
                    ret.Ustaw(new Ulamek(1, 1), j, i);

            }
            else
            {

                for (long i = 0; i < macierz.Wiersze(); i++)
                    for (long j = 0, x = ret.Wiersze(); j < macierz.Kolumny(); j++, x++)
                        ret.Ustaw(new Ulamek(macierz.Tablica()[i, j].Licznik(), macierz.Tablica()[i, j].Mianownik()), i, x);

                for (long i = 0; i < ret.Wiersze(); i++)
                    ret.Ustaw(new Ulamek(1, 1), i, i);

            }

            return ret;

        }

        public static Macierz Obcieta(Macierz macierz, bool obetnijLewa)
        {

            if (macierz.Kolumny() % 2 != 0 || macierz.Wiersze() != (macierz.Kolumny() / 2))
                throw new ArgumentException("Nieprawidłowy kształt macierzy. Ilość kolumn musi być podzielna przez 2 i po podzieleniu być równa ilości wierszy. Czy na pewno do macierzy dopisano I?");

            Macierz ret = new Macierz(macierz.Wiersze(), macierz.Wiersze());

            if (obetnijLewa)
            {

                for (long i = macierz.Kolumny() / 2, x = 0; i < macierz.Kolumny(); i++, x++)
                    for (long j = 0; j < macierz.Wiersze(); j++)
                        ret.Ustaw(new Ulamek(macierz.Tablica()[j, i].Licznik(), macierz.Tablica()[j, i].Mianownik()), j, x);

            }
            else
            {

                for (long i = 0, x = 0; i < macierz.Kolumny() / 2; i++, x++)
                    for (long j = 0; j < macierz.Wiersze(); j++)
                        ret.Ustaw(new Ulamek(macierz.Tablica()[j, i].Licznik(), macierz.Tablica()[j, i].Mianownik()), j, x);

            }

            return ret;

        }

        private void Ustaw(Ulamek a, long wiersz, long kolumna) => macierz[wiersz, kolumna] = a;

        public static Macierz operator +(Macierz a, Macierz b)
        {

            if (a.Wiersze() != b.Wiersze() || a.Kolumny() != b.Kolumny())
                throw new ArgumentException("Nie można dodawać/odejmować macierzy o różnych rozmiarach!");

            Macierz ret = new Macierz(a.Wiersze(), b.Kolumny());

            for (long i = 0; i < a.Wiersze(); i++)
                for (long j = 0; j < a.Kolumny(); j++)
                    ret.Ustaw(a.Tablica()[i, j] + b.Tablica()[i, j], i, j);

            return ret;

        }

        public static Macierz operator *(Ulamek skalar, Macierz a)
        {

            Macierz temp = new Macierz(a.Wiersze(), a.Kolumny());

            for (long i = 0; i < a.Wiersze(); i++)
                for (long j = 0; j < a.Kolumny(); j++)
                    temp.Ustaw(a.Tablica()[i, j] * skalar, i, j);

            return temp;

        }

        public static Macierz operator *(Macierz a, Macierz b)
        {

            Macierz ret = new Macierz(a.Wiersze(), b.Kolumny());
            Ulamek temp;

            if (a.Kolumny() != b.Wiersze())
                throw new ArgumentException("Ilość kolumn w macierzy A nie jest równa ilości wierszy macierzy B.");

            for (long i = 0; i < ret.Wiersze(); i++)
                for (long j = 0; j < ret.Kolumny(); j++)
                {

                    temp = new Ulamek(0, 1);

                    for (long k = 0; k < a.Kolumny(); k++)
                        temp += a.Tablica()[i, k] * b.Tablica()[k, j];

                    ret.Ustaw(temp, i, j);

                }

            return ret;

        }

        public static Macierz operator -(Macierz a, Macierz b)
            => a + (new Ulamek(-1, 1) * b);

        public DataTable DataTable()
        {

            DataTable tabela = new DataTable();

            for (long i = 0; i < Kolumny(); i++)
                tabela.Columns.Add();

            for (long i = 0; i < Wiersze(); i++)
            {

                DataRow wiersz = tabela.NewRow();

                for (int j = 0; j < Kolumny(); j++)
                    wiersz[j] = Tablica()[i, j].ToString();

                tabela.Rows.Add(wiersz);

            }

            return tabela;

        }

        public static DataTable DataTablePusta(long wiersze, long kolumny, bool dodajZera = false)
        {

            DataTable tabela = new DataTable();

            for (long i = 0; i < kolumny; i++)
                tabela.Columns.Add();

            for (long i = 0; i < wiersze; i++)
            {

                DataRow wiersz = tabela.NewRow();

                for (int j = 0; j < kolumny; j++)
                    wiersz[j] = dodajZera ? "0" : "";

                tabela.Rows.Add(wiersz);

            }

            return tabela;

        }

        public override string ToString()
        {

            string ret = "";

            for (long i = 0; i < Wiersze(); i++)
            {

                for (long j = 0; j < Kolumny(); j++)
                    ret += Tablica()[i, j] + (j < Kolumny() - 1 ? "\t" : null);

                ret += "\n";

            }

            return ret;

        }

    }

    public class Ulamek
    {

        [JsonProperty(Required = Required.Always)]
        private long licznik, mianownik;

        public long Licznik() => licznik;
        public long Mianownik() => mianownik;

        [JsonConstructor]
        private Ulamek()
        {

        }

        public Ulamek(long licznik, long mianownik, bool czySkrocic = true)
        {

            if (mianownik == 0) throw new ArgumentException("Nie można dzielić przez 0.");
            else if (mianownik < 0)
            {

                mianownik *= -1;
                licznik *= -1;

            }

            this.licznik = licznik;
            this.mianownik = mianownik;

            if (czySkrocic) Skroc();

        }

        public Ulamek(string liczba) :
            this(long.Parse(liczba.Split('/')[0].Replace("(", "")), liczba.Split('/').Count() >= 2 ? long.Parse(liczba.Split('/')[1].Replace(")", "")) : 1L)
        {

            if (liczba.Split('/').Count() > 2)
                throw new ArgumentException("Ułamek nie spełnia żadnego z formatów: \"a/b\", \"(a/b)\", \"a\".");

        }

        public override string ToString()
            => licznik.ToString() + (mianownik != 1 ? '/' + mianownik.ToString() : null);

        public static long NWD(long a, long b)
        {

            if (a == 0) return b;
            else if (b == 0) return a;

            a = Math.Abs(a);
            b = Math.Abs(b);

            while(b != 0)
            {

                long c = a % b;
                a = b;
                b = c;

            }

            return a;

        }

        private Ulamek Skroc(bool czyZmienicUlamek = true)
        {

            long nwd = NWD(licznik, mianownik);

            if (czyZmienicUlamek)
            {

                licznik /= nwd;
                mianownik /= nwd;
                return this;

            }
            else
            {

                long licznik = this.licznik;
                long mianownik = this.mianownik;
                return new Ulamek(licznik / nwd, mianownik / nwd);

            }

        }

        public static long NWW(long a, long b) => Math.Abs(a * b) / NWD(a, b);

        public static Ulamek operator *(Ulamek a, Ulamek b)
            => new Ulamek(a.Licznik() * b.Licznik(), a.Mianownik() * b.Mianownik()).Skroc();

        public static Ulamek operator /(Ulamek a, Ulamek b)
            => (a * new Ulamek(b.Mianownik(), b.Licznik())).Skroc();

        public static Ulamek operator +(Ulamek a, Ulamek b)
        {

            long wspolny_mianownik = NWW(a.Mianownik(), b.Mianownik());

            Ulamek _a = new Ulamek(a.Licznik() * (wspolny_mianownik / a.Mianownik()), wspolny_mianownik, false);
            Ulamek _b = new Ulamek(b.Licznik() * (wspolny_mianownik / b.Mianownik()), wspolny_mianownik, false);

            return new Ulamek(_a.Licznik() + _b.Licznik(), _a.Mianownik()).Skroc();

        }

        public static Ulamek operator -(Ulamek a, Ulamek b)
            => (a + new Ulamek(b.Licznik() * -1, b.Mianownik())).Skroc();

        public override bool Equals(object obj)
            => obj is Ulamek ulamek && Mianownik() == ulamek.Mianownik() && Licznik() == ulamek.Licznik();

        public override int GetHashCode()
            => (int)((Licznik() % (int.MaxValue / 2)) + (Mianownik() % (int.MaxValue / 2)));

    }

}
