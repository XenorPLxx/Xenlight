using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Xenlight
{
    class kontrolaKolorow
    {
        #region zmienne
        private Collection<Color> kolory = new Collection<Color>();
        private Collection<Color> kolory_Poprzednie = new Collection<Color>();
        private Collection<Color> koloryZamienione = new Collection<Color>();
        private DxScreenCapture sc = new DxScreenCapture();
        private Dx11ScreenCapture sc11 = new Dx11ScreenCapture();
        private int[] wskaznikWzrostuKoloru = new int[3];
        private int wskaznikKoloru = 0;
        private int liczbaMonitorow = 1;
        #endregion
        #region GS
        public Collection<Color> kolory_GS
        {
            get { return kolory; }
        }
        public Collection<Color> kolory_Poprzednie_GS
        {
            get { return kolory_Poprzednie; }
        }
        #endregion
        #region tryby
        public void tryb_Podstawowy(Collection<Collection<long>> pozycje)
        {
            sprawdzLiczbeMonitorow();
            Surface s = sc.CaptureScreen();
            GraphicsStream gs = s.LockRectangle(LockFlags.None);
            kolory.Clear();
            for (int k = 0; k < pozycje.Count; k++)
            {
                kolory.Add(avcs(gs, pozycje[k]));
            }
        }
        public void tryb_Podstawowy_11(Collection<Collection<long>> pozycje)
        {
            SharpDX.DataStream gs = sc11.CaptureScreen();
            kolory.Clear();
            for (int k = 0; k < pozycje.Count; k++)
            {
                kolory.Add(avcs(gs, pozycje[k]));
            }
            sc11.dataStream.Close();
        }
        public void wszystko_Czarne(int ilosc)
        {
            kolory.Clear();
            for (int i = 0; i < ilosc; i++)
            {
                kolory.Add(Color.Black);
            }
        }
        public void tryb_Plynne_Calosc(int liczba)
        {
            Color kolor;
            if (kolory.Count > 0)
            {
                kolor = kolory[0];
            }
            else
            {
                kolor = Color.Black;
            }
            kolory.Clear();

            if ((kolor.R == 0) && (kolor.G == 0) && (kolor.B == 0))
            {
                wskaznikWzrostuKoloru[0] = 1;
                wskaznikWzrostuKoloru[1] = 0;
                wskaznikWzrostuKoloru[2] = 0;
            }
            if ((kolor.R == 255) && (kolor.G == 0) && (kolor.B == 0))
            {
                wskaznikWzrostuKoloru[0] = 0;
                wskaznikWzrostuKoloru[1] = 1;
                wskaznikWzrostuKoloru[2] = 0;
            }
            if ((kolor.R == 255) && (kolor.G == 255) && (kolor.B == 0))
            {
                wskaznikWzrostuKoloru[0] = 2;
                wskaznikWzrostuKoloru[1] = 0;
                wskaznikWzrostuKoloru[2] = 0;
            }
            if ((kolor.R == 0) && (kolor.G == 255) && (kolor.B == 0))
            {
                wskaznikWzrostuKoloru[0] = 0;
                wskaznikWzrostuKoloru[1] = 0;
                wskaznikWzrostuKoloru[2] = 1;
            }
            if ((kolor.R == 0) && (kolor.G == 255) && (kolor.B == 255))
            {
                wskaznikWzrostuKoloru[0] = 0;
                wskaznikWzrostuKoloru[1] = 2;
                wskaznikWzrostuKoloru[2] = 0;
            }
            if ((kolor.R == 0) && (kolor.G == 0) && (kolor.B == 255))
            {
                wskaznikWzrostuKoloru[0] = 1;
                wskaznikWzrostuKoloru[1] = 0;
                wskaznikWzrostuKoloru[2] = 0;
            }
            if ((kolor.R == 255) && (kolor.G == 0) && (kolor.B == 255))
            {
                wskaznikWzrostuKoloru[0] = 0;
                wskaznikWzrostuKoloru[1] = 0;
                wskaznikWzrostuKoloru[2] = 2;
            }

            if (wskaznikWzrostuKoloru[0] == 1)
            {
                kolor = Color.FromArgb(kolor.R + 1, kolor.G, kolor.B);
            }
            else if (wskaznikWzrostuKoloru[0] == 2)
            {
                kolor = Color.FromArgb(kolor.R - 1, kolor.G, kolor.B);
            }

            if (wskaznikWzrostuKoloru[1] == 1)
            {
                kolor = Color.FromArgb(kolor.R, kolor.G + 1, kolor.B);
            }
            else if (wskaznikWzrostuKoloru[1] == 2)
            {
                kolor = Color.FromArgb(kolor.R, kolor.G - 1, kolor.B);
            }

            if (wskaznikWzrostuKoloru[2] == 1)
            {
                kolor = Color.FromArgb(kolor.R, kolor.G, kolor.B + 1);
            }
            else if (wskaznikWzrostuKoloru[2] == 2)
            {
                kolor = Color.FromArgb(kolor.R, kolor.G, kolor.B - 1);
            }
            for (int i = 0; i < liczba; i++)
            {
                kolory.Add(kolor);
            }
        }
        public void tryb_Choinka(int liczba)
        {
            kolory.Clear();
                for (int i = 0; i < liczba; i++)
                {
                    if (i % 3 == wskaznikKoloru)
                    {
                        if (wskaznikKoloru == 0) kolory.Add(Color.FromArgb(255,0,0));
                        else if (wskaznikKoloru == 1) kolory.Add(Color.FromArgb(0, 255, 0));
                        else if (wskaznikKoloru == 2) kolory.Add(Color.FromArgb(0, 0, 255));
                    }
                    else
                    {
                        kolory.Add(Color.Black);
                    }
                }

                if (wskaznikKoloru == 0) wskaznikKoloru = 1;
                else if (wskaznikKoloru == 1) wskaznikKoloru = 2;
                else if (wskaznikKoloru == 2) wskaznikKoloru = 0;
        }
        public void tryb_Manualny_Reczny(Collection<Panel> panele)
        {
            kolory.Clear();
            for (int i = 0; i < panele.Count; i++)
            {
                kolory.Add(panele[i].BackColor);
            }
        }
        public void tryb_Manualny_Duplikat(Color kolor, int liczba)
        {
            kolory.Clear();
            for (int i = 0; i < liczba; i++)
            {
                kolory.Add(kolor);
            }
        }

        #endregion
        #region tryby_-_funkcje_pomocnicze
        Color avcs(GraphicsStream gs, Collection<long> positions)
        {
            byte[] bu = new byte[4];
            int r = 0;
            int g = 0;
            int b = 0;
            int i = 0;

            foreach (long pos in positions)
            {
                gs.Position = pos;
                if (gs.Length > positions[positions.Count - 1])
                if (gs.CanRead) try { gs.Read(bu, 0, 4); }
                    catch { }
                r += bu[2];
                g += bu[1];
                b += bu[0];
                i++;
            }
            if (i == 0)
                return Color.Black;
            else
            {
                int r_Usr = r / i;
                int g_Usr = g / i;
                int b_Usr = b / i;
                if (r_Usr == 0xAA)
                    r_Usr += 1;
                if (g_Usr == 0xAA)
                    g_Usr += 1;
                if (b_Usr == 0xAA)
                    b_Usr += 1;
                return Color.FromArgb(r_Usr, g_Usr, b_Usr);
            }
        }
        Color avcs(SharpDX.DataStream gs, Collection<long> positions)
        {
            byte[] bu = new byte[4];
            int r = 0;
            int g = 0;
            int b = 0;
            int i = 0;

            foreach (long pos in positions)
            {
                gs.Position = pos;
                if (gs.Length > positions[positions.Count - 1])
                    if (gs.CanRead) try { gs.Read(bu, 0, 4); }
                        catch { }
                r += bu[2];
                g += bu[1];
                b += bu[0];
                i++;
            }
            if (i == 0)
                return Color.Black;
            else
            {
                int r_Usr = r / i;
                int g_Usr = g / i;
                int b_Usr = b / i;
                if (r_Usr == 0xAA)
                    r_Usr += 1;
                if (g_Usr == 0xAA)
                    g_Usr += 1;
                if (b_Usr == 0xAA)
                    b_Usr += 1;
                return Color.FromArgb(r_Usr, g_Usr, b_Usr);
            }
        }
           
        private void sprawdzLiczbeMonitorow()
        {
            if (liczbaMonitorow != System.Windows.Forms.SystemInformation.MonitorCount)
            {
                sc = new DxScreenCapture();
                sc11 = new Dx11ScreenCapture();
                liczbaMonitorow = System.Windows.Forms.SystemInformation.MonitorCount;
            }
        }
        public void kalibracja_RGB(int r, int g, int b)
        {
            for (int i = 0; i < kolory.Count; i++)
            {
                double r_temp_d = (double)kolory[i].R * (((double)r + 100) / 100);
                double g_temp_d = (double)kolory[i].G * (((double)g + 100) / 100);
                double b_temp_d = (double)kolory[i].B * (((double)b + 100) / 100);
                int r_temp = (int)r_temp_d;
                int g_temp = (int)g_temp_d;
                int b_temp = (int)b_temp_d;
                if (r_temp > 255) r_temp = 255;
                if (g_temp > 255) g_temp = 255;
                if (b_temp > 255) b_temp = 255;
                if (r_temp < 0) r_temp = 0;
                if (g_temp < 0) g_temp = 0;
                if (b_temp < 0) b_temp = 0;
                kolory[i] = Color.FromArgb(r_temp, g_temp, b_temp);
            }
        }
        #endregion
        #region wysylanie
        private void zamiana_Stron()
        {
            koloryZamienione.Clear();
            for (int i = 0; i < 14; i++)
            {
                koloryZamienione.Add(Color.Black);
            }
            koloryZamienione[0] = kolory[4];
            koloryZamienione[1] = kolory[3];
            koloryZamienione[2] = kolory[2];
            koloryZamienione[3] = kolory[1];
            koloryZamienione[4] = kolory[0];
            koloryZamienione[5] = kolory[13];
            koloryZamienione[13] = kolory[5];
            koloryZamienione[6] = kolory[12];
            koloryZamienione[12] = kolory[6];
            koloryZamienione[7] = kolory[11];
            koloryZamienione[8] = kolory[10];
            koloryZamienione[9] = kolory[9];
            koloryZamienione[10] = kolory[8];
            koloryZamienione[11] = kolory[7];

            for (int i = 0; i < kolory.Count; i++)
                kolory[i] = koloryZamienione[i];
        } //!!!
        private void przepisywanie_Poprzednich_Kolorow()
        {
            kolory_Poprzednie.Clear();
            for (int k = 0; k < kolory.Count; k++)
            {
                kolory_Poprzednie.Add(kolory[k]);
            }
        }
        public Collection<Tuple<Color, int>> kolory_Do_Wyslania()
        {
            if (kolory.Count == 14)
            {
                zamiana_Stron();
            }
            Collection<Tuple<Color, int>> do_Wyslania = new Collection<Tuple<Color, int>>();
            if (kolory_Poprzednie.Count < kolory.Count)
                for (int i = kolory_Poprzednie.Count; i < kolory.Count; i++)
                    kolory_Poprzednie.Add(Color.Black);
            for (int i = 0; i < kolory.Count; i++)
            {
                if (kolory[i] != kolory_Poprzednie[i])
                    do_Wyslania.Add(new Tuple<Color, int>(kolory[i], i));
            }
            przepisywanie_Poprzednich_Kolorow();
            return do_Wyslania;
        }
        #endregion
    }
}
