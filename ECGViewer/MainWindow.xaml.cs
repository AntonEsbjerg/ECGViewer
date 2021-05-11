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
        public MainWindow()
        {
            logicObj = new Logic();
            loginW = new LoginWindow(this, logicObj);
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
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
            ecgw = new ECG_Window(logicObj, socsecNB);
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
            ecgw = new ECG_Window(logicObj, socsecNB);
            this.Hide();
            ecgw.ShowDialog();
            this.Show();
        }
        //private SqlConnection connect
        //{
        //    get
        //    {
        //        SqlConnection conn;
        //        conn = new SqlConnection(@"Data Source=st-i4dab.uni.au.dk;Initial Catalog=F21ST2ITS2au675718;User ID=F21ST2ITS2au675718;Password=" + db + ";Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        //        conn.Open();
        //        return conn;
        //    }
        //}

        //private void cpr_CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    double[] tal;
        //    SqlDataReader rdr;
        //    byte[] bytesArr = new byte[8];
        //    string selectString = "Select ekgmaaleid from EKGMAELING where borger_cprnr = " + cpr_CB.Text;
        //    using (SqlCommand cmd = new SqlCommand(selectString, connect))
        //    {
        //        rdr = cmd.ExecuteReader();
        //        if (rdr.Read())
        //            socsecNB = cpr_CB.Text;

        //    }
        //    connect.Close();
        //}
    }
}

