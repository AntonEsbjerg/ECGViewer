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
using Extreme.Statistics;
using Extreme.DataAnalysis;
using Extreme.Mathematics;



namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for ECG_Window.xaml
    /// </summary>
    public partial class ECG_Window : Window
    {
        private Logic logicRef;
        private String SocSecNb;
        private string måleID;
        private bool offentlig;
        public ChartValues<double> ecgCollection { get; set; }
        private double[] ekgarray;
        private const double SAMPLE_RATE = 50;
        public Func<double, string> labelformatter { get; set; }
        public Func<double, string> labelformatter1 { get; set; }
        public SeriesCollection Maalingcollection { get; set; }
        public LineSeries EKGMaaling { get; set; }
      public ECG_Window(Logic logicRef, String SocSecNb, string måleID, bool offentlig)
        {
            InitializeComponent();
            EKGMaaling = new LineSeries();
            this.logicRef = logicRef;
            this.SocSecNb = SocSecNb;
            this.måleID = måleID;
            this.offentlig = offentlig;
            Maalingcollection = new SeriesCollection();
            EKGMaaling.Values = new ChartValues<double> { };
            labelformatter = x => (x / SAMPLE_RATE).ToString();
            labelformatter1 = x => (x.ToString("F1"));
            ekgarray = new double[500];
            DataContext = this;

      }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
         EKGMaaling.Title = "EKG-måling";
         Maalingcollection.Clear();
         EKGMaaling.Values.Clear();
         ChartECG.AxisX[0].Separator.Step = 0.04 / (1/SAMPLE_RATE);
         ChartECG.AxisX[1].Separator.Step = 0.2 /(1/SAMPLE_RATE);
         ChartECG.AxisY[0].Separator.Step = 0.1;
         ChartECG.AxisY[1].Separator.Step = 0.5;
         EKGMaaling.Fill = System.Windows.Media.Brushes.Transparent;
         EKGMaaling.PointGeometry = null;

         if (offentlig == true)
         {
            double baseline = 0;
            var histogram1 = Histogram.CreateEmpty(-1.8, 5.8, 76);
            DTO.DTO_ECG[] dTO_array = new DTO.DTO_ECG[500];
            dTO_array = logicRef.ECGData(måleID).ToArray();

            for (int i = 0; i < dTO_array.Length; i++)
            {

               ekgarray[i] = Convert.ToDouble(dTO_array[i].ECGVoltage);
               histogram1.Increment(ekgarray[i]);
            }

            var max = histogram1.MaxIndex();
            Interval<double> bin = histogram1.Bins[max];
            baseline = bin.LowerBound + bin.Width / 2;

            for (int i = 0; i < ekgarray.Length; i++)
            {
               EKGMaaling.Values.Add(ekgarray[i] - baseline);
            }

            STEMI_Button.IsEnabled = false;
            NOSTEMI_Button.IsEnabled = false;
            cpr_Lb.Content = SocSecNb;
         }
         else if (offentlig == false)
         {
            double baseline = 0;
            var histogram1 = Histogram.CreateEmpty(-1.8, 5.8, 76);
            DTO.DTO_ECG[] dTO_array = new DTO.DTO_ECG[500];
            dTO_array = logicRef.GetLokalinfo()._lokalECG.ToArray();
            cpr_Lb.Content = logicRef.GetLokalinfo()._borger_cprnr;

            for (int i = 0; i < dTO_array.Length; i++)
            {

               ekgarray[i] = Convert.ToDouble(dTO_array[i].ECGVoltage);
               histogram1.Increment(ekgarray[i]);
            }

            var max = histogram1.MaxIndex();
            Interval<double> bin = histogram1.Bins[max];
            baseline = bin.LowerBound + bin.Width / 2;

            for (int i = 0; i < ekgarray.Length; i++)
            {
               EKGMaaling.Values.Add(ekgarray[i] - baseline);
            }

            if (logicRef.GetLokalinfo()._STEMI_suspected == true)
            {
               Analyse_label.Content = "STEMI mistænkt";
            }

            else if (logicRef.GetLokalinfo()._STEMI_suspected == false)
            {
               Analyse_label.Content = "Ingen STEMI";
            }

         }

         Maalingcollection.Add(EKGMaaling);

      }

      private void home_button_Click1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void STEMI_Button_Click(object sender, RoutedEventArgs e)
        {
            // ved tryk skal bekræftes tastet hvis bekræftet uploades den nuværende måling til den Offentlige-EKG-Database hvis denne ikke er taget fra DOEDB.
            // og diagnosen STEMI uploades til den lokale database hvis den måling ikke er taget fra den Offentlige EKG-Database
            var result = MessageBox.Show("OBS. Du er ved at give diagnosen STEMI, ønsker du at forsætte?", "Advarelse", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                logicRef.uploadSTEMI(logicRef.GetLokalinfo());
            }
        }

        private void NOSTEMI_Button_Click(object sender, RoutedEventArgs e)
        {
            // ved tryk skal bekræftes tastet hvis bekræftet uploades diagnosen ingen STEMI i den lokale database.
            var result = MessageBox.Show("OBS. Du er ved at give diagnosen INGEN STEMI, ønsker du at forsætte?", "Advarelse", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                logicRef.uploadNoSTEMI(logicRef.GetLokalinfo());
            }
        }
    }
}

