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
        private const String db = "F21ST2ITS2au675718";
        private int id = 0;
        public ChartValues<Double> Yvalues { get; set; }
        public ChartValues<Double> Xvalues { get; set; }

        public ECG_Window(Logic logicRef, String SocSecNb)
        {
            InitializeComponent();

            this.logicRef = logicRef;
            this.SocSecNb = SocSecNb;


        }
        private SqlConnection connect
        {
            get
            {
                SqlConnection conn;
                conn = new SqlConnection(@"Data Source=st-i4dab.uni.au.dk;Initial Catalog=F21ST2ITS2au675718;User ID=F21ST2ITS2au675718;Password=" + db + ";Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                conn.Open();
                return conn;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cpr_Lb.Content = SocSecNb;

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

