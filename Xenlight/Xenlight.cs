using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Xenlight
{
    public partial class Xenlight : Form
    {
        #region zmienne_i_stale
        private paneleDuze_Obszary obszar;
        private panelePodglad_Obszary podglad;
        private Collection<Obszary> formatki = new Collection<Obszary>();
        private kontrolaKolorow kolory = new kontrolaKolorow();
        private Rectangle rozdzielczosc = Screen.PrimaryScreen.Bounds;
        private Boolean wskaznik_Balonika = false;
        const int pixel = 4;
        #endregion
        #region konstruktor
        public Xenlight(string[] args)
        {
            InitializeComponent();

            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead("http://www.dawidkarczewski.pl/xenlight/wersja.txt");
                StreamReader reader = new StreamReader(stream);
                String content = reader.ReadToEnd();
                String wersja = File.ReadAllText(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\wersja.txt");

                if (content != wersja)
                {
                    if (MessageBox.Show("Lokalna wersja programu: " + wersja + ".\nNowa wersja programu: " + content + ".\nCzy chcesz przejść do strony pobierania?", "Aktualizacja", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("http://www.dawidkarczewski.pl/xenlight/");
                    }
                }
            }
            catch (Exception) { }

            obszar = new paneleDuze_Obszary((int)numericUpDown2.Value, (int)numericUpDown5.Value,
                (int)numericUpDown6.Value, (int)numericUpDown7.Value, (int)numericUpDown3.Value, (int)numericUpDown4.Value);
            podglad = new panelePodglad_Obszary((int)numericUpDown2.Value, (int)numericUpDown5.Value,
                (int)numericUpDown6.Value, (int)numericUpDown7.Value, (int)numericUpDown3.Value, (int)numericUpDown4.Value);
            formatki.Add(obszar);
            formatki.Add(podglad);
            wczytaj_Config();
            aktualizacja_Danych();
            sprawdzanieParametrów(args);
        }

        private void sprawdzanieParametrów(string[] args)
        {
            if ((args.Length == 2) && (args[0] == "-con") && (args[1] == "-main"))
            {
                button1_Click(null, null);
                checkBox2.Checked = true;
            }
            else if ((args.Length == 3) && (args[0] == "-con") && (args[1] == "-main") && (args[2] == "-min"))
            {
                button1_Click(null, null);
                checkBox2.Checked = true;
                wskaznik_Balonika = true;
                this.WindowState = FormWindowState.Minimized;
                Xenlight_Resize(null, null);
            }
            timer1.Enabled = true;
        }
        #endregion

        private void wczytaj_Config()
        {
            // read text file
            string filePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\config.cfg";
            if (File.Exists(filePath))
            {
                StreamReader file = null;
                try
                {
                    file = new StreamReader(filePath);
                    numericUpDown1.Value = Convert.ToInt32(file.ReadLine());
                    numericUpDown2.Value = Convert.ToInt32(file.ReadLine());
                    numericUpDown3.Value = Convert.ToInt32(file.ReadLine());
                    numericUpDown4.Value = Convert.ToInt32(file.ReadLine());
                    numericUpDown5.Value = Convert.ToInt32(file.ReadLine());
                    numericUpDown6.Value = Convert.ToInt32(file.ReadLine());
                    numericUpDown7.Value = Convert.ToInt32(file.ReadLine());
                    numericUpDown8.Value = Convert.ToInt32(file.ReadLine());
                    numericUpDown9.Value = Convert.ToInt32(file.ReadLine());
                    numericUpDown10.Value = Convert.ToInt32(file.ReadLine());
                }
                finally
                {
                    if (file != null)
                        file.Close();
                }
            }
            
            Console.ReadLine();
        }
        #region funkcja_podstawowa
        private void timer1_Tick(object sender, System.EventArgs e)
        {
            if ((rozdzielczosc == Screen.PrimaryScreen.Bounds) && (checkBox2.Checked == true))
            {
                try
                {
                    kolory.tryb_Podstawowy_11(obszar.pozycje);
                    label18.Text = "DirectX w użyciu: DX11";
                }
                catch
                {
                    kolory.tryb_Podstawowy(obszar.pozycje);
                    label18.Text = "DirectX w użyciu: DX9";
                }
                podglad.kolorowanie_Paneli(kolory.kolory_GS);
                kolory.kalibracja_RGB((int)numericUpDown8.Value, (int)numericUpDown9.Value, (int)numericUpDown10.Value);
            }
            else if (checkBox3.Checked == true)
            {
                kolory.tryb_Plynne_Calosc(obszar.pozycje.Count);
                podglad.kolorowanie_Paneli(kolory.kolory_GS);
            }
            else if (checkBox4.Checked == true)
            {
                kolory.tryb_Choinka(obszar.pozycje.Count);
                podglad.kolorowanie_Paneli(kolory.kolory_GS);
            }
            else if (checkBox5.Checked == true)
            {
                if (radioButton1.Checked)
                {
                    kolory.tryb_Manualny_Duplikat(podglad.panele_GS[0].BackColor, obszar.pozycje.Count);
                }
                if (radioButton2.Checked)
                {
                }
                if (radioButton3.Checked)
                {
                }
                if (radioButton4.Checked)
                {
                    kolory.tryb_Manualny_Reczny(podglad.panele_GS);
                }
                kolory.kalibracja_RGB((int)numericUpDown8.Value, (int)numericUpDown9.Value, (int)numericUpDown10.Value);
            }
            else
            {
                kolory.wszystko_Czarne(obszar.pozycje.Count);
            }
            wysylanie_Kolorow();
        }
        #endregion
        #region wysylanie_UART
        private void wysylanie_Kolorow()
        {
            Collection<Tuple<Color, int>> do_Wyslania = kolory.kolory_Do_Wyslania();
            for (int i = 0; i < do_Wyslania.Count; i++)
            {
                Send_COM(do_Wyslania[i].Item1, do_Wyslania[i].Item2);
            }
        }
        private void Send_COM(Color kolor, int numer)
        {
            if (serialPort1.IsOpen == true)
            {
                byte[] bajt = new byte[6];
                bajt[0] = (byte)numer;
                bajt[1] = kolor.R;
                bajt[2] = kolor.G;
                bajt[3] = kolor.B;
                bajt[4] = 0xAA;
                bajt[5] = 0xAA;
                serialPort1.Write(bajt, 0, 6);
            }
        }
        #endregion
        #region przyciski_lacznosci
        private void button1_Click(object sender, System.EventArgs e)
        {
            try
            {
                serialPort1.PortName = "COM" + numericUpDown1.Text;
                serialPort1.Open();
                button1.Enabled = false;
                button2.Enabled = true;
                kolory.kolory_Poprzednie_GS.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                wygaszanie_Ledow();
                serialPort1.Close();
                button1.Enabled = true;
                button2.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
        #region menu_trybow
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            zmiana_Zaznaczen((CheckBox)sender);
            zmiana_Widocznosci((CheckBox)sender);
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                podglad.WskaznikRecznego_GS = 1;
            }
            if (radioButton2.Checked)
            {
                podglad.WskaznikRecznego_GS = 2;
            }
            if (radioButton3.Checked)
            {
                podglad.WskaznikRecznego_GS = 3;
            }
            if (radioButton4.Checked)
            {
                podglad.WskaznikRecznego_GS = 4;
            }
            kolory.wszystko_Czarne(obszar.pozycje.Count);
            podglad.kolorowanie_Paneli(kolory.kolory_GS);
        }
        #endregion
        #region menu_trybow_-_funkcje
        private void zmiana_Zaznaczen(CheckBox tempBox)
        {
            if (tempBox.Checked == true)
            {
                for (int i = 2; i < 14; i++)
                {
                    CheckBox box = (CheckBox)groupBox1.Controls["checkBox" + i.ToString()];
                    if (box != tempBox)
                    {
                        box.Checked = false;
                    }
                }
                tempBox.Checked = true;
                for (int a = 0; a < 14; a++)
                {
                    kolory.wszystko_Czarne(obszar.pozycje.Count);
                }
            }
        }
        private void zmiana_Widocznosci(CheckBox tempbox)
        {
            foreach (Control con in groupBox2.Controls)
            {
                con.Visible = false;
            }
            podglad.WskaznikRecznego_GS = 0;
            timer1.Interval = 1;
            if (tempbox.Name == "checkBox2")
            {
                timer1.Interval = 16;
                label2.Visible = true;
                numericUpDown2.Visible = true;
                label4.Visible = true;
                numericUpDown3.Visible = true;
                label5.Visible = true;
                numericUpDown4.Visible = true;
                button3.Visible = true;
                label6.Visible = true;
                label7.Visible = true;
                label8.Visible = true;
                label9.Visible = true;
                numericUpDown5.Visible = true;
            }
            if (tempbox.Name == "checkBox3")
            {
                trackBar1_Scroll(null, null);
                label3.Visible = true;
                trackBar1.Visible = true;
            }
            if (tempbox.Name == "checkBox4")
            {
                trackBar1_Scroll(null, null);
                label3.Visible = true;
                trackBar1.Visible = true;
            }
            if (tempbox.Name == "checkBox5")
            {
                radioButton1.Visible = true;
                radioButton2.Visible = true;
                radioButton3.Visible = true;
                radioButton4.Visible = true;
                label13.Visible = true;
                radioButton1_CheckedChanged(null, null);
            }
        }
        #endregion
        #region sekwencja_wylaczania
        private void wygaszanie_Ledow()
        {
            foreach (CheckBox con in groupBox1.Controls)
            {
                con.Checked = false;
            }
            kolory.wszystko_Czarne(obszar.pozycje.Count);
            wysylanie_Kolorow();
            
        }
        private void Xenlight_FormClosing(object sender, FormClosingEventArgs e)
        {
            wygaszanie_Ledow();
            zapisanie_Configu();
            
        }

        private void zapisanie_Configu()
        {
            try
            {
                StreamWriter sr = new StreamWriter(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\config.cfg");
                sr.WriteLine(numericUpDown1.Value.ToString());
                sr.WriteLine(numericUpDown2.Value.ToString());
                sr.WriteLine(numericUpDown3.Value.ToString());
                sr.WriteLine(numericUpDown4.Value.ToString());
                sr.WriteLine(numericUpDown5.Value.ToString());
                sr.WriteLine(numericUpDown6.Value.ToString());
                sr.WriteLine(numericUpDown7.Value.ToString());
                sr.WriteLine(numericUpDown8.Value.ToString());
                sr.WriteLine(numericUpDown9.Value.ToString());
                sr.WriteLine(numericUpDown10.Value.ToString());
                sr.Close();
            }
            catch { }
        }
        #endregion
        #region zmiana_szybkosci
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            timer1.Interval = 1001 - trackBar1.Value;
        }
        #endregion
        #region dodatkowe_formatki
        private void button3_Click(object sender, EventArgs e)
        {
            if (obszar.Visible)
                obszar.Visible = false;
            else
            {
                obszar.Show();
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (podglad.Visible)
                podglad.Visible = false;
            else
            {
                podglad.Show();
            }
        }
        #endregion
        #region zmiana_wartosci_numeric
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < formatki.Count; i++)
            {
                formatki[i].przerwa_Pion_GS = (int)numericUpDown2.Value;
            }
            aktualizacja_Danych();
        }
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < formatki.Count; i++)
            {
                formatki[i].rozmiar_Procentowy_GS = (int)numericUpDown3.Value;
            }
            aktualizacja_Danych();
        }
        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < formatki.Count; i++)
            {
                formatki[i].wysokosc_Czarnych_Pasow_GS = (int)numericUpDown4.Value;
            }
            aktualizacja_Danych();
        }
        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < formatki.Count; i++)
            {
                formatki[i].przerwa_Poziom_GS = (int)numericUpDown5.Value;
            }
            aktualizacja_Danych();
        }
        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < formatki.Count; i++)
            {
                formatki[i].obszary_Pion_GS = (int)numericUpDown6.Value;
            }
            aktualizacja_Danych();
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < formatki.Count; i++)
            {
                formatki[i].obszary_Poziom_GS = (int)numericUpDown7.Value;
            }
            aktualizacja_Danych();
        }
        #endregion
        #region zmiana_wartosci_numeric_-_funkcje
        private void aktualizacja_Danych()
        {
            int[] szerokosci = obszar.szerokosci_GS;
            int[] wysokosci = obszar.wysokosci_GS;
            int[] pixele = obszar.pixele_GS;
            string[] pixeleStr = new string[3];

            for (int i = 0; i < 3; i++)
            {
                if (pixele[i] > 100000)
                {
                    pixeleStr[i] = (pixele[i] / 1000).ToString() + "k";
                }
                else
                {
                    pixeleStr[i] = pixele[i].ToString();
                }
            }

            label6.Text = "Wysokość obszaru: " + wysokosci[0] + ", " + wysokosci[1];
            label7.Text = "Szerokość obszaru: " + szerokosci[0] + ", " + szerokosci[1];
            label8.Text = "Pixele na obszar: " + pixeleStr[0] + ", " + pixeleStr[1] + ", " + pixeleStr[2];
        }
        #endregion       

        private void Xenlight_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                if (!wskaznik_Balonika) notifyIcon1.ShowBalloonTip(500);
                wskaznik_Balonika = true;
                this.Hide(); 
                this.ShowInTaskbar = false;
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }       
    }
}
