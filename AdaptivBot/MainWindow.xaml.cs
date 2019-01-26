using AutoIt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using CredentialManagement;

namespace AdaptivBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }


        private void StoreUserCredentials()
        {
            if (!CredentialStore.Instance.credentialsFound && (bool)chkBxRememberMe.IsChecked)
            {
                CredentialStore.Instance.credential.Username = txtUserName.Text;
                CredentialStore.Instance.credential.Password = txtPasswordBox.Password;
                CredentialStore.Instance.credential.PersistanceType =
                    PersistanceType.LocalComputer;
                CredentialStore.Instance.credential.Save();
            }
            else if (CredentialStore.Instance.credentialsFound
                && (CredentialStore.Instance.credential.Username != txtUserName.Text
                || CredentialStore.Instance.credential.Password != txtPasswordBox.Password))
            {
                
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StoreUserCredentials();
            // Wrap this in a function called login to Adaptiv or which checks if Adaptiv has already been 
            // logged into.
            webBrowser.Navigate("https://adaptiv.standardbank.co.za/Adaptiv/default.aspx");
            var username = txtUserName.Text;
            var password = txtPasswordBox.Password;
            var enterAdaptivCredentialsThread
                = new Thread(() => EnterAdaptivCredentials(username, password));
            enterAdaptivCredentialsThread.Start();
        }

        private void EnterAdaptivCredentials(string username, string password)
        {
            AutoItX.WinWait("Windows Security");
            AutoItX.WinActivate("Windows Security");
            //logger.StatusText = "Entering credentials...";
            AutoItX.Send(username);
            AutoItX.Send("{TAB}");
            AutoItX.Send(password);
            AutoItX.Send("{TAB}");
            AutoItX.Send("{ENTER}");
            AutoItX.WinWait("Adaptiv Disclaimer -- Webpage Dialog");
            AutoItX.WinActivate("Adaptiv Disclaimer -- Webpage Dialog");
            //logger.StatusText = "Acknowledging disclaimer...";
            AutoItX.Send("{ENTER}");
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (CredentialStore.Instance.credentialsFound)
            {
                txtUserName.Text = CredentialStore.Instance.credential.Username;
                txtPasswordBox.Password = CredentialStore.Instance.credential.Password;
            }
        }
    }
}
