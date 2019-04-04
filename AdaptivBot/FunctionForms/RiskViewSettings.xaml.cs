using AutoIt;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Xml.Linq;
using Application = System.Windows.Application;
using TextBox = System.Windows.Controls.TextBox;


namespace AdaptivBot.SettingForms
{
    /// <summary>
    /// Interaction logic for RiskViewSettings.xaml
    /// </summary>
    public partial class RiskViewSettings : Page
    {
        private readonly MainWindow _window = (MainWindow)Application.Current.MainWindow;


        public RiskViewSettings()
        {
            InitializeComponent();
        }


        private void JavaScriptErrorDialogFound()
        {
            for (var i = 0; i < 15; i++)
            {
                AutoItX.Sleep(100);
                if (AutoItX.WinExists("Script Error") != 0)
                {
                    Dispatcher.Invoke((Action) (() =>
                        _window.Logger.DontPanicErrorText =
                            $"JavaScript error caught, restarting extraction..."));
                    AutoItX.WinActivate("Script Error");
                    AutoItX.Send("!y");
                    throw new Exception();
                }
            }
        }


        private async void BtnRunExtraction_Click(object sender, RoutedEventArgs e)
        {
            GlobalDataBindingValues.Instance.extractionStartTime = DateTime.Now;

            _window?.Logger.NewExtraction("Risk View Reports Extraction Started");

            if (!CredentialStore.Instance.StoreUserCredentials())
            {
                return;
            }

            if (CredentialStore.Instance.CancelRun)
            {
                _window.Logger.WarningText = "Run cancelled by user!";
                return;
            }

            // TODO: Use binding here.
            var username = _window?.TxtUserName.Text;
            var password = _window?.TxtPasswordBox.Password;

            var selectedInstruments = (from object selectedItem in lstBxInstruments.SelectedItems
                select InstrumentLists.InstrumentGuiNameToFolderNameMapping[selectedItem.ToString()
                    .Replace("System.Windows.Controls.ListBoxItem: ", "")]).ToList();

            var instrumentsToLoopOver = selectedInstruments.Count != 0
                ? selectedInstruments
                : InstrumentLists.InstrumentFolderNameToInstrumentBatchMapping.Keys.ToList();

            var currentAdaptivEnvironment = _window?.CmbBxAdaptivEnvironments.SelectedValue.ToString();
            var numberOfFailedExtractions = 0;
            var numberOfSuccessfulExtractions = 0;
            const int maxFailureCount = 5;
            foreach (var instrumentBatch in instrumentsToLoopOver)
            {
                for (var failureCount = 0; failureCount < maxFailureCount; failureCount++)
                {
                    try
                    {
                        _window.Logger.NewProcess($"{instrumentBatch} risk view extraction started...");
                        var successfulLogin = await Task.Run(() =>
                            _window.OpenAdaptivAndLogin(username, password,
                                currentAdaptivEnvironment));

                        if (!successfulLogin)
                        {
                            _window.Logger.ErrorText = "Failed to run risk view extraction!";
                            return;
                        }

                        #region wait for browser

                        while (!_window.completedLoading)
                        {
                            await Task.Run(() => Thread.Sleep(100));
                        }

                        await Task.Run(() => Thread.Sleep(2000));
                        _window.completedLoading = false;

                        #endregion wait for browser

                        #region wait for browser

                        while (!_window.completedLoading)
                        {
                            await Task.Run(() => Thread.Sleep(100));
                        }

                        await Task.Run(() => Thread.Sleep(2000));
                        _window.completedLoading = false;

                        #endregion wait for browser

                        _window.InjectJavascript(nameof(JsScripts.OpenRiskView),
                            JsScripts.OpenRiskView);
                        _window.WebBrowser.Document.InvokeScript(nameof(JsScripts.OpenRiskView));

                        #region wait for browser

                        while (!_window.completedLoading)
                        {
                            await Task.Run(() => Thread.Sleep(100));
                        }

                        await Task.Run(() => Thread.Sleep(2000));
                        _window.completedLoading = false;

                        #endregion wait for browser

                        Action methodName = JavaScriptErrorDialogFound;

                        IAsyncResult result = methodName.BeginInvoke(null, null);
                        _window.Logger.OkayText = $"Filtering for {instrumentBatch}...";
                        _window.InjectJavascript(
                            nameof(JsScripts.FilterRiskViewOnInstruments),
                            JsScripts.FilterRiskViewOnInstruments);
                        _window.WebBrowser.Document.InvokeScript(
                            nameof(JsScripts.FilterRiskViewOnInstruments),
                            new object[] { InstrumentLists.InstrumentFolderNameToInstrumentBatchMapping[instrumentBatch] });
                        

                        #region wait for browser

                        while (!_window.completedLoading)
                        {
                            await Task.Run(() => Thread.Sleep(100));
                        }

                        await Task.Run(() => Thread.Sleep(1000));
                        _window.completedLoading = false;

                        #endregion wait for browser

                        methodName.EndInvoke(result);
                        _window.InjectJavascript(nameof(JsScripts.ExportToCsv),
                            JsScripts.ExportToCsv);
                        _window.WebBrowser.Document.InvokeScript(nameof(JsScripts.ExportToCsv));
                        

                        #region wait for browser

                        while (!_window.completedLoading)
                        {
                            await Task.Run(() => Thread.Sleep(100));
                        }

                        await Task.Run(() => Thread.Sleep(1000));
                        _window.completedLoading = false;

                        #endregion wait for browser


                        while (_window.WebBrowser.Document.GetElementsByTagName("A").Count == 0)
                        {
                            await Task.Run(() => Thread.Sleep(100));
                        }

                        foreach (HtmlElement link in _window.WebBrowser.Document
                            .GetElementsByTagName("A"))
                        {
                            if (link.InnerText.Equals("exported file link"))
                                link.InvokeMember("Click");
                        }
                        
                        await Task.Run(() => Thread.Sleep(1000));
                        var overrideExistingFile = (bool)chkBxOverrideExistingFiles.IsChecked;                  
                        await Task.Run(() => SaveFile(instrumentBatch, overrideExistingFile).Wait());
                        numberOfSuccessfulExtractions++;
                        break;
                    }
                    catch (Exception exception)
                    {
                        if (failureCount < maxFailureCount)
                        {
                            _window.Logger.ErrorText = $"Something failed for {instrumentBatch} extraction. Trying again. Attempt number: {failureCount}";
                        }
                        else
                        {
                            numberOfFailedExtractions++;
                            _window.Logger.ErrorText =
                                $"{instrumentBatch} extraction failed {maxFailureCount} times. Moving on to next instrument set.";
                        }
                    }
                }
            }

            _window.Logger.ExtractionComplete("Risk View Extraction");
            _window.Logger.OkayTextWithoutTime =
                $"Number of successful extractions: {numberOfSuccessfulExtractions}";
            if (numberOfFailedExtractions > 0)
            {
                _window.Logger.ErrorTextWithoutTime =
                    $"Number of failed extractions: {numberOfFailedExtractions}";
            }
            else
            {
                _window.Logger.OkayTextWithoutTime =
                    $"Number of failed extractions: {numberOfFailedExtractions}";
            }

            GlobalDataBindingValues.Instance.extractionEndTime = DateTime.Now;
            var timeSpan = GlobalDataBindingValues.Instance.extractionEndTime
                           - GlobalDataBindingValues.Instance.extractionStartTime;
            _window.Logger.OkayTextWithoutTime
                = $"Extraction took: {timeSpan.Minutes} minutes {timeSpan.Seconds % 60} seconds";
            //_window.WebBrowser.Url = new Uri("C:\\GitLab\\AdaptivBot\\ExtractionComplete.html");
            _window.WebBrowser.Url =
                new Uri(GlobalDataBindingValues.Instance.ExtractionCompleteWithoutErrors);
        }


