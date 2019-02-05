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
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf.Transitions;
using WebBrowser = System.Windows.Controls.WebBrowser;
using NodaTime;
using Duration = System.Windows.Duration;


namespace AdaptivBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool completedLoading = false;

        public enum AdaptivEnvironments
        {
            Production,
            Stress,
            T5,
            T8,
            T10
        };

        public Dictionary<string, string> AdaptivEnvironmentUrls
            = new Dictionary<string, string>()
            {
                ["Production"]
                    = "https://adaptiv.standardbank.co.za/Adaptiv/default.aspx",
                ["StressEnvironment"]
                    = "https://adaptivstressenv.standardbank.co.za/Adaptiv/default.aspx"
            };


        public System.Windows.Forms.WebBrowser webBrowser;

        public Dictionary<string, string> injectedScripts
            = new Dictionary<string, string>();

        public SHDocVw.WebBrowser_V1 axBrowser = new SHDocVw.WebBrowser_V1();

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


        public void axBrowser_NewWindow(
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
        public bool StoreUserCredentials()
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
                // TODO: Add message box to warn user that the credentials that have been entered are different 
                // to the saved credentials & would they like to save them?
            }

            if (txtUserName.Text == "")
            {
                logger.ErrorText = "User name blank.";
                return false;
            }

            if (txtPasswordBox.Password == "")
            {
                logger.ErrorText = "Password blank.";
                return false;
            }

            return true;
        }

        public void EnterAdaptivCredentials(string username, string password)
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

        public void LoadAdaptivCredentials(object sender, SelectionChangedEventArgs e)
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
            
            var localDate = LocalDate.FromDateTime(DateTime.Now.AddDays(-1));
            if (localDate.DayOfWeek == IsoDayOfWeek.Saturday)
            {
                localDate = localDate.PlusDays(-1);
            }
            else if (localDate.DayOfWeek == IsoDayOfWeek.Sunday)
            {
                localDate = localDate.PlusDays(-2);
            }
            
        }

        public async void OpenAdaptivAndLogin(
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


        public async void WaitForBrowser()
        {
            while (!completedLoading)
            {
                await Task.Run(() => Thread.Sleep(1000));
            }
            await Task.Run(() => Thread.Sleep(1000));
            completedLoading = false;
        }


        public async void SaveFile(string instrumentBatch, bool overrideExistingFile)
        {
            AutoItX.WinWait("File Download", timeout: 20);
            AutoItX.WinActivate("File Download");
            AutoItX.Send("{TAB}");
            AutoItX.Send("{TAB}");
            AutoItX.Send("{TAB}");
            AutoItX.Send("{ENTER}");
            Dispatcher.Invoke((Action)(() =>
            {
                logger.OkayText = $"Saving CSV file for {instrumentBatch}.";
            })); 

            AutoItX.WinWait("Save As", timeout: 20);
            AutoItX.WinActivate("Save As");

            AutoItX.Send("{DEL}");
            // TODO: Make the output file name a parameter.
            AutoItX.Send($"STBUKTCPROD (Standard Bank Group) (Filtered){DateTime.Now:dd-MM-yyyy}.csv");
            AutoItX.Send("!d");
            AutoItX.Send("{DEL}");

            AutoItX.Send($"\\\\pcibtighnas1\\CBSData\\Portfolio Analysis\\Data\\{instrumentBatch}\\SBG");
            AutoItX.Send("!s");
            AutoItX.Sleep(1000);
            if (AutoItX.WinExists("Confirm Save As") != 0)
            {
                AutoItX.WinActivate("Confirm Save As");
                if (overrideExistingFile)
                {
                    AutoItX.Send("!y");
                    Dispatcher.Invoke((Action) (() =>
                    {
                        logger.WarningText =
                            $"Overriding existing file for {instrumentBatch}.";
                    }));
                }
                else
                {
                    AutoItX.Send("!n");
                    Dispatcher.Invoke((Action)(() =>
                    {
                        logger.WarningText =
                            $"File already exists for {instrumentBatch}.";
                    }));
                    AutoItX.WinWait("Save As", timeout: 20);
                    AutoItX.WinActivate("Save As");
                    AutoItX.Send("{TAB}");
                    AutoItX.Send("{TAB}");
                    AutoItX.Send("{TAB}");
                    AutoItX.Send("{TAB}");
                    AutoItX.Send("{TAB}");
                    AutoItX.Send("{TAB}");
                    AutoItX.Send("{TAB}");
                    AutoItX.Send("{TAB}");
                    AutoItX.Send("{TAB}");
                    AutoItX.Send("{TAB}");
                    AutoItX.Send("{ENTER}");
                }
            }

            await Task.Run(() => Thread.Sleep(100));

            while (AutoItX.WinGetTitle("[ACTIVE]").Contains(".csv from adaptiv.standardbank.co.za Completed"))
            { 
                await Task.Run(() => Thread.Sleep(500));
            }
            // TODO: Checkbox to close window when complete.

        }


        public void InjectJavascript(string scriptName, string script)
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

        public async void btnExtract_RiskView_Click(object sender, RoutedEventArgs e)
        {
            GlobalConfigValues.Instance.extractionStartTime = DateTime.Now;
            
            logger.NewExtraction("Risk View Reports Extraction Started");

            if (!StoreUserCredentials())
            {
                return;
            }
            
            // Wrap this in a function called login to Adaptiv or which checks if Adaptiv
            // has already been  logged into.

            // TODO: Use binding here.
            var username = txtUserName.Text;
            var password = txtPasswordBox.Password;

             
            var currentAdaptivEnvironment = cmbBxAdaptivEnvironments.SelectedValue.ToString();
            int numberOfFailedExtractions = 0;
            int numberOfSuccessfulExtractions = 0;
            foreach (var instrumentBatch in InstrumentLists.instruments.Keys)
            {
                for (var errorCount = 0; errorCount < 3; errorCount++)
                {
                    try
                    {
                        await Task.Run(() =>
                            OpenAdaptivAndLogin(username, password,
                                currentAdaptivEnvironment));

                        #region wait for browser

                        while (!completedLoading)
                        {
                            await Task.Run(() => Thread.Sleep(100));
                        }

                        await Task.Run(() => Thread.Sleep(2000));
                        completedLoading = false;

                        #endregion wait for browser

                        #region wait for browser

                        while (!completedLoading)
                        {
                            await Task.Run(() => Thread.Sleep(100));
                        }

                        await Task.Run(() => Thread.Sleep(2000));
                        completedLoading = false;

                        #endregion wait for browser

                        InjectJavascript(nameof(JsScripts.OpenRiskView),
                            JsScripts.OpenRiskView);

                        await Task.Run(() => Thread.Sleep(1000));
                        webBrowser.Document.InvokeScript(nameof(JsScripts.OpenRiskView));

                        #region wait for browser

                        for (int i = 0; i < 3; i++)
                        {
                            while (!completedLoading)
                            {
                                await Task.Run(() => Thread.Sleep(100));
                            }

                            completedLoading = false;
                        }

                        await Task.Run(() => Thread.Sleep(2000));

                        #endregion wait for browser

                        InjectJavascript(
                            nameof(JsScripts.FilterRiskViewOnInstruments),
                            JsScripts.FilterRiskViewOnInstruments);
                        webBrowser.Document.InvokeScript(
                            nameof(JsScripts.FilterRiskViewOnInstruments),
                            new object[] {InstrumentLists.instruments[instrumentBatch]});


                        #region wait for browser

                        while (!completedLoading)
                        {
                            await Task.Run(() => Thread.Sleep(100));
                        }

                        await Task.Run(() => Thread.Sleep(1000));
                        completedLoading = false;

                        #endregion wait for browser


                        InjectJavascript(nameof(JsScripts.ExportToCsv),
                            JsScripts.ExportToCsv);
                        webBrowser.Document.InvokeScript(nameof(JsScripts.ExportToCsv));

                        await Task.Run(() => Thread.Sleep(500));

                        #region wait for browser

                        while (!completedLoading)
                        {
                            await Task.Run(() => Thread.Sleep(100));
                        }

                        await Task.Run(() => Thread.Sleep(1000));
                        completedLoading = false;

                        #endregion wait for browser

                        while (webBrowser.Document.GetElementsByTagName("A").Count == 0)
                        {
                            await Task.Run(() => Thread.Sleep(100));
                        }

                        foreach (HtmlElement link in webBrowser.Document
                            .GetElementsByTagName("A"))
                        {
                            if (link.InnerText.Equals("exported file link"))
                                link.InvokeMember("Click");
                        }

                        var overrideExistingFile = true;
                            //(bool) chkBxOverrideExistingFiles.IsChecked;
                        await Task.Run(() =>
                            SaveFile(instrumentBatch, overrideExistingFile));
                        numberOfSuccessfulExtractions++;
                        break;
                    }
                    catch (Exception)
                    {
                        if (errorCount < 2)
                        {
                            logger.ErrorText = $"Something failed for {instrumentBatch} extraction. Trying again. Attempt number: {++errorCount}";
                        }
                        else
                        {
                            numberOfFailedExtractions++;
                            logger.ErrorText = $"{instrumentBatch} extraction failed 3 times. Moving on to next instrument set.";
                        }
                    }
                }
            }

            logger.OkayText = "Completed";
            logger.OkayText =
                $"Number of successful extractions: {numberOfSuccessfulExtractions}";
            logger.ErrorText =
                $"Number of failed extractions: {numberOfFailedExtractions}";
            GlobalConfigValues.Instance.extractionEndTime = DateTime.Now;
            logger.OkayText =
                $"Extraction took: {(GlobalConfigValues.Instance.extractionEndTime - GlobalConfigValues.Instance.extractionStartTime).Minutes} minutes";
        }

      
        public async void btnExtract_CustomerLimitUtilisation_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Switch to Customer Limit function date
            DateTime? date = (DateTime)DateTime.Now;// datePicker.SelectedDate;
            if (date is null)
            {
                logger.ErrorText = "Please select a date for extraction.";
                return;
            }

            if (!StoreUserCredentials())
            {
                return;
            }

            logger.NewExtraction("Customer Limit Utilisation Report Extraction Started");

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
            await Task.Run(() => Thread.Sleep(1000));
            completedLoading = false;
            while (!completedLoading)
            {
                await Task.Run(() => Thread.Sleep(100));
            }
            await Task.Run(() => Thread.Sleep(1000));
            completedLoading = false;
            #endregion wait for browser

            InjectJavascript(
                nameof(JsScripts.OpenCustomerLimitUtilisationReport),
                JsScripts.OpenCustomerLimitUtilisationReport);

            webBrowser.Document?.InvokeScript(nameof(JsScripts.OpenCustomerLimitUtilisationReport));

            #region wait for browser
            while (!completedLoading)
            {
                await Task.Run(() => Thread.Sleep(100));
            }
            await Task.Run(() => Thread.Sleep(1000));
            completedLoading = false;
            while (!completedLoading)
            {
                await Task.Run(() => Thread.Sleep(100));
            }
            await Task.Run(() => Thread.Sleep(1000));
            completedLoading = false;
            #endregion wait for browser


            InjectJavascript(
                nameof(JsScripts.FilterCustomerLimitUtilisationReport),
                JsScripts.FilterCustomerLimitUtilisationReport);
            webBrowser.Document?.InvokeScript(nameof(JsScripts.FilterCustomerLimitUtilisationReport));

            #region wait for browser
            completedLoading = false;
            while (!completedLoading)
            {
                await Task.Run(() => Thread.Sleep(100));
            }
            await Task.Run(() => Thread.Sleep(1000));
            completedLoading = false;
            #endregion wait for browser

            InjectJavascript(
                nameof(JsScripts.ChooseCustomerLimitUtilisationReport),
                JsScripts.ChooseCustomerLimitUtilisationReport);
            await Task.Run(() => Thread.Sleep(1000));
            webBrowser.Document?.InvokeScript(nameof(JsScripts.ChooseCustomerLimitUtilisationReport));

            injectedScripts.Clear();


            #region wait for browser
            completedLoading = false;
            while (!completedLoading)
            {
                await Task.Run(() => Thread.Sleep(100));
            }
            await Task.Run(() => Thread.Sleep(1000));
            completedLoading = false;
            #endregion wait for browser
            await Task.Run(() => Thread.Sleep(1000));


            InjectJavascript(
                nameof(JsScripts.SelectCustomerLimitUtilisationReportDate),
                JsScripts.SelectCustomerLimitUtilisationReportDate);

            webBrowser.Document?.InvokeScript(
                nameof(JsScripts.SelectCustomerLimitUtilisationReportDate),
                new object[] { ((DateTime)date).ToString("dd/MM/yyyy") });
            
            #region wait for browser
            completedLoading = false;
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

            webBrowser.Document?.InvokeScript(
                nameof(JsScripts.GenerateCustomerLimitUtilisationReport));

            while (webBrowser.Document?.GetElementsByTagName("img").Count < 5)
            {
                await Task.Run(() => Thread.Sleep(1000));
            }
            await Task.Run(() => Thread.Sleep(3000));

            InjectJavascript(
                nameof(JsScripts.ExportCustomerLimitUtilisationReportToCsv),
                JsScripts.ExportCustomerLimitUtilisationReportToCsv);

            webBrowser.Document?.InvokeScript(nameof(JsScripts.ExportCustomerLimitUtilisationReportToCsv));

            var overrideExistingFile = true;
                //(bool)chkBxOverrideExistingFiles.IsChecked;

            await Task.Run(() => Thread.Sleep(1000));
            await Task.Run(() => SaveCustomerLimitUtilisationReport((DateTime)date, overrideExistingFile));

            var csvFile = $"\\\\pcibtighnas1\\CBSData\\Portfolio Analysis\\Data\\Cust Util\\SBG\\CustomerLimitUtil {date:dd.MM.yyyy}.csv";

            ConvertWorkbookFormats(csvFile, ".csv", ".xlsx");
            logger.OkayText = "Complete";
        }

        public async void SaveCustomerLimitUtilisationReport(DateTime date, bool overrideExistingFile)
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

            AutoItX.Sleep(1000);
            if (AutoItX.WinExists("Confirm Save As") != 0)
            {
                AutoItX.WinActivate("Confirm Save As");
                if (overrideExistingFile)
                {
                    AutoItX.Send("!y");
                    Dispatcher.Invoke((Action)(() =>
                    {
                        logger.WarningText =
                            $"Overriding existing file.";
                    }));
                }
                else
                {
                    AutoItX.Send("!n");
                    Dispatcher.Invoke((Action)(() =>
                    {
                        logger.WarningText =
                            $"File already exists.";
                    }));
                    AutoItX.WinWait("Save As", timeout: 20);
                    AutoItX.WinActivate("Save As");
                    AutoItX.Send("{TAB}");
                    AutoItX.Send("{TAB}");
                    AutoItX.Send("{TAB}");
                    AutoItX.Send("{TAB}");
                    AutoItX.Send("{TAB}");
                    AutoItX.Send("{TAB}");
                    AutoItX.Send("{TAB}");
                    AutoItX.Send("{TAB}");
                    AutoItX.Send("{TAB}");
                    AutoItX.Send("{TAB}");
                    AutoItX.Send("{ENTER}");
                }
            }

            await Task.Run(() => Thread.Sleep(100));

            while (AutoItX.WinGetTitle("[ACTIVE]").Contains("Utilisation"))
            {
                await Task.Run(() => Thread.Sleep(500));
            }
        }


        public static async void ConvertWorkbookFormats(string csvFile, string extFrom, string extTo)
        {
            while (!(File.Exists(csvFile)))
            {
                await Task.Run(() => Thread.Sleep(1000));
            }

            var excelConverterPath = Path.Combine(Path.GetDirectoryName(GlobalConfigValues.excelPath), "excelcnv.exe");
            var targetFile = Path.ChangeExtension(csvFile, "." + extTo);
            Process.Start($"\"{excelConverterPath}\"", $"-oice \"{csvFile}\" \"{targetFile}\"");
        }
    }
}
