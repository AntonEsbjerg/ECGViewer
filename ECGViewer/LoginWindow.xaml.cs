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
using System.Windows.Shapes;
using Logic_Layer;

namespace Presentation_Layer
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow: Window
    {
        private Logic logicRef;
        private MainWindow mainWRef;

        public LoginWindow(MainWindow mainWRef, Logic logicRef)
        {
            InitializeComponent();
            this.logicRef = logicRef;
            this.mainWRef = mainWRef;
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);

        }
        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
        private void login_Button_Click(object sender, RoutedEventArgs e)
        {
            //sætter eventhandler til login knappen, således at hvis login er gyligt så sættes loginok til true, loginviduet lukkes og hovedmenuen åbner
            //hvis ikke login er gyldigt kommer en messagebox frem og textboksene nulstilles + cursor sættes i brugernavn_tb
            logicRef = new Logic();
            if (logicRef.checkLogin(brugernavn_TB.Text, password_TB.Password) == true)
            {

                mainWRef.LoginOK = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Du har indtastet forkert brugernavn eller adgangskode, prøv igen!", "Fejlmeddelelse", MessageBoxButton.OK, MessageBoxImage.Error);
                brugernavn_TB.Clear();
                password_TB.Clear();
                brugernavn_TB.Focus();

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //cursoren sættes i brugernavn_tb til start
            brugernavn_TB.Focus();

            //gør således brugeren ikke kan skrive mere end cpr-nr. i brugernavntb
            brugernavn_TB.MaxLength = 11;


            capsLock_image.Visibility = Visibility.Hidden;


        }

        private void password_TB_KeyDown(object sender, KeyEventArgs e)
        {

            //loop der gør det muligt at trykke enter istedet for at trykkke på loginknap

            if (Keyboard.IsKeyDown(Key.Enter))
            {
                login_Button_Click(this, new RoutedEventArgs());
            }

            //tjekker for capslock ved aktivitet inde i pazzword-tekstboxen
            capsLock_image.Visibility = ((Keyboard.GetKeyStates(Key.CapsLock) & KeyStates.Toggled) > 0) ? Visibility.Visible : Visibility.Hidden;

        }


        private void password_TB_GotFocus(object sender, RoutedEventArgs e)
        {
            //tjekker for capslock når man går ind i password-teksboxen
            capsLock_image.Visibility = ((Keyboard.GetKeyStates(Key.CapsLock) & KeyStates.Toggled) > 0) ? Visibility.Visible : Visibility.Hidden;

        }

        private void password_TB_LostFocus(object sender, RoutedEventArgs e)
        {
            //skjuler capslock-billedet når man går ud af password-teksboxen
            capsLock_image.Visibility = Visibility.Hidden;
        }

        private void LoginW_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //hvis programmet lukkes ved kommando kaldt i koden lukkes vinduet som normalt

            if (mainWRef.LoginOK == true)
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
                    mainWRef.LoginOK = false;
                    e.Cancel = false;
                }
            }

        }
    }
}