        public async Task SaveFile(string instrumentBatch, bool overrideExistingFile)
        {
            AutoItX.WinWait("File Download", timeout: 20);
            AutoItX.WinActivate("File Download");
            AutoItX.Send("!s");
            Dispatcher.Invoke((Action) (() =>
                _window.Logger.OkayText = $"Saving CSV file for {instrumentBatch}..."));

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
            await Task.Run(() => Thread.Sleep(1000));
            var fileSaved = true;

            if (AutoItX.WinExists("Confirm Save As") != 0)
            {
                AutoItX.WinActivate("Confirm Save As");
                if (overrideExistingFile)
                {
                    AutoItX.Send("!y");
                    Dispatcher.Invoke((System.Action)(() =>
                    {
                        _window.Logger.WarningText =
                            $"Overriding existing file for {instrumentBatch}...";
                    }));
                }
                else
                {
                    AutoItX.Send("!n");
                    Dispatcher.Invoke((System.Action)(() =>
                    {
                        _window.Logger.WarningText =
                            $"File already exists for {instrumentBatch}.";
                    }));
                    AutoItX.WinWait("Save As", timeout: 20);
                    AutoItX.WinActivate("Save As");
                    AutoItX.Send("{TAB 10}");
                    AutoItX.Send("{ENTER}");
                    fileSaved = false;
                }
            }

            await Task.Run(() => Thread.Sleep(1000));

            if (AutoItX.WinExists("", "Close this dialog box when download completes") != 0)
            {
                AutoItX.WinActivate("", "Close this dialog box when download completes");
                AutoItX.Send("{Tab}");
                AutoItX.Send("+");

                while (AutoItX.WinExists("", "Close this dialog box when download completes") != 0)
                {
                    AutoItX.Sleep(100);
                }
            }

            var filePath =
                $"\\\\pcibtighnas1\\CBSData\\Portfolio Analysis\\Data\\{instrumentBatch}\\SBG\\STBUKTCPROD (Standard Bank Group) (Filtered){DateTime.Now:dd-MM-yyyy}.csv";

            while (!File.Exists(filePath))
            {
                await Task.Run(() => Thread.Sleep(500));
            }

            if (fileSaved)
            {
                var fileSize = (new FileInfo(filePath).Length >= 1048576)
                    ? $"{new FileInfo(filePath).Length / 1048576:n}" + " MB"
                    : $"{new FileInfo(filePath).Length / 1024:n}" + " KB";

                Dispatcher.Invoke(() =>
                {
                    if (_window.extractedFiles.Any(x => x.FilePath == filePath))
                    {
                        _window.extractedFiles.Remove(_window.extractedFiles.First(x => x.FilePath == filePath));
                    }

                    _window.extractedFiles.Add(new ExtractedFile()
                    {
                        FilePath = filePath,
                        FileName = Path.GetFileName(filePath),
                        FileType = $"Risk View : {instrumentBatch}",
                        FileSize = fileSize
                    });
                });
            }
        }


        private void RiskViewSettings_OnLoaded(object sender, RoutedEventArgs e)
        {
            var xdp = (XmlDataProvider) this.Resources["RiskViewSettingsXml"];
            xdp.Source = new Uri(GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath);
        }


        private void BtnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            txtBxBaseFolder.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtBxFileNameFormat.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            var xdp = (XmlDataProvider)this.Resources["RiskViewSettingsXml"];
            xdp.Document.Save(GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath);
            _window.Logger.OkayText = "Risk view settings saved.";
        }
    }
}
