using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using Microsoft.VisualBasic;

namespace Werkplekleren_1_Galgje
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Variabele
        string teRadenWoord;
        private int levens = 10, intAftelTimer=10, gekozeTijd = 10;
        List<char> juisteLettersList = new List<char>();
        List<char> fouteLettersList = new List<char>();
        
        StringBuilder historiek = new StringBuilder();
        private bool singleplayer = false;
        private bool multiplayer = false;
        
        private DispatcherTimer aftelTimer = new DispatcherTimer();

        private string[] NamenPlayers = new string[5];
        private int[] highScoreLevens = new int[5];
        private string[] tijden = new string[5];

        BitmapImage bi = new BitmapImage();

        bool hintGevraagd = false;

        public MainWindow()
        {
            InitializeComponent();
            aftelTimer.Tick += AftelTimer_Tick;
            aftelTimer.Interval = new TimeSpan(0, 0, 1);
            
        }

        private void AftelTimer_Tick(object sender, EventArgs e)
        {
            //Achtergrond kleur, tijd aflopen, tijd in de progressbar steken
            var bc = new BrushConverter();
            intAftelTimer--;          
            pbTimer.Value = intAftelTimer;
            
            //als de tijd op is
            if (intAftelTimer==0)
            {                             
                Grid.Background = (Brush)bc.ConvertFrom("#640000");
                MessageBox.Show("Je hebt niet op tijd geraden","Leven kwijt");
                intAftelTimer = gekozeTijd;
                levens--;

                aftelTimer.Stop();
                galg();
                Delay();
                CheckVoorLevensOver();
            }
            Grid.Background = (Brush)bc.ConvertFrom("#19191f");
        }

        private string[] galgjeWoorden = new string[]
        {
           "grafeem","tjiftjaf",    "maquette",    "kitsch",    "pochet",    "convocaat",   "jakkeren",    "collaps",    "zuivel",    "cesium",    "voyant",    "spitten",    "pancake",    "gietlepel",
           "karwats",   "dehydreren",   "viswijf",    "flater",    "cretonne",    "sennhut",    "tichel",    "wijten",    "cadeau",    "trotyl",    "chopper",    "pielen",    "vigeren",
           "vrijuit",    "dimorf",    "kolchoz",   "janhen",   "plexus",    "borium",    "ontweien",    "quiche",    "ijverig",    "mecenaat",    "falset",    "telexen",    "hieruit",
           "femelaar",    "cohesie",    "exogeen",    "plebejer",    "opbouw",    "zodiak",    "volder",    "vrezen",    "convex",    "verzenden",    "ijstijd",    "fetisj",    "gerekt",    "necrose",
           "conclaaf",    "clipper",    "poppetjes",    "looikuip",    "hinten",    "inbreng",    "arbitraal",    "dewijl",    "kapzaag",    "welletjes",    "bissen",    "catgut",
           "oxymoron",   "heerschaar",    "ureter",    "kijkbuis",    "dryade",    "grofweg",    "laudanum",    "excitatie",    "revolte",    "heugel",    "geroerd",    "hierbij",    "glazig",
           "pussen",    "liquide",    "aquarium",    "formol",    "kwelder",    "zwager",    "vuldop",    "halfaap",    "hansop",    "windvaan",    "bewogen",    "vulstuk",    "efemeer",    "decisief",
           "omslag",    "prairie",    "schuit",    "weivlies",    "ontzeggen",    "schijn",    "sousafoon"
        };

        private void btnSinglePlayer_Click(object sender, RoutedEventArgs e)
        {
            //Singleplayer gekozen
            SpelOpenen();
            btnNieuw.Visibility = Visibility.Visible;
            btnVerberg.Visibility = Visibility.Hidden;
            btnRaad.Visibility = Visibility.Visible;
            btnRaad.IsEnabled = true;
            singleplayer = true;
            lblGeefWoord.Content = "Raad het woord";
            NieuwWoord();           
            btnRaad.IsDefault=true;
            Delay();
            btnHint.Visibility = Visibility.Visible;
        }
        private void btnMultiplayer_Click(object sender, RoutedEventArgs e)
        {
            //Multiplayer gekozen
            SpelOpenen();
            Reset();
            multiplayer = true;          
            btnNieuw.IsDefault = true;
        }

        private void btnHint_Click(object sender, RoutedEventArgs e)
        {
            Hint();
            hintGevraagd = true;
        }

        private void btnHighscores_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"{historiek}", "Historiek sessie winnaars");
        }

        private void btnSluiten_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnNieuw_Click(object sender, RoutedEventArgs e)
        {         
            if (singleplayer)
            {
                Reset();
                NieuwWoord();
                btnRaad.IsEnabled = true;
                aftelTimer.Start();
                btnHint.IsEnabled = true;
            }
            else if (multiplayer)
            {
                if (!string.IsNullOrWhiteSpace(txtInvoer.Text) && Regex.IsMatch(txtInvoer.Text, @"^[a-zA-Z]+$")) //Kijken over er iets is ingevoerd en alleen letters
                {
                    teRadenWoord = (txtInvoer.Text).ToLower();
                    lblGeefWoord.Content = $"Gekoze woord: {teRadenWoord}";
                    btnVerberg.IsEnabled = true;
                    txtInvoer.Clear();
                    btnVerberg.Visibility = Visibility.Visible;
                    btnRaad.Visibility = Visibility.Hidden;
                    btnNieuw.IsDefault = false;
                    btnVerberg.Focus();
                    aftelTimer.Stop();
                    Reset();
                    btnHint.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Geef een woord in");
                }
            }
           
        }

        private void btnVerberg_Click(object sender, RoutedEventArgs e)
        {
            txtInvoer.Clear();
            lblGeefWoord.Content = "";
            btnVerberg.Visibility = Visibility.Hidden;
            btnRaad.Visibility = Visibility.Visible;
            btnRaad.IsEnabled = true;
            btnHint.Visibility = Visibility.Visible;
            Delay();
            WeergaveMask();
        }

        private void btnRaad_Click(object sender, RoutedEventArgs e)
        {

            if (txtInvoer.Text.Count() == 1 && Regex.IsMatch(txtInvoer.Text, @"^([a-zA-Z]+)$")) //Als er alleen maar een letter is ingeven
            {
                char letter = Convert.ToChar(txtInvoer.Text);
                CheckOfLetterJuistIs(letter);
            }

            else if (txtInvoer.Text.Count() > 1) //Kijkt of er meer dan 2 characters zijn ingevuld 
            {
                string userWoord = txtInvoer.Text;
                CheckOfWoordJuistIs(userWoord);
            }

            else
            {
                MessageBox.Show("Geen geldige ingave");
            }
            WeergaveMask();
            
        }

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            //Terug naar menu knop
            MenuOpenen();
            Reset();
            singleplayer = false;
            multiplayer = false;
            aftelTimer.Stop();
        }

        private void btnTimer_Click(object sender, RoutedEventArgs e)
        {
            //Gebruiker kan kiezen wat de tijd is tussen keuzes, if lus gaat kijken of er een getal wordt ingevuld en of dit een getal is tussen 5 en 20.
            if (int.TryParse(Interaction.InputBox("Kies de tijd tussen raad pogingen.                          Kies een waarde tussen 5 - 20 seconden     (Standaard tijd is 10 seconden)", "Tijd keuze"), out gekozeTijd) || gekozeTijd >= 5 || gekozeTijd <= 20)
            {
                while (gekozeTijd < 5 || gekozeTijd > 20) //zolang dat er geen getal tussen 5 en 20 is gekozen blijft het een nummer afvragen
                {
                    MessageBox.Show("Geef een getal tussen 5 - 20 seconden!", "Foute ingave");
                    int.TryParse(Interaction.InputBox("Kies de tijd tussen raad pogingen.                          Kies een waarde tussen 5 - 20 seconden     Standaard tijd is 10 seconden", "Tijd keuze"), out gekozeTijd);
                }
                pbTimer.Maximum = gekozeTijd; //Progressbar wordt aangepast aan de nieuwe gekoze tijd
            }

        }

        #region Methods

        private void NieuwWoord()
        {
            //Genereert een nieuw woord voor Singleplayer
            Random rnd = new Random();
            teRadenWoord = galgjeWoorden[rnd.Next(0, 100)];
            WeergaveMask();
        }


        private void WeergaveMask()
        {
            //Zet het woord om in streepjes
            string maskeerWoord = "";
            foreach (char letter in teRadenWoord)
            {
                if (juisteLettersList.Contains(letter))
                {
                    maskeerWoord += letter;
                }
                else
                {
                    maskeerWoord += "__";
                }
                maskeerWoord += " ";
            }
            lblMask.Content=maskeerWoord;
            
        }

        #region CheckUps

        public void CheckOfLetterJuistIs(char letter)
        {
            //checken of het letter juist is
            if (juisteLettersList.Contains(letter) || fouteLettersList.Contains(letter)) //Kijken of de letter al is geraden
            {
                MessageBox.Show("Deze letter heb je al geprobeerd", letter.ToString());
            }
            else
            {
                if (teRadenWoord.Contains(letter))
                {
                    juisteLettersList.Add(letter);
                    lblJuist.Content += letter.ToString();
                    txtInvoer.Clear();
                }
                else
                {
                    fouteLettersList.Add(letter);
                    levens--;
                    lblFout.Content += letter.ToString();
                    txtInvoer.Clear();
                    CheckVoorLevensOver();
                    galg();
                }
            }
            aftelTimer.Stop();
            intAftelTimer = gekozeTijd;
            //Als er geen levens meer zijn moet de timer niet meer verder tellen
            if (levens > 0)
            {
                Delay();
            }
            
        }

        public void CheckOfWoordJuistIs(string woord)
        {
            //checken of het woord juist is
            if (teRadenWoord.Equals(woord))
            {
                MessageBox.Show($"Hoera je hebt {teRadenWoord} correct geraden");
                txtInvoer.Clear();
                btnRaad.IsEnabled = false; 
                if (!hintGevraagd) //Als er een hint is gevraagd gaat deze niet door de historiek functie
                {
                    Historiek();
                }
                if (multiplayer)
                {
                    lblGeefWoord.Content = "Geef een nieuw woord in";
                }
                Reset();
                aftelTimer.Stop();
                btnHint.IsEnabled = false;
               
                
            }
            else
            {
                levens--;
                MessageBox.Show("Fout geraden", "Fout");
                txtInvoer.Clear();              
                aftelTimer.Stop();
                intAftelTimer = gekozeTijd;
                galg();
                
                CheckVoorLevensOver();
                if (levens>0)
                {
                    Delay();
                }
                
            }
        }

        public void CheckVoorLevensOver()
        {
            //Kijken of speler nog levens heeft
            if (levens == 0)
            {
                aftelTimer.Stop();
                MessageBox.Show("Je hebt niet optijd het woord kunnen raden", "Verloren!");
                btnRaad.IsEnabled = false;
                
                
            }
        }

        #endregion

        private void galg()
        {
            //Afbeeldingen aan de hand van levens
            switch (levens)
            {
                default:
                    imgGalg.Source = new BitmapImage();
                    break;
                case 9:
                    imgGalg.Source = new BitmapImage(new Uri("Galg\\galg1.png", UriKind.RelativeOrAbsolute));
                    break;
                case 8:
                    imgGalg.Source = new BitmapImage(new Uri("Galg\\galg2.png", UriKind.RelativeOrAbsolute));
                    break;
                case 7:
                    imgGalg.Source = new BitmapImage(new Uri("Galg\\galg3.png", UriKind.RelativeOrAbsolute));
                    break;
                case 6:
                    imgGalg.Source = new BitmapImage(new Uri("Galg\\galg4.png", UriKind.RelativeOrAbsolute));
                    break;
                case 5:
                    imgGalg.Source = new BitmapImage(new Uri("Galg\\galg5.png", UriKind.RelativeOrAbsolute));
                    break;
                case 4:
                    imgGalg.Source = new BitmapImage(new Uri("Galg\\galg6.png", UriKind.RelativeOrAbsolute));
                    break;
                case 3:
                    imgGalg.Source = new BitmapImage(new Uri("Galg\\galg7.png", UriKind.RelativeOrAbsolute));
                    break;
                case 2:
                    imgGalg.Source = new BitmapImage(new Uri("Galg\\galg8.png", UriKind.RelativeOrAbsolute));
                    break;
                case 1:
                    imgGalg.Source = new BitmapImage(new Uri("Galg\\galg9.png", UriKind.RelativeOrAbsolute));
                    break;
                case 0:
                    imgGalg.Source = new BitmapImage(new Uri("Galg\\galg10.png", UriKind.RelativeOrAbsolute));
                    break;
            }
            
        }


        public void Reset()
        {
            //Alles resetten na een spel
            lblFout.Content = "";
            lblJuist.Content = "";
            txtInvoer.Clear();
            intAftelTimer = gekozeTijd;
            levens = 10;
            juisteLettersList.Clear();
            fouteLettersList.Clear();
            lblMask.Content = "";
            imgGalg.Source = new BitmapImage();
            hintGevraagd = false;
        }

        

        private void Historiek()
        {
            //naam - n levens (tijd) bijhouden van spelers die het woord hebben geraden 
            int idx = 0;
            NamenPlayers[idx] = Interaction.InputBox("Geef je naam voor de score bij te houden");
            highScoreLevens[idx] = levens;
            tijden[idx] = DateTime.Now.ToString("HH:mm:ss");                   
            historiek.Append(NamenPlayers[idx].ToString());
            historiek.Append($" - {highScoreLevens[idx].ToString()} levens");
            historiek.Append( $" ({tijden[idx]})\r");

            idx++;
        }

        private void Hint()
        {
            //Als er een hint wordt gevraagd
            string letters = "abcdefghijklmnopqrstuvwxyz";
            for (int i = 0; i < letters.Length; i++) //Blijft door deze lus gaan tot dat er een letter is gevonden dat nog niet tussen de fout geraden letters staat
            {          
                Random rnd = new Random();
                int getal = rnd.Next(0, letters.Length);
                if (!teRadenWoord.Contains(letters[getal]) && !fouteLettersList.Contains(letters[getal]))
                {
                    fouteLettersList.Add(letters[getal]);
                    lblFout.Content += letters[getal].ToString();
                    break;
                }
            }
        } 

        private void Delay()
        {
            //De timer tussen raad pogingen start pas na 1 seconden
            Task.Delay(1000).ContinueWith(_ =>
            {
                aftelTimer.Start();
            });
        }

        #region Visuals

        private void MenuOpenen()
        {
            //Wanneer het menu wordt geopent
            Titel.Visibility = Visibility.Visible;
            btnSinglePlayer.Visibility = Visibility.Visible;
            btnMultiplayer.Visibility = Visibility.Visible;
            btnSluiten.Visibility = Visibility.Visible;
            btnTimer.Visibility = Visibility.Visible;
            btnHighscores.Visibility = Visibility.Visible;

            btnNieuw.Visibility = Visibility.Hidden;
            btnVerberg.Visibility = Visibility.Hidden;
            lblFoutText.Visibility = Visibility.Hidden;
            lblGoedText.Visibility = Visibility.Hidden;
            txtInvoer.Visibility = Visibility.Hidden;
            lblGeefWoord.Visibility = Visibility.Hidden;
            btnMenu.Visibility = Visibility.Hidden;
            btnRaad.Visibility = Visibility.Hidden;
            lblMask.Visibility = Visibility.Hidden;
            pbTimer.Visibility = Visibility.Hidden;
            btnHint.Visibility = Visibility.Hidden;
            lblMask.Visibility = Visibility.Hidden;
        }

        private void SpelOpenen()
        {
            //Wanneer het spel wordt geopent
            Titel.Visibility = Visibility.Hidden;
            btnSinglePlayer.Visibility = Visibility.Hidden;
            btnMultiplayer.Visibility = Visibility.Hidden;
            btnSluiten.Visibility = Visibility.Hidden;
            tbWinnaars.Visibility = Visibility.Hidden;
            btnHighscores.Visibility = Visibility.Hidden;
            btnTimer.Visibility = Visibility.Hidden;

            btnNieuw.Visibility = Visibility.Visible;
            btnVerberg.Visibility = Visibility.Visible;
            lblFoutText.Visibility = Visibility.Visible;
            lblGoedText.Visibility = Visibility.Visible;
            txtInvoer.Visibility = Visibility.Visible;
            lblGeefWoord.Visibility = Visibility.Visible;
            btnMenu.Visibility = Visibility.Visible;
            lblMask.Visibility = Visibility.Visible;
            pbTimer.Visibility = Visibility.Visible;
            lblMask.Visibility = Visibility.Visible;
        }
        #endregion
        #endregion
    }
}
