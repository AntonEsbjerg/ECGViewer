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
        Logic logicRef;
        String SocSecNb;
        string måleID;
        bool offentlig;
        public ChartValues<double> ecgCollection { get; set; }
        public ChartValues<double> Xvalues { get; set; }
        double[] ekgarray = new double[500];

      public ECG_Window(Logic logicRef, String SocSecNb, string måleID, bool offentlig)
        {
            InitializeComponent();
            this.logicRef = logicRef;
            this.SocSecNb = SocSecNb;
            this.måleID = måleID;
            this.offentlig = offentlig;
            DataContext = this;
            ecgCollection = new ChartValues<double>();
            Xvalues = new ChartValues<double>();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            ecgCollection.Clear();
           
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
            foreach (var item in logicRef.ECGData(måleID))
                   { 
                       ecgCollection.Add(item.ECGVoltage-baseline);

                   }          
                       STEMI_Button.IsEnabled = false;
                       NOSTEMI_Button.IsEnabled = false;

                  cpr_Lb.Content = SocSecNb;
            }
            else if(offentlig==false)
            {
            double baseline = 0;
            var histogram1 = Histogram.CreateEmpty(-1.8, 5.8, 76);
            DTO.DTO_ECG[] dTO_array = new DTO.DTO_ECG[500];
            dTO_array = logicRef.GetLokalinfo()._lokalECG.ToArray();

            for (int i = 0; i < dTO_array.Length; i++)
            {

               ekgarray[i] = Convert.ToDouble(dTO_array[i].ECGVoltage);
               histogram1.Increment(ekgarray[i]);
            }

            var max = histogram1.MaxIndex();
            Interval<double> bin = histogram1.Bins[max];
            baseline = bin.LowerBound + bin.Width / 2;

            foreach (var item in logicRef.GetLokalinfo()._lokalECG)
                   {
                       ecgCollection.Add(item.ECGVoltage-baseline);
                   }

                cpr_Lb.Content = logicRef.GetLokalinfo()._borger_cprnr;
               if (logicRef.GetLokalinfo()._STEMI_suspected == true)
               {
                  Analyse_label.Content = "STEMI mistænkt";
               }

               else if(logicRef.GetLokalinfo()._STEMI_suspected == false)
               {
                  Analyse_label.Content = "Ingen STEMI";
               }

         }
         
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

