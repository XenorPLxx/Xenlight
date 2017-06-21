using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;

namespace Xenlight
{
    class panelePodglad_Obszary : Obszary
    {
        private int WskaznikRecznego = 0;
        public panelePodglad_Obszary(int przerwa_Pion, int przerwa_Poziom, int obszary_Pion, int obszary_Poziom, int rozmiar_Procentowy, int wysokosc_Czarnych_Pasow)
            : base(przerwa_Pion, przerwa_Poziom, obszary_Pion, obszary_Poziom, rozmiar_Procentowy, wysokosc_Czarnych_Pasow)
        {
            this.Opacity = 100;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Location = new Point(x / 4, x / 4);
            this.Text = "Xenlight - Podgląd kolorów";
            this.BackColor = Color.White;
            this.ForeColor = Color.White;
            this.label1.ForeColor = Color.Black;
        }

        public int WskaznikRecznego_GS
        {
            set
            {
                WskaznikRecznego = value;
            }
        }

        protected override void przelicz_Panele(int x_Start, int y_Start, int numer, int szerokosc, int wysokosc)
        {
            x_Start /= 2;
            y_Start /= 2;
            szerokosc /= 2;
            wysokosc /= 2;
            panele[numer].Name = "panel" + numer;
            panele[numer].Location = new Point(x_Start + 1, y_Start + 1);
            panele[numer].Height = wysokosc - 2;
            panele[numer].Width = szerokosc - 2;
            panele[numer].BackColor = Color.Black;
            panele[numer].Click -= new System.EventHandler(Obszary_Click);
            panele[numer].Click += new System.EventHandler(Obszary_Click);
            panele[numer].TabIndex = numer + 1;
            Controls.Add(panele[numer]);
        }
        protected override void aktualizuj_Rozmiar()
        {
            int szerokosc_Ramki = this.Width - this.ClientSize.Width;
            int wysokosc_Ramki = this.Height - this.ClientSize.Height;
            this.Width = x / 2 + szerokosc_Ramki;
            this.Height = y / 2 + wysokosc_Ramki;
        }
        public void kolorowanie_Paneli(Collection<Color> kolory)
        {
            for (int i = 0; i < kolory.Count; i++)
            {
                panele[i].BackColor = kolory[i];
            }
        }
        protected override void Obszary_Click(object sender, System.EventArgs e)
        {
            try
            {
                Panel tempPanel = (Panel)sender;
                if (WskaznikRecznego == 1)
                {
                    if (tempPanel.Name == "panel0")
                    {
                        if (colorDialog1.ShowDialog() == DialogResult.OK)
                        {
                            for (int i = 0; i < panele.Count; i++)
                            {
                                panele[i].BackColor = colorDialog1.Color;
                            }
                        }
                    }
                }
                else if (WskaznikRecznego == 4)
                {
                    if (colorDialog1.ShowDialog() == DialogResult.OK)
                    {
                        tempPanel.BackColor = colorDialog1.Color;
                    }
                }
            }
            catch { }
        }
    }
}