using AdaptivBot.SettingForms;
using AutoIt;
using CredentialManagement;
using mshtml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;


namespace AdaptivBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool completedLoading = false;

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

        private Dictionary<string, string> injectedScripts
            = new Dictionary<string, string>();

        private SHDocVw.WebBrowser_V1 axBrowser = new SHDocVw.WebBrowser_V1();

        public Logger logger;

        public MainWindow()
        {
            InitializeComponent();
            webBrowser = (webBrowserHost.Child as System.Windows.Forms.WebBrowser);

            axBrowser = (SHDocVw.WebBrowser_V1)webBrowser.ActiveXInstance;
            // listen for new windows
            axBrowser.NewWindow += axBrowser_NewWindow;

            webBrowser.DocumentCompleted += webBrowser_DocumentCompleted;
            logger = new Logger(rtbLogger);

            if (GlobalConfigValues.createdConfigFile == YesNoMaybe.Yes)
            {
                logger.OkayText = "Config file created.";
            }
            else if (GlobalConfigValues.createdConfigFile == YesNoMaybe.No)
            {
                logger.ErrorText = "Config file not created!";
            }

            if (GlobalConfigValues.excelPathConfigured == YesNoMaybe.No)
            {
                logger.OkayText
                    = "Excel path not configured. You will have to manually configure it in general settings.";
            }
            else if (GlobalConfigValues.excelPathConfigured == YesNoMaybe.Yes)
            {
                logger.ErrorText = "Excel path configured.";
            }

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
            webBrowser.DocumentCompleted += webBrowser_DocumentCompleted;
        }


        #region Credential functions
        private bool StoreUserCredentials()
        {
            var credentialStore = new CredentialStore($"AdaptivBot{cmbBxAdaptivEnvironments.SelectedValue}");
            if (!credentialStore.credentialsFound && (bool)chkBxRememberMe.IsChecked)
            {
                credentialStore.credential.Username = txtUserName.Text;
                credentialStore.credential.Password = txtPasswordBox.Password;
                credentialStore.credential.PersistanceType =
                    PersistanceType.LocalComputer;
                credentialStore.credential.Save();
                return true;
            }

            if (credentialStore.credentialsFound
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
            AutoItX.Sleep(3000);
            if (AutoItX.WinExists("Windows Security") != 0)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    logger.OkayText = "Entering credentials...";
                }));
                AutoItX.WinActivate("Windows Security");
                AutoItX.Send(username);
                AutoItX.Send("{TAB}");
                AutoItX.Send(password);
                AutoItX.Send("{TAB}");
                AutoItX.Send("{ENTER}");
            }
            else
            {
                Dispatcher.BeginInvoke((Action)(() => { logger.OkayText = "Adaptiv already open"; }));
            }

            Dispatcher.BeginInvoke((Action)(() => { logger.OkayText = "Acknowledging disclaimer..."; }));
            AutoItX.WinWait("Adaptiv Disclaimer -- Webpage Dialog");
            AutoItX.WinActivate("Adaptiv Disclaimer -- Webpage Dialog");
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

        private async void OpenAdaptivAndLogin(
            string username,
            string password,
            string currentAdaptivEnvironment)
        {
            webBrowser.Navigate(
                AdaptivEnvironmentUrls[
                    currentAdaptivEnvironment]);
            injectedScripts.Clear();

            await Task.Run(() => EnterAdaptivCredentials(username, password));
        }


        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //Check if page is fully loaded or not
            if (this.webBrowser.IsBusy || this.webBrowser.ReadyState != WebBrowserReadyState.Complete)
                return;
            else
            {
                completedLoading = true;
            }
            //Action to be taken on page loading completion
        }


        private async void WaitForBrowser()
        {
            while (!completedLoading)
            {
                await Task.Run(() => Thread.Sleep(1000));
            }
            await Task.Run(() => Thread.Sleep(1000));
            completedLoading = false;
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

            // TODO: Use binding here.
            var username = txtUserName.Text;
            var password = txtPasswordBox.Password;

             
            var currentAdaptivEnvironment = cmbBxAdaptivEnvironments.SelectedValue.ToString();

            foreach (var instrumentBatch in InstrumentLists.instruments.Keys)
            {
                await Task.Run(() => OpenAdaptivAndLogin(username, password, currentAdaptivEnvironment));

                #region wait for browser
                //while (!completedLoading)
                //{
                //    await Task.Run(() => Thread.Sleep(100));
                //}

                if (webBrowser.Document?.GetElementById("MainFrame") is null)
                {
                    await Task.Run(() => Thread.Sleep(100));
                }
                //await Task.Run(() => Thread.Sleep(3000));
                completedLoading = false;
                #endregion wait for browser

                InjectJavascript(nameof(JsScripts.OpenRiskView), JsScripts.OpenRiskView);

                #region wait for browser
                while (!completedLoading)
                {
                    await Task.Run(() => Thread.Sleep(100));
                }
                await Task.Run(() => Thread.Sleep(1000));
                completedLoading = false;
                #endregion wait for browser

                webBrowser.Document.InvokeScript(nameof(JsScripts.OpenRiskView));

                #region wait for browser
                while (!completedLoading)
                {
                    await Task.Run(() => Thread.Sleep(100));
                }
                await Task.Run(() => Thread.Sleep(1000));
                completedLoading = false;
                #endregion wait for browser

                InjectJavascript(
                        nameof(JsScripts.FilterRiskViewOnInstruments),
                        JsScripts.FilterRiskViewOnInstruments);
                webBrowser.Document.InvokeScript(
                    nameof(JsScripts.FilterRiskViewOnInstruments),
                    new object[] { InstrumentLists.instruments[instrumentBatch] });

                
                #region wait for browser
                while (!completedLoading)
                {
                    await Task.Run(() => Thread.Sleep(100));
                }
                await Task.Run(() => Thread.Sleep(1000));
                completedLoading = false;
                #endregion wait for browser

                InjectJavascript(nameof(JsScripts.ExportToCsv), JsScripts.ExportToCsv);
                webBrowser.Document.InvokeScript(nameof(JsScripts.ExportToCsv));

                await Task.Run(() => Thread.Sleep(5000));

                foreach (HtmlElement link in webBrowser.Document.GetElementsByTagName("A"))
                {
                    if (link.InnerText.Equals("exported file link"))
                        link.InvokeMember("Click");
                }

                await Task.Run(() => SaveFile(instrumentBatch));
            }
        }


        private void SaveFile(string instrumentBatch)
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

            AutoItX.Send($"\\\\pcibtighnas1\\CBSData\\Portfolio Analysis\\Data\\{instrumentBatch}\\SBG");
            AutoItX.Send("!s");

            // TODO: Checkbox to close window when complete.

            //AutoItX.WinWait("Download Complete", timeout: 30);
            //AutoItX.WinActivate("Download Complete");
            //AutoItX.Send("{SPACE}");
        }


        private void InjectJavascript(string scriptName, string script)
        {
            if (!injectedScripts.ContainsKey(scriptName))
            {
                var doc = (HtmlDocument) webBrowser.Document;
                var headElement = doc?.GetElementsByTagName("head")[0];
                var scriptElement = doc?.CreateElement("script");
                var element = (IHTMLScriptElement) scriptElement?.DomElement;
                if (!(element is null))
                {
                    element.text = script;
                    headElement.AppendChild(scriptElement);
                    injectedScripts.Add(scriptName, script);
                }
            }
        }
        

        private async void btnExtract_CustomerLimitUtilisation_Click(object sender, RoutedEventArgs e)
        {
            var date = datePicker.SelectedDate;
            if (date is null)
            {
                return;
            }

            if (!StoreUserCredentials())
            {
                return;
            }

            // Wrap this in a function called login to Adaptiv or which checks if Adaptiv
            // has already been  logged into.
            // TODO: Couple to combobox Adaptiv environment.

            // TODO: Use binding here.
            var username = txtUserName.Text;
            var password = txtPasswordBox.Password;


            var currentAdaptivEnvironment =
                cmbBxAdaptivEnvironments.SelectedValue.ToString();

            StoreUserCredentials();

            await Task.Run(() => OpenAdaptivAndLogin(username, password, currentAdaptivEnvironment));

            #region wait for browser
            while (!completedLoading)
            {
                await Task.Run(() => Thread.Sleep(100));
            }
            await Task.Run(() => Thread.Sleep(5000));
            completedLoading = false;
            #endregion wait for browser

            InjectJavascript(
                nameof(JsScripts.OpenCustomerLimitUtilisationReport),
                JsScripts.OpenCustomerLimitUtilisationReport);

            webBrowser.Document.InvokeScript(nameof(JsScripts.OpenCustomerLimitUtilisationReport));

            // TODO: What is this doing
            InjectJavascript(
                nameof(JsScripts.SelectCustomerLimitUtilisationReport),
                JsScripts.SelectCustomerLimitUtilisationReport);

            #region wait for browser
            while (!completedLoading)
            {
                await Task.Run(() => Thread.Sleep(100));
            }
            await Task.Run(() => Thread.Sleep(15000));
            completedLoading = false;
            #endregion wait for browser

            webBrowser.Document.InvokeScript(nameof(JsScripts.SelectCustomerLimitUtilisationReport));

            #region wait for browser
            while (!completedLoading)
            {
                await Task.Run(() => Thread.Sleep(100));
            }
            await Task.Run(() => Thread.Sleep(1000));
            completedLoading = false;
            #endregion wait for browser


            InjectJavascript(
                nameof(JsScripts.GenerateCustomerLimitUtilisationReport),
                JsScripts.GenerateCustomerLimitUtilisationReport);

            #region wait for browser
            //while (!completedLoading)
            //{
            //    await Task.Run(() => Thread.Sleep(100));
            //}
            await Task.Run(() => Thread.Sleep(1000));
            completedLoading = false;
            #endregion wait for browser

            webBrowser.Document.InvokeScript(
                nameof(JsScripts.GenerateCustomerLimitUtilisationReport),
                new object[] { ((DateTime)date).ToString("dd/MM/yyyy")});


            InjectJavascript(
                nameof(JsScripts.ExportCustomerLimitUtilisationReportToCsv),
                JsScripts.ExportCustomerLimitUtilisationReportToCsv);

            #region wait for browser
            while (!completedLoading)
            {
                await Task.Run(() => Thread.Sleep(100));
            }
            await Task.Run(() => Thread.Sleep(40000));
            completedLoading = false;
            #endregion wait for browser

            webBrowser.Document.InvokeScript(nameof(JsScripts.ExportCustomerLimitUtilisationReportToCsv));

            await Task.Run(() => SaveCustomerLimitUtilisationReport((DateTime)date));

            var workbookPath = $"\\\\pcibtighnas1\\CBSData\\Portfolio Analysis\\Data\\Cust Util\\SBG\\CustomerLimitUtil {date:dd.MM.yyyy}.csv";

            
            ConvertWorkbookFormats(workbookPath, ".csv", ".xlsx");
        }

        private void SaveCustomerLimitUtilisationReport(DateTime date)
        {
            AutoItX.WinWait("File Download", timeout: 200);
            AutoItX.WinActivate("File Download");
            AutoItX.Send("!s");
            AutoItX.WinWait("Save As", timeout: 20);
            AutoItX.WinActivate("Save As");

            AutoItX.Send("{DEL}");
            // TODO: Make the output file name a parameter.
            AutoItX.Send($"CustomerLimitUtil {date:dd.MM.yyyy}.csv");
            AutoItX.Send("!d");
            AutoItX.Send("{DEL}");

            AutoItX.Send($"\\\\pcibtighnas1\\CBSData\\Portfolio Analysis\\Data\\Cust Util\\SBG");
            AutoItX.Send("!s");
        }

        private void btnExtract_DealRiskCarriers_Click(object sender, RoutedEventArgs e)
        {

        }
        public static void ConvertWorkbookFormats(string workbookPath, string extFrom, string extTo)
        {

            // TODO: must just act on single file.
            var xlsFiles = Directory.GetFiles(workbookPath, "*." + extFrom, SearchOption.AllDirectories);
            var sourceFiles = new List<string>(xlsFiles);
            sourceFiles.RemoveAll(x => x.EndsWith("." + extTo));

            var processes = new List<Process>(Process.GetProcesses());
            //TODO: "Find excel directory"
            var excelDirectory
                = Path.GetDirectoryName(
                    processes.First(x => x.ProcessName == "EXCEL").MainModule.FileName);

            var excelcnvPath = Path.Combine(excelDirectory, "excelcnv.exe");
            foreach (var sourceFile in sourceFiles)
            {
                var targetFile = Path.ChangeExtension(sourceFile, "." + extTo);
                Process.Start($"\"{excelcnvPath}\"", $"-oice \"{sourceFile}\" \"{targetFile}\"");
            }
        }


        private void RiskView_Settings_Click(object sender, RoutedEventArgs e)
        {
            var riskViewSettings = new RiskViewSettings();
            riskViewSettings.Show();
        }


        private void CustomerLimitUtilisation_Settings_Click(object sender, RoutedEventArgs e)
        {
            var window = new CustomerLimitUtilisationSettings();
            window.Show();
        }


        private void DealRiskCarriers_Settings_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void btnGeneralSettings_Click(object sender, RoutedEventArgs e)
        {
            var window = new GeneralSettings();
            window.Show();
        }
    }
}
