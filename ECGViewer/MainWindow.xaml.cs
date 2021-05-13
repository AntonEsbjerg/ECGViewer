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
        private String måleID;
        private List<String> tempsocsecNB;
       
        public MainWindow()
        {
            logicObj = new Logic();
            loginW = new LoginWindow(this, logicObj);
            tempsocsecNB = new List<string>();
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }
        private SqlConnection connect
        {
            get
            {
                var con = new SqlConnection(@"Data Source=st-i4dab.uni.au.dk;Initial Catalog=ST2PRJ2OffEKGDatabase;Integrated Security=False;User ID=ST2PRJ2OffEKGDatabase;Password=ST2PRJ2OffEKGDatabase;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False");

                con.Open();

                return con;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();

            loginW.ShowDialog();

            cpr_CB.Text = "Find CPR";
            

            if (LoginOK == true)
            {
                this.Show();

            }
            if (LoginOK == false)
            {
                this.Close();
            }
            SqlDataReader rdr;
            string insertStringParam = ("Select borger_cprnr,ekgmaaleid from EKGMAELING where borger_cprnr IS NOT NULL");
            using (SqlCommand cmd = new SqlCommand(insertStringParam, connect))
            {
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    socsecNB = Convert.ToString(rdr["borger_cprnr"]);
                    måleID = Convert.ToString(rdr["ekgmaaleid"]);
                    tempsocsecNB.Add(socsecNB + " måling nr: " + måleID);
                }
            }
            connect.Close();
            foreach (var item in tempsocsecNB)
            {
                cpr_CB.Items.Add(item);
            } 
        }
        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
        public void Blinkingbutton(Button newECG_Button, int length, double repetition)
        {
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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
            Blinkingbutton(newECG_Button, 500, 3.0);
            ecgw = new ECG_Window(logicObj, socsecNB, måleID);
            this.Hide();
            ecgw.ShowDialog();
            this.Show();
        }

        private void logout_BT_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void valgtEKG_BT_Click(object sender, RoutedEventArgs e)
        {
            int found = 0;
            found = cpr_CB.Text.IndexOf(" måling nr: ");
            string måleid= cpr_CB.Text.Substring(found + 12);
            string cpr= cpr_CB.Text.Substring(0,found);
            socsecNB = cpr_CB.Text;
            ecgw = new ECG_Window(logicObj, cpr, måleid);
            this.Hide();
            ecgw.ShowDialog();
            this.Show();         
        }
    }
}

