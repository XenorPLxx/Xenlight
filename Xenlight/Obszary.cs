using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

namespace Xenlight
{
    public partial class Obszary : Form
    {
        #region zmienne
        protected const int pixel = 4;

        protected int x, y;

        protected int przerwa_Pion, przerwa_Poziom;
        protected int obszary_Pion, obszary_Poziom;
        protected int liczba_Obszarow;

        protected int szerokosc_Mala, szerokosc_Duza, wysokosc_Mala, wysokosc_Duza;
        protected int wysokosc_Czarnych_Pasow, rozmiar_Procentowy, wysokosc_Pasa;

        public Collection<Collection<long>> pozycje = new Collection<Collection<long>>();
        protected Collection<Panel> panele = new Collection<Panel>();
        #endregion
        public Obszary(int przerwa_Pion, int przerwa_Poziom, int obszary_Pion, int obszary_Poziom, int rozmiar_Procentowy, int wysokosc_Czarnych_Pasow)
        {
            InitializeComponent();

            x = Screen.PrimaryScreen.Bounds.Width;
            y = Screen.PrimaryScreen.Bounds.Height;

            aktualizuj_Rozmiar();

            this.przerwa_Pion = przerwa_Pion;
            this.przerwa_Poziom = przerwa_Poziom;
            this.obszary_Pion = obszary_Pion;
            this.obszary_Poziom = obszary_Poziom;
            this.rozmiar_Procentowy = rozmiar_Procentowy;
            this.wysokosc_Czarnych_Pasow = y * wysokosc_Czarnych_Pasow / 100;
            wysokosc_Pasa = wysokosc_Czarnych_Pasow / 2;
            liczba_Obszarow = obszary_Poziom * 2 + obszary_Pion * 2 - 2 * 2;
            this.aktualizuj_Obszary();
        }
        #region GS
        public int przerwa_Pion_GS
        {
            get
            {
                return przerwa_Pion;
            }
            set
            {
                przerwa_Pion = value;
                this.aktualizuj_Obszary();
            }
        }
        public int przerwa_Poziom_GS
        {
            get
            {
                return przerwa_Poziom;
            }
            set
            {
                przerwa_Poziom = value;
                this.aktualizuj_Obszary();
            }
        }
        public int obszary_Pion_GS
        {
            get
            {
                return obszary_Pion;
            }
            set
            {
                obszary_Pion = value;
                liczba_Obszarow = obszary_Poziom * 2 + obszary_Pion * 2 - 2 * 2;
                this.aktualizuj_Obszary();
            }
        }
        public int obszary_Poziom_GS
        {
            get
            {
                return obszary_Poziom;
            }
            set
            {
                obszary_Poziom = value;
                liczba_Obszarow = obszary_Poziom * 2 + obszary_Pion * 2 - 2 * 2;
                this.aktualizuj_Obszary();
            }
        }
        public int rozmiar_Procentowy_GS
        {
            set
            {
                rozmiar_Procentowy = value;
                this.aktualizuj_Obszary();
            }
        }
        public int wysokosc_Czarnych_Pasow_GS
        {
            set
            {
                this.wysokosc_Czarnych_Pasow = value * y / 100;
                wysokosc_Pasa = wysokosc_Czarnych_Pasow / 2;
                this.aktualizuj_Obszary();
            }
        }
        public int[] szerokosci_GS
        {
            get 
            { 
                int[] temp = {szerokosc_Duza, szerokosc_Mala};
                return temp;
            }
        }
        public int[] wysokosci_GS
        {
            get 
            {
                int[] temp = { wysokosc_Duza, wysokosc_Mala };
                return temp;
            }
        }
        public int[] pixele_GS
        {
            get
            {
                int[] temp = { pozycje[0].Count, pozycje[1].Count, pozycje[obszary_Poziom].Count };
                return temp;
            }
        }
        public Collection<Panel> panele_GS
        {
            get
            {
                return panele;
            }
        }
        #endregion
        protected void aktualizuj_Label()
        {
            label1.Width = this.ClientSize.Width - panele[0].Width * 2-20;
            label1.Height = this.ClientSize.Height - panele[0].Height * 2-20;
            label1.Location = new Point(panele[0].Width+10, panele[0].Height+10);
        }
        private void aktualizuj_Obszary()
        {
            x = Screen.PrimaryScreen.Bounds.Width;
            y = Screen.PrimaryScreen.Bounds.Height;

            aktualizuj_Rozmiar();

            int wysokosc_Bez_Pasow = y - wysokosc_Czarnych_Pasow;
            wysokosc_Duza = wysokosc_Bez_Pasow / obszary_Pion;
            szerokosc_Duza = x / obszary_Poziom;
            wysokosc_Mala = rozmiar_Procentowy * wysokosc_Duza / 100;
            szerokosc_Mala = rozmiar_Procentowy * szerokosc_Duza / 100;

            aktualizuj_Pozycje();
            aktualizuj_Label();
        }
        protected virtual void Obszary_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }
        protected void Obszary_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible == true)
            {
                aktualizuj_Pozycje();
                aktualizuj_Label();
            }
        }
        protected void aktualizuj_Pozycje()
        {
            if (panele.Count != liczba_Obszarow)
            {
                for (int i = 0; i < panele.Count; i++)
                {
                    Controls.Remove(panele[i]);
                }
                panele.Clear();
                for (int i = 0; i < liczba_Obszarow; i++)
                {
                    panele.Add(new Panel());
                }
            }
            pozycje.Clear();

            for (int i = 0; i < liczba_Obszarow; i++)
            {
                pozycje.Add(new Collection<long>());
            }

            #region gorne
            for (int i = 0; i < obszary_Poziom; i++)
            {
                if ((i > 0) && (i < (obszary_Poziom - 1)))
                {
                    licz_Pozycje_Dla_Obszaru(i * szerokosc_Duza, wysokosc_Pasa, 1, i);
                }
                else
                {
                    licz_Pozycje_Dla_Obszaru(i * szerokosc_Duza, wysokosc_Pasa, 0, i);
                }
            }
            #endregion

            #region dolne
            int lewy_dolny = obszary_Poziom * 2 + obszary_Pion - 2;
            for (int i = 0; i < obszary_Poziom; i++)
            {
                if ((i > 0) && (i < (obszary_Poziom - 1)))
                {
                    licz_Pozycje_Dla_Obszaru(i * szerokosc_Duza, wysokosc_Pasa + (wysokosc_Duza * (obszary_Pion - 1)) + (wysokosc_Duza - wysokosc_Mala), 1, lewy_dolny - 1 - i);
                }
                else
                {
                    licz_Pozycje_Dla_Obszaru(i * szerokosc_Duza, wysokosc_Pasa + (wysokosc_Duza * (obszary_Pion - 1)), 0, lewy_dolny - 1 - i);
                }
            }
            #endregion

            #region lewe
            for (int i = 0; i < obszary_Pion - 2; i++)
            {
                licz_Pozycje_Dla_Obszaru(0, wysokosc_Pasa + wysokosc_Duza * (i + 1), 2, lewy_dolny + (obszary_Pion - 2) - 1 - i);
            }
            #endregion

            #region prawe
            for (int i = 0; i < obszary_Pion - 2; i++)
            {
                licz_Pozycje_Dla_Obszaru(obszary_Poziom * szerokosc_Duza - szerokosc_Mala, wysokosc_Pasa + wysokosc_Duza * (i + 1), 2, obszary_Poziom + 1 - 1 + i);
            }
            #endregion
        }
        private void licz_Pozycje_Dla_Obszaru(int x_Start, int y_Start, int rodzaj, int numer_Obszaru) /* Rodzaje: duzy - 0, poziomy - 1, pionowy - 2 */
        {
            int x_Stop = 0;
            int y_Stop = 0;
            int szerokosc = 0;
            int wysokosc = 0;

            if (rodzaj == 0)
            {
                szerokosc = szerokosc_Duza;
                wysokosc = wysokosc_Duza;
            }
            else if (rodzaj == 1)
            {
                szerokosc = szerokosc_Duza;
                wysokosc = wysokosc_Mala;
            }
            else if (rodzaj == 2)
            {
                szerokosc = szerokosc_Mala;
                wysokosc = wysokosc_Duza;
            }
            x_Stop = x_Start + szerokosc;
            y_Stop = y_Start + wysokosc;

            for (int i = y_Start; i < y_Stop; i += przerwa_Pion)
            {
                for (int k = x_Start; k < x_Stop; k += przerwa_Poziom)
                {
                    pozycje[numer_Obszaru].Add((k + i * x) * pixel);
                }
            }
            if (this.Visible == true)
                przelicz_Panele(x_Start, y_Start, numer_Obszaru, szerokosc, wysokosc);
        }
        protected virtual void przelicz_Panele(int x_Start, int y_Start, int numer, int szerokosc, int wysokosc) { }
        protected virtual void aktualizuj_Rozmiar() { }
        private void Obszary_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}