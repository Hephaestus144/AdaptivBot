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
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Xml.Linq;
using MaterialDesignThemes.Wpf;
using Outlook = NetOffice.OutlookApi;
using Action = System.Action;
using Brushes = System.Windows.Media.Brushes;


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
            NetworkChange.NetworkAddressChanged +=
                new NetworkAddressChangedEventHandler(AddressCallbackChange);
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

        public bool IsUsingEthernet(Logger _logger)
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up
                    && ni.NetworkInterfaceType.ToString()
                        .IndexOf("Ethernet", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        _logger.OkayText = "Ethernet connection restored.";
                    }));
                    return true;
                }
            }

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        _logger.WarningText = "Connected to WIFI.";
                        _logger.WarningText = "For stability use Ethernet instead.";
                    }));
                    return false;
                }
            }

            Dispatcher.BeginInvoke((Action)(() =>
            {
                _logger.ErrorText = "Network not detected.";
            }));
            return false;
        }


        void AddressCallbackChange(object sender, EventArgs e)
        {
            if (IsUsingEthernet(this.logger))
            {
                Dispatcher.BeginInvoke((Action) (() =>
                {
                    iconNetworkType.Kind = PackIconKind.EthernetCable;
                }));
            }
            else
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    iconNetworkType.Kind = PackIconKind.Wifi;
                    btnNetworkType.Background = Brushes.Orange;
                }));
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


        #region Credentials functions
        public bool StoreUserCredentials()
        {
            var credentialStore = new CredentialStore($"AdaptivBot{cmbBxAdaptivEnvironments.SelectedValue}");
            if (!credentialStore.credentialsFound && (bool)chkBxRememberMe.IsChecked)
            {
                credentialStore.Credentials.Username = txtUserName.Text;
                credentialStore.Credentials.Password = TxtPasswordBox.Password;
                credentialStore.Credentials.PersistanceType =
                    PersistanceType.LocalComputer;
                credentialStore.Credentials.Save();
                logger.OkayText = "Saving credentials...";
                return true;
            }

            if (credentialStore.credentialsFound
                && (credentialStore.Credentials.Username != txtUserName.Text
                || credentialStore.Credentials.Password != TxtPasswordBox.Password))
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

            if (TxtPasswordBox.Password == "")
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
                txtUserName.Text = credentialStore.Credentials.Username;
                TxtPasswordBox.Password = credentialStore.Credentials.Password;
            }
            else
            {
                txtUserName.Text = "";
                TxtPasswordBox.Password = "";
            }
        }

        #endregion Credentials functions
        

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var credentialStore = new CredentialStore("AdaptivBotProduction");
            if (credentialStore.credentialsFound)
            {
                txtUserName.Text = credentialStore.Credentials.Username;
                TxtPasswordBox.Password = credentialStore.Credentials.Password;
            }

            if (IsUsingEthernet(this.logger))
            {
                iconNetworkType.Kind = PackIconKind.EthernetCable;
            }
            else
            {
                iconNetworkType.Kind = PackIconKind.Wifi;
            }


            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            GlobalConfigValues.Instance.AdaptivBotDirectory
                = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder
                        .LocalApplicationData), "AdaptivBot");

            GlobalConfigValues.Instance.AdaptivBotConfigFilePath
                = Path.Combine(GlobalConfigValues.Instance.AdaptivBotDirectory,
                    "AdaptivBot.config");


            // TODO: Replace GlobalConfigValues with GlobalDataBindingValues
            GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath =
                GlobalConfigValues.Instance.AdaptivBotConfigFilePath;

            if (!Directory.Exists(GlobalConfigValues.Instance.AdaptivBotDirectory))
            {
                Directory.CreateDirectory(GlobalConfigValues.Instance.AdaptivBotDirectory);
            }

            if (!File.Exists(GlobalConfigValues.Instance.AdaptivBotConfigFilePath))
            {
                File.WriteAllText(
                    GlobalConfigValues.Instance.AdaptivBotConfigFilePath,
                    GlobalConfigValues.defaultConfigFileContent);
                GlobalConfigValues.CreatedConfigFile = YesNoMaybe.Yes;
            }

            var document =
                XDocument.Load(GlobalConfigValues.Instance.AdaptivBotConfigFilePath, LoadOptions.PreserveWhitespace);

            var configDocument =
                XDocument.Load(GlobalConfigValues.Instance.AdaptivBotConfigFilePath);

            if (document?.Root?.Element("GeneralSettings")?.Element("ExcelExecutablePath")?.Value != null)
            {
                if (File.Exists(GlobalConfigValues.possibleExcelPath1)
                    && document?.Root?.Element("GeneralSettings")?.Element("ExcelExecutablePath")?.Value == "")
                {
                    document.Root.Element("GeneralSettings").Element("ExcelExecutablePath").Value =
                        GlobalConfigValues.possibleExcelPath1;
                    document.Save(GlobalConfigValues.Instance.AdaptivBotConfigFilePath);
                    GlobalConfigValues.ExcelPathConfigured = YesNoMaybe.Yes;


                    GlobalConfigValues.excelPath
                        = configDocument.Root.Element("GeneralSettings").Element("ExcelExecutablePath").Value;
                }
                else if (document?.Root?.Element("GeneralSettings")?.Element("ExcelExecutablePath")?.Value == "")
                {
                    GlobalConfigValues.ExcelPathConfigured = YesNoMaybe.No;
                    GlobalConfigValues.excelPath
                        = configDocument.Root.Element("GeneralSettings").Element("ExcelExecutablePath").Value;
                }
                else
                {
                    GlobalConfigValues.ExcelPathConfigured = YesNoMaybe.Maybe;
                }
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


        #region functions for manipulating extractedfiles (DataGrid : "dgExtractedFiles")
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


        //TODO: Complete this.
        private void EmailExtractedFile(object sender, RoutedEventArgs e)
        {
            
        }
        #endregion functions for manipulating extractedfiles (DataGrid : "dgExtractedFiles")
    }
}
