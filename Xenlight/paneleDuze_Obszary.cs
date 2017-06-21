using System.Drawing;

namespace Xenlight
{
    class paneleDuze_Obszary : Obszary
    {
        public paneleDuze_Obszary(int przerwa_Pion, int przerwa_Poziom, int obszary_Pion, int obszary_Poziom, int rozmiar_Procentowy, int wysokosc_Czarnych_Pasow)
            : base(przerwa_Pion, przerwa_Poziom, obszary_Pion, obszary_Poziom, rozmiar_Procentowy, wysokosc_Czarnych_Pasow)
        {
            this.Text = "Xenlight - Podgląd obszarów";
        }

        protected override void przelicz_Panele(int x_Start, int y_Start, int numer, int szerokosc, int wysokosc)
        {
            panele[numer].Name = "panel" + numer;
            panele[numer].Location = new Point(x_Start + 1, y_Start + 1);
            panele[numer].Height = wysokosc - 2;
            panele[numer].Width = szerokosc - 2;
            panele[numer].BackColor = Color.White;
            panele[numer].Click -= new System.EventHandler(Obszary_Click);
            panele[numer].Click += new System.EventHandler(Obszary_Click);
            panele[numer].Visible = true;
            Controls.Add(panele[numer]);
        }
        protected override void aktualizuj_Rozmiar()
        {
            this.Width = x;
            this.Height = y;
        }
    }
}
