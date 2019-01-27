using AutoIt;
using CredentialManagement;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;


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
            CredentialStore credentialStore = new CredentialStore("AdaptivBot" + cmbBxAdaptivEnvironments.SelectedValue.ToString());
            if (!credentialStore.credentialsFound && (bool)chkBxRememberMe.IsChecked)
            {
                credentialStore.credential.Username = txtUserName.Text;
                credentialStore.credential.Password = txtPasswordBox.Password;
                credentialStore.credential.PersistanceType =
                    PersistanceType.LocalComputer;
                credentialStore.credential.Save();
            }
            else if (credentialStore.credentialsFound
                && (credentialStore.credential.Username != txtUserName.Text
                || credentialStore.credential.Password != txtPasswordBox.Password))
            {
                // Add message box to warn user that the credentials that have been entered are different 
                // to the saved credentials & would they like to save them?
            }
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
            CredentialStore credentialStore = new CredentialStore("AdaptivBotProduction");
            if (credentialStore.credentialsFound)
            {
                txtUserName.Text = credentialStore.credential.Username;
                txtPasswordBox.Password = credentialStore.credential.Password;
            }
        }


        private void LoadAdaptivPassword(object sender, SelectionChangedEventArgs e)
        {
            CredentialStore credentialStore =
                new CredentialStore("AdaptivBot" + cmbBxAdaptivEnvironments.SelectedValue);
            if (credentialStore.credentialsFound)
            {
                txtUserName.Text = credentialStore.credential.Username;
                txtPasswordBox.Password = credentialStore.credential.Password;
            }
            else
            {
                txtUserName.Text = "";
                txtPasswordBox.Password = "";
            }
        }



        private void btnExtract_RiskView_Click(object sender, RoutedEventArgs e)
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


        private void btnExtract_CustomerLimitUtilisation_Click(object sender, RoutedEventArgs e)
        {
            StoreUserCredentials();
        }


        private void btnExtract_DealRiskCarriers_Click(object sender, RoutedEventArgs e)
        {

        }


        private void RiskView_Settings_Click(object sender, RoutedEventArgs e)
        {
            RiskViewSettings riskViewSettings = new RiskViewSettings();
            riskViewSettings.Show();
        }


        private void CustomerLimitUtilisation_Settings_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }


        private void DealRiskCarriers_Settings_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
