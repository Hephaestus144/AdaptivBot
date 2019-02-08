using AutoIt;
using CredentialManagement;
using mshtml;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;
using Microsoft.Office.Interop.Outlook;
using Action = System.Action;

namespace AdaptivBot
{
    public sealed class GlobalDataBindingValues : INotifyPropertyChanged
    {
        private static readonly object padlock = new object();
        private static GlobalDataBindingValues instance = null;

        GlobalDataBindingValues()
        {
            // Used in the DatePickers. Users must not be able to select date after & including today's date.
            var displayEndDate = LocalDate.FromDateTime(DateTime.Now.AddDays(-1));
            switch (displayEndDate.DayOfWeek)
            {
                case IsoDayOfWeek.Saturday:
                    displayEndDate = displayEndDate.PlusDays(-1);
                    break;
                case IsoDayOfWeek.Sunday:
                    displayEndDate = displayEndDate.PlusDays(-2);
                    break;
            }
            this.DisplayDateEnd = displayEndDate.ToDateTimeUnspecified();
        }


        public static GlobalDataBindingValues Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new GlobalDataBindingValues();
                        }
                    }
                }

                return instance;
            }
        }
        

        private DateTime _displayDateEnd;

        public DateTime DisplayDateEnd
        {
            get => _displayDateEnd;
            set
            {
                if (_displayDateEnd != value)
                {
                    _displayDateEnd = value;
                    this.OnPropertyChanged(nameof(DisplayDateEnd));
                }
            }
        }


        private string _adaptivBotConfigFilePath;

        public string AdaptivBotConfigFilePath
        {
            get => _adaptivBotConfigFilePath;
            set
            {
                if (_adaptivBotConfigFilePath != value)
                {
                    _adaptivBotConfigFilePath = value;
                    this.OnPropertyChanged(nameof(AdaptivBotConfigFilePath));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<ExtractedFile> extractedFiles =
            new ObservableCollection<ExtractedFile>();

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

            dgExtractedFiles.ItemsSource = extractedFiles;

            // Deals with new windows created by browser.
            webBrowser = (webBrowserHost.Child as System.Windows.Forms.WebBrowser);
            axBrowser = (SHDocVw.WebBrowser_V1)webBrowser.ActiveXInstance;
            // listen for new windows
            axBrowser.NewWindow += axBrowser_NewWindow;
            webBrowser.DocumentCompleted += webBrowser_DocumentCompleted;

            logger = new Logger(rtbLogger);

            switch (GlobalConfigValues.CreatedConfigFile)
            {
                case YesNoMaybe.Yes:
                    logger.OkayText = $"Config file created : {GlobalConfigValues.Instance.AdaptivBotConfigFilePath}";
                    break;
                case YesNoMaybe.No:
                    logger.ErrorText = "Config file not created!";
                    break;
            }

            switch (GlobalConfigValues.ExcelPathConfigured)
            {
                case YesNoMaybe.No:
                    logger.OkayText
                        = "Excel path not configured. You will have to manually configure it in General Settings.";
                    break;
                case YesNoMaybe.Yes:
                    logger.ErrorText = "Excel path configured.";
                    break;
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
                logger.OkayText = "Saving credentials...";
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
                Dispatcher.BeginInvoke((Action)(() => { logger.OkayText = "Adaptiv already open."; }));
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
            AutoItX.Send("{TAB 3}");
            AutoItX.Send("{ENTER}");
            Dispatcher.Invoke((System.Action) (() =>
            {
                logger.OkayText = $"Saving CSV file for {instrumentBatch}...";
            }));

            AutoItX.WinWait("Save As", timeout: 20);
            AutoItX.WinActivate("Save As");

            AutoItX.Send("{DEL}");
            // TODO: Make the output file name a parameter.
            AutoItX.Send(
                $"STBUKTCPROD (Standard Bank Group) (Filtered){DateTime.Now:dd-MM-yyyy}.csv");
            AutoItX.Send("!d");
            AutoItX.Send("{DEL}");

            AutoItX.Send(
                $"\\\\pcibtighnas1\\CBSData\\Portfolio Analysis\\Data\\{instrumentBatch}\\SBG");
            AutoItX.Send("!s");
            AutoItX.Sleep(1000);
            var fileSaved = true;
            if (AutoItX.WinExists("Confirm Save As") != 0)
            {
                AutoItX.WinActivate("Confirm Save As");
                if (overrideExistingFile)
                {
                    AutoItX.Send("!y");
                    Dispatcher.Invoke((System.Action) (() =>
                    {
                        logger.WarningText =
                            $"Overriding existing file for {instrumentBatch}...";
                    }));
                }
                else
                {
                    AutoItX.Send("!n");
                    Dispatcher.Invoke((System.Action) (() =>
                    {
                        logger.WarningText =
                            $"File already exists for {instrumentBatch}.";
                    }));
                    AutoItX.WinWait("Save As", timeout: 20);
                    AutoItX.WinActivate("Save As");
                    AutoItX.Send("{TAB 10}");
                    AutoItX.Send("{ENTER}");
                    fileSaved = false;
                }
            }

            await Task.Run(() => Thread.Sleep(100));

            while (AutoItX.WinGetTitle("[ACTIVE]")
                .Contains(".csv from adaptiv.standardbank.co.za Completed"))
            {
                await Task.Run(() => Thread.Sleep(500));
            }

            var filePath =
                $"\\\\pcibtighnas1\\CBSData\\Portfolio Analysis\\Data\\{instrumentBatch}\\SBG\\STBUKTCPROD (Standard Bank Group) (Filtered){DateTime.Now:dd-MM-yyyy}.csv";

            while (!File.Exists(filePath))
            {
                await Task.Run(() => Thread.Sleep(500));
            }

            //TODO: alpha
            if (fileSaved)
            {
                var fileSize = (new FileInfo(filePath).Length >= 1048576)
                    ? $"{(new FileInfo(filePath).Length / 1048576):n}" + " MB"
                    : $"{(new FileInfo(filePath).Length / 1024):n}" + " KB";
                Dispatcher.Invoke((System.Action)(() =>
                {
                    extractedFiles.Add(new ExtractedFile()
                    {
                        FilePath = filePath, FileName = Path.GetFileName(filePath),
                        FileType = $"Risk View : {instrumentBatch}", FileSize = fileSize
                    });
                }));
            }
            // TODO: Checkbox to close window when complete.

        }


        public async void SaveDrcFile(bool overrideExistingFile)
        {
            AutoItX.WinWait("File Download", timeout: 20);
            AutoItX.WinActivate("File Download");
            AutoItX.Send("{TAB 3}");
            AutoItX.Send("{ENTER}");
            Dispatcher.Invoke((Action)(() =>
            {
                logger.OkayText = $"Saving CSV file for DRCs...";
            }));

            AutoItX.WinWait("Save As", timeout: 20);
            AutoItX.WinActivate("Save As");

            AutoItX.Send("{DEL}");
            // TODO: Make the output file name a parameter.
            AutoItX.Send(
                $"DRCs {DateTime.Now:yyyy-MM-dd}.csv");
            AutoItX.Send("!d");
            AutoItX.Send("{DEL}");

            var drcFolder =
                $"\\\\pcibtighnas1\\CBSData\\Portfolio Analysis\\Data\\DRC\\{DateTime.Now:MMMMyyyy}";
            if (!Directory.Exists(drcFolder))
            {
                Directory.CreateDirectory(drcFolder);
            }

            AutoItX.Send(drcFolder);

            AutoItX.Send("!s");
            AutoItX.Sleep(1000);
            var fileSaved = true;
            if (AutoItX.WinExists("Confirm Save As") != 0)
            {
                AutoItX.WinActivate("Confirm Save As");
                if (overrideExistingFile)
                {
                    AutoItX.Send("!y");
                    Dispatcher.Invoke((Action)(() =>
                    {
                        logger.WarningText =
                            $"Overriding existing file for DRCs...";
                    }));
                }
                else
                {
                    AutoItX.Send("!n");
                    Dispatcher.Invoke((Action)(() =>
                    {
                        logger.WarningText =
                            $"File already exists for DRCs.";
                    }));
                    AutoItX.WinWait("Save As", timeout: 20);
                    AutoItX.WinActivate("Save As");
                    AutoItX.Send("{TAB 10}");
                    AutoItX.Send("{ENTER}");
                    fileSaved = false;
                }
            }

            await Task.Run(() => Thread.Sleep(100));

            while (AutoItX.WinGetTitle("[ACTIVE]")
                .Contains(".csv from adaptiv.standardbank.co.za Completed"))
            {
                await Task.Run(() => Thread.Sleep(500));
            }

            var filePath =
                $"{drcFolder}\\DRCs {DateTime.Now:yyyy-MM-dd}.csv";

            while (!File.Exists(filePath))
            {
                await Task.Run(() => Thread.Sleep(500));
            }
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
            var targetFile = Path.ChangeExtension(csvFile, extTo);
            Process.Start($"\"{excelConverterPath}\"", $"-oice \"{csvFile}\" \"{targetFile}\"");
        }


        #region functions for manipulating extractefiles (DataGrid : "dgExtractedFiles")
        private void OpenExtractedFileContainingFolder(object sender, RoutedEventArgs e)
        {
            var startInfo = new ProcessStartInfo
            {
                Arguments = "/select, \"" + ((ExtractedFile)dgExtractedFiles.SelectedItem).FilePath + "\"",
                FileName = "explorer.exe"
            };
            Process.Start(startInfo);
        }


        private void OpenExtractedFile(object sender, RoutedEventArgs e)
        {
            var startInfo = new ProcessStartInfo
            {
                Arguments = "\"" + ((ExtractedFile)dgExtractedFiles.SelectedItem).FilePath + "\"",
                FileName = "Excel.exe"
            };
            Process.Start(startInfo);
        }


        private void DeleteExtractedFile(object sender, RoutedEventArgs e)
        {
            extractedFiles.Remove((ExtractedFile) dgExtractedFiles.SelectedItem);
        }


        private void EmailExtractedFile(object sender, RoutedEventArgs e)
        {
            var ol = new Microsoft.Office.Interop.Outlook.Application();
            MailItem mail = ol.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem) as Microsoft.Office.Interop.Outlook.MailItem;
            
        }
        #endregion functions for manipulating extractefiles (DataGrid : "dgExtractedFiles")
    }
}
