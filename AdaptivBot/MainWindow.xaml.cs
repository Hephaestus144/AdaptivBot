using MaterialDesignThemes.Wpf;
using mshtml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml.Linq;
using Action = System.Action;
using Brushes = System.Windows.Media.Brushes;


namespace AdaptivBot
{
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


        public System.Windows.Forms.WebBrowser WebBrowser;

        public Dictionary<string, string> InjectedScripts
            = new Dictionary<string, string>();

        public SHDocVw.WebBrowser_V1 axBrowser = new SHDocVw.WebBrowser_V1();

        public Logger Logger;


        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            CredentialStore.Instance.Target = "AdaptivBotProduction";
            IconNetworkType.Kind = IsUsingEthernet(this.Logger, true)
                ? PackIconKind.Network
                : PackIconKind.Wifi;

            #region config file & variables setup
            GlobalDataBindingValues.Instance.AdaptivBotDirectory
                = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder
                        .LocalApplicationData), "AdaptivBot");

            GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath
                = Path.Combine(GlobalDataBindingValues.Instance.AdaptivBotDirectory,
                    "AdaptivBot.config");

            if (!Directory.Exists(GlobalDataBindingValues.Instance.AdaptivBotDirectory))
            {
                Directory.CreateDirectory(GlobalDataBindingValues.Instance.AdaptivBotDirectory);
            }

            if (!File.Exists(GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath))
            {
                try
                {
                    File.WriteAllText(
                        GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath,
                        Properties.Resources.AdaptivBot);
                    Logger.OkayText = $"Config file created : {GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath}";
                }
                catch (Exception configFileCreationException)
                {
                    Logger.ErrorText = $"Exception caught: {configFileCreationException.Message}";
                    Logger.ErrorText = "Config file not created! Limited functionality.";
                }
            }
            else
            {
                Logger.OkayText= $"Config file found : " +
                    $"{GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath}";
            }

            var document
                = XDocument.Load(GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath,
                    LoadOptions.PreserveWhitespace);

            if (document.Root?.Element("GeneralSettings")?.Element("ExcelExecutablePath")?.Value != null)
            {
                if (document.Root?.Element("GeneralSettings")
                        ?.Element("ExcelExecutablePath")?.Value?.Length == 0)
                {
                    var excelPathFound = false;
                    foreach (var path in GlobalDataBindingValues.PossibleExcelPaths)
                    {
                        if (File.Exists(path))
                        {
                            document.Root.Element("GeneralSettings")
                                    .Element("ExcelExecutablePath").Value
                                = path;

                            document.Save(GlobalDataBindingValues.Instance
                                .AdaptivBotConfigFilePath);
                            GlobalDataBindingValues.actualExcelPath = path;
                            Logger.OkayText = $"Excel path found & configured: {path}";
                            excelPathFound = true;
                            break;
                        }
                    }

                    if (!excelPathFound)
                    {
                        Logger.ErrorText =
                            $"Excel path not found. Limited functionality.";
                        Logger.ErrorText =
                            $"You will need to manually configure your Excel path in General Settings.";
                    }
                }
                else
                {
                    GlobalDataBindingValues.actualExcelPath = document.Root
                        .Element("GeneralSettings").Element("ExcelExecutablePath").Value;
                }
            }

            #endregion  config file & variables setup
        }


        public MainWindow()
        {
            InitializeComponent();
            
            dgExtractedFiles.ItemsSource = extractedFiles;
            NetworkChange.NetworkAddressChanged +=
                new NetworkAddressChangedEventHandler(AddressCallbackChange);

            // Deals with new windows created by browser.
            WebBrowser = (webBrowserHost.Child as System.Windows.Forms.WebBrowser);
            axBrowser = (SHDocVw.WebBrowser_V1)WebBrowser.ActiveXInstance;
            // listen for new windows
            axBrowser.NewWindow += axBrowser_NewWindow;
            WebBrowser.DocumentCompleted += webBrowser_DocumentCompleted;

            Logger = new Logger(rtbLogger);
        }


        public bool IsUsingEthernet(Logger logger, bool windowJustLoaded = false)
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up
                    && ni.NetworkInterfaceType.ToString()
                        .IndexOf("Ethernet", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (!windowJustLoaded)
                    {
                        Dispatcher.BeginInvoke((Action) (() => { logger.OkayText = "Ethernet connection restored."; }));
                    }
                    return true;
                }
            }

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        logger.WarningText = "Connected to WIFI.";
                        logger.WarningText = "For stability use Ethernet instead.";
                    }));
                    return false;
                }
            }

            Dispatcher.BeginInvoke((Action)(() =>
            {
                logger.ErrorText = "Network not detected.";
            }));
            return false;
        }


        //TODO: Check if we're on a network not just if it's wifi or ethernet.
        void AddressCallbackChange(object sender, EventArgs e)
        {
            if (IsUsingEthernet(this.Logger))
            {
                Dispatcher.BeginInvoke((Action) (() =>
                {
                    IconNetworkType.Kind = PackIconKind.Network;
                }));
            }
            else
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    IconNetworkType.Kind = PackIconKind.Wifi;
                    BtnNetworkType.Background = Brushes.Orange;
                    BtnNetworkType.BorderBrush = Brushes.Orange;
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
            WebBrowser.Navigate(URL);
            WebBrowser.DocumentCompleted += webBrowser_DocumentCompleted;
        }


        public async void OpenAdaptivAndLogin(
            string username,
            string password,
            string currentAdaptivEnvironment)
        {
            WebBrowser.Navigate(
                AdaptivEnvironmentUrls[
                    currentAdaptivEnvironment]);
            InjectedScripts.Clear();

            await Task.Run(()
                => CredentialStore.Instance.EnterAdaptivCredentials(username, password));
        }


        /// <summary>
        /// Attempts to check if Internet Explorer page has finished loading.
        /// Unfortunately it's not perfect.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowser_DocumentCompleted(
            object sender, 
            WebBrowserDocumentCompletedEventArgs e)
        {
            if (this.WebBrowser.IsBusy
                || this.WebBrowser.ReadyState != WebBrowserReadyState.Complete)
                return;
            else
            {
                completedLoading = true;
            }
        }
        

        /// <summary>
        /// Injects JavaScript into the Header of the Adaptiv page.
        /// </summary>
        /// <param name="scriptName">JavaScript function name.</param>
        /// <param name="script">JavaScript function string.</param>
        public void InjectJavascript(string scriptName, string script)
        {
            if (!InjectedScripts.ContainsKey(scriptName))
            {
                var doc = (HtmlDocument) WebBrowser.Document;
                var headElement = doc?.GetElementsByTagName("head")[0];
                var scriptElement = doc?.CreateElement("script");
                var element = (IHTMLScriptElement) scriptElement?.DomElement;
                if (!(element is null))
                {
                    element.text = script;
                    headElement.AppendChild(scriptElement);
                    InjectedScripts.Add(scriptName, script);
                }
            }
        }


        public static async void ConvertWorkbookFormats(
            string csvFile,
            string extTo)
        {
            while (!File.Exists(csvFile))
            {
                await Task.Run(() => Thread.Sleep(1000)).ConfigureAwait(false);
            }

            var excelConverterPath = Path.Combine(Path.GetDirectoryName(GlobalDataBindingValues.actualExcelPath), "excelcnv.exe");
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

        private void CmbBxAdaptivEnvironments_OnSelectionChanged(
            object sender, 
            SelectionChangedEventArgs e)
        {
            CredentialStore.Instance.Target = CmbBxAdaptivEnvironments.SelectedValue.ToString();
            TxtPasswordBox.Password = CredentialStore.Instance.Password;
        }


        private void BtnEmailBug_OnClick(object sender, RoutedEventArgs e)
        {
            FrameFunctions.SelectedIndex = 5;
            FrmEmailBugSuggestion.Visibility = Visibility.Visible;
        }
    }
}
 