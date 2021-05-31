using System.Text;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data.SqlClient;
using Logic_Layer;

namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool LoginOK { get; set; }
        private LoginWindow loginW;
        private Logic logicObj;
        private ECG_Window ecgw;
        private string socsecNB;
        private string måleID;
       
        public MainWindow()
        {
            // ved opstart oprettes loginvinduet men denne åbnes først i Window_Loaded, programmet bedes også lytte efter tryk på ESC, funktionaliten
            // for ESC programmeres senere
            loginW = new LoginWindow(this, logicObj);
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // et Logik objekt oprettes, loginW vises 
            logicObj = new Logic();
            this.Hide();
            loginW.ShowDialog();
            // Hvis at resultatet af loginW er true vises dette vindue igen hvis at resultatet er false forsøges programmet lukket. 
            if (LoginOK == true)
            {
                this.Show();
            }
            if (LoginOK == false)
            {
                this.Close();
            }
            //Hvis at det er en måling i den lokale database som ikke er blevet undersøgt af en læge blinker knappen NewECG
            if (logicObj.GetLokalinfo()._doctor_att==true)
            {
                Blinkingbutton(newECG_Button,1000,5);

               if(logicObj.GetLokalinfo()._STEMI_suspected==true)
               {
                  Stemi_Alarm_Label.Content = "STEMI mistænkt";
               }
               
            }
            // comboboksen fyldes med målinger fra den offentlige EKG-database, de står i formattet "borgerCPR + måling nr: + måleID"
            foreach (var item in logicObj.ID())
            {
                cpr_CB.Items.Add(item.borgerCPR + " måling nr: " + item.måleID);
            }
        }
        private void HandleEsc(object sender, KeyEventArgs e)
        {
            // Ved tryk på ESC forsøges at lukke vinduet.
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
        public void Blinkingbutton(Button newECG_Button, int length, double repetition)
        {
            // Den funktion kan kaldes for at få en knap til at blinke.
            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromMilliseconds(length)),
                AutoReverse = true,
                RepeatBehavior = new RepeatBehavior(repetition)
            };
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(opacityAnimation);
            Storyboard.SetTarget(opacityAnimation, newECG_Button);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            storyboard.Begin(newECG_Button);
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //hvis programmet lukkes ved kommando kaldt i koden lukkes vinduet som normalt
            if (LoginOK == false)
            {
                e.Cancel = false;
            }

            // hvis programmet lukkes på lukknappen, sørger metoden  for at der kommer en advarelsesbok op hvor brugeren kan nå at fortryde
            else
            {
                e.Cancel = true;
                var result = MessageBox.Show("Ønsker du at lukke programmet?", "Advarelse", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                if (result == MessageBoxResult.Yes)
                {
                    e.Cancel = false;
                }
            }
        }
        private void newECG_Button_Click(object sender, RoutedEventArgs e)
        {
            måleID = Convert.ToString(logicObj.GetLokalinfo()._lokalID);
            // trykkes på knappen med nyt ecg undersøges det om der er en måling i den lokaledatabase som ikke er blevet set på fra hospitales side
            // hvis der er sådan en måling vises den i ECG_Window ellers vises en besked om at der ikke er nogen ny måling.
            if(logicObj.GetLokalinfo()._lokalID!= 0)
            {
                ecgw = new ECG_Window(logicObj, socsecNB, måleID, false);
                logicObj.GetLokalinfo()._doctor_att = true;
                this.Hide();
                ecgw.ShowDialog();
                this.Show();
            }
            else
            {
                MessageBox.Show("Ingen ny ECG");
            }  
        }
        private void logout_BT_Click(object sender, RoutedEventArgs e)
        {
            // Programmet forsøges at logge ud af nuværende bruger og returnere til loginvinduet
            this.Hide();
            var mv = new MainWindow();
            mv.Show();
            LoginOK = false;
            this.Close();
      }
        private void valgtEKG_BT_Click(object sender, RoutedEventArgs e)
        {
            // Eventhandler for tryk på knappen valgtEKG, ved tryk vises en ekg fra den offentlige EKG-database
            int found = 0;
            if(cpr_CB.Text!="")
            {
                found = cpr_CB.Text.IndexOf(" måling nr: ");
                string måleid = cpr_CB.Text.Substring(found + 12);
                string cpr = cpr_CB.Text.Substring(0, found);
                socsecNB = cpr_CB.Text;
                ecgw = new ECG_Window(logicObj, cpr, måleid, true);
                this.Hide();
                ecgw.ShowDialog();
                this.Show();
            }

            else 
            {
                MessageBox.Show("Vælg venligst en ekg måling");
            }
        }
    }
}   