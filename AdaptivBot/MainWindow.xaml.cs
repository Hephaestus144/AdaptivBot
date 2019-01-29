using AutoIt;
using CredentialManagement;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using mshtml;
using Application = System.Windows.Application;
using WebBrowser = System.Windows.Controls.WebBrowser;

namespace AdaptivBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum AdaptivEnvironments
        {
            Production,
            Stress,
            T5,
            T8,
            T10
        };

        private Dictionary<string, string> AdaptivEnvironmentUrls
            = new Dictionary<string, string>()
            {
                ["Production"] =
                    "https://adaptiv.standardbank.co.za/Adaptiv/default.aspx"
            };

        private System.Windows.Forms.WebBrowser webBrowser;

        private Dictionary<string, string> injectedScripts = new Dictionary<string, string>();

        private SHDocVw.WebBrowser_V1 axBrowser = new SHDocVw.WebBrowser_V1();

        public MainWindow()
        {
            InitializeComponent();
            webBrowser = (webBrowserHost.Child as System.Windows.Forms.WebBrowser);

            axBrowser = (SHDocVw.WebBrowser_V1)webBrowser.ActiveXInstance;
            // listen for new windows
            axBrowser.NewWindow += axBrowser_NewWindow;
        }

        void axBrowser_NewWindow(
            string URL,
            int Flags,
            string TargetFrameName,
            ref object PostData,
            string Headers,
            ref bool Processed)
        {
            // cancel the PopUp event
            Processed = true;

            // send the popup URL to the WebBrowser control
            webBrowser.Navigate(URL);
        }

        #region Credential functions
        private bool StoreUserCredentials()
        {
            CredentialStore credentialStore = new CredentialStore("AdaptivBot" + cmbBxAdaptivEnvironments.SelectedValue.ToString());
            if (!credentialStore.credentialsFound && (bool)chkBxRememberMe.IsChecked)
            {
                credentialStore.credential.Username = txtUserName.Text;
                credentialStore.credential.Password = txtPasswordBox.Password;
                credentialStore.credential.PersistanceType =
                    PersistanceType.LocalComputer;
                credentialStore.credential.Save();
                return true;
            }
            else if (credentialStore.credentialsFound
                && (credentialStore.credential.Username != txtUserName.Text
                || credentialStore.credential.Password != txtPasswordBox.Password))
            {
                var window = new AlertUpdateUserCredentials();
                window.Show();
                return false;
                // Add message box to warn user that the credentials that have been entered are different 
                // to the saved credentials & would they like to save them?
            }

            return true;
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

        private void LoadAdaptivCredentials(object sender, SelectionChangedEventArgs e)
        {
            var credentialStore =
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

        #endregion Credential functions

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var credentialStore = new CredentialStore("AdaptivBotProduction");
            if (credentialStore.credentialsFound)
            {
                txtUserName.Text = credentialStore.credential.Username;
                txtPasswordBox.Password = credentialStore.credential.Password;
            }

        }



        private async void btnExtract_RiskView_Click(object sender, RoutedEventArgs e)
        {
            if (!StoreUserCredentials())
            {
                return;
            }
            // Wrap this in a function called login to Adaptiv or which checks if Adaptiv
            // has already been  logged into.
            // TODO: Couple to combobox Adaptiv environment.
            webBrowser.Navigate(AdaptivEnvironmentUrls[cmbBxAdaptivEnvironments.SelectedValue.ToString()]);
            var username = txtUserName.Text;
            var password = txtPasswordBox.Password;

            await Task.Run(() => EnterAdaptivCredentials(username, password));

            await Task.Run(() => Thread.Sleep(5000));

            // delegates
            InjectJavascript(nameof(JsScripts.OpenRiskView), JsScripts.OpenRiskView);
            webBrowser.Document.InvokeScript(nameof(JsScripts.OpenRiskView));

            
            await Task.Run(() => Thread.Sleep(3000));

            InjectJavascript(
                nameof(JsScripts.FilterRiskViewOnInstruments),
                JsScripts.FilterRiskViewOnInstruments);

            webBrowser.Document.InvokeScript(
                nameof(JsScripts.FilterRiskViewOnInstruments),
                new object[] { InstrumentLists.bondInstruments });

            await Task.Run(() => Thread.Sleep(10000));

            InjectJavascript(nameof(JsScripts.ExportToCsv), JsScripts.ExportToCsv);
            webBrowser.Document.InvokeScript(nameof(JsScripts.ExportToCsv));

            await Task.Run(() => Thread.Sleep(5000));

            foreach (HtmlElement link in webBrowser.Document.GetElementsByTagName("A"))
            {
                if (link.InnerText.Equals("exported file link"))
                    link.InvokeMember("Click");
            }

            await Task.Run(() => SaveFile());
        }


        private void SaveFile()
        {
            AutoItX.WinWait("File Download", timeout: 20);
            AutoItX.WinActivate("File Download");
            AutoItX.Send("{TAB}");
            AutoItX.Send("{TAB}");
            AutoItX.Send("{TAB}");
            AutoItX.Send("{ENTER}");
            //logger.StatusText = "Saving CSV file...";
            AutoItX.WinWait("Save As", timeout: 20);
            AutoItX.WinActivate("Save As");

            AutoItX.Send("{DEL}");
            // TODO: Make the output file name a parameter.
            AutoItX.Send($"STBUKTCPROD (Standard Bank Group) (Filtered){DateTime.Now:dd-MM-yyyy}.csv");
            AutoItX.Send("!d");
            AutoItX.Send("{DEL}");

            var instrumentType = "Bond";

            AutoItX.Send($"\\\\pcibtighnas1\\CBSData\\Portfolio Analysis\\Data\\{instrumentType}\\SBG");
            AutoItX.Send("!s");

            AutoItX.WinWait("Download Complete", timeout: 20);
            AutoItX.WinActivate("Download Complete");
            AutoItX.Send("{SPACE}");

        }

        private void InjectJavascript(string scriptName, string script)
        {
            if (!injectedScripts.ContainsKey(scriptName))
            {
                var doc = (HtmlDocument) webBrowser.Document;
                var headElement = doc.GetElementsByTagName("head")[0];
                var scriptElement = doc.CreateElement("script");
                var element = (IHTMLScriptElement) scriptElement.DomElement;
                element.text = script;
                headElement.AppendChild(scriptElement);
                injectedScripts.Add(scriptName, script);
            }
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
            var riskViewSettings = new RiskViewSettings();
            riskViewSettings.Show();
        }


        private void CustomerLimitUtilisation_Settings_Click(object sender, RoutedEventArgs e)
        {
            //CustomerLimitUtilisationSettings window =
            //    new CustomerLimitUtilisationSettings();
            //window.Show();
        }


        private void DealRiskCarriers_Settings_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
