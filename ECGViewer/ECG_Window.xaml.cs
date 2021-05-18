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
using LiveCharts.Wpf;
using LiveCharts;
using System.Data.SqlClient;
using Logic_Layer;


namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for ECG_Window.xaml
    /// </summary>
    public partial class ECG_Window : Window
    {
        Logic logicRef;
        String SocSecNb;
        string måleID;
        public ChartValues<double> ecgCollection { get; set; }
        public ChartValues<double> Xvalues { get; set; }

        public ECG_Window(Logic logicRef, String SocSecNb, string måleID)
        {
            InitializeComponent();
            this.logicRef = logicRef;
            this.SocSecNb = SocSecNb;
            this.måleID = måleID;
            DataContext = this;
            ecgCollection = new ChartValues<double>();
            Xvalues = new ChartValues<double>();
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
            ecgCollection.Clear();
            cpr_Lb.Content = SocSecNb;
            foreach (var item in logicRef.ECGData(måleID))
            {
                ecgCollection.Add(item.ECGVoltage);
            }
            // listerne til x og y værdieren fyldes med data:

            //foreach (var dTO_BSugar in logicRef.getBSugarData(SocSecNb))
            //{
            //   Yvalues.Add();
            //   Xvalues.Add();


            //DataContext = this;
        }

        private void home_button_Click1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void STEMI_Button_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("OBS. Du er ved at give diagnosen STEMI, ønsker du at forsætte?", "Advarelse", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                logicRef.uploadToDOEDB(logicRef.GetLokalinfo());
            }
        }

        private void NOSTEMI_Button_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("OBS. Du er ved at give diagnosen INGEN STEMI, ønsker du at forsætte?", "Advarelse", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {

            }
        }
    }
}

