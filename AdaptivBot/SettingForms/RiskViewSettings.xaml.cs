using AutoIt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using Application = System.Windows.Application;


namespace AdaptivBot.SettingForms
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class RiskViewSettings : Page
    {
        private readonly MainWindow window = (MainWindow)Application.Current.MainWindow;


        public RiskViewSettings()
        {
            InitializeComponent();
        }
        

        private async void btnRunExtraction_Click(object sender, RoutedEventArgs e)
        {
            GlobalConfigValues.Instance.extractionStartTime = DateTime.Now;

            
            window?.Logger.NewExtraction("Risk View Reports Extraction Started");

            if (!CredentialStore.Instance.StoreUserCredentials())
            {
                return;
            }


            // TODO: Use binding here.
            var username = window.txtUserName.Text;
            var password = window.TxtPasswordBox.Password;

            var selectedInstruments = new List<string>();

            foreach (var selectedItem in lstBxInstruments.SelectedItems)
            {
                selectedInstruments.Add(InstrumentLists.InstrumentGuiNameToFolderNameMapping[
                    selectedItem.ToString()
                        .Replace("System.Windows.Controls.ListBoxItem: ", "")]);
            }

            var instrumentsToLoopOver = (selectedInstruments.Count != 0)
                ? selectedInstruments
                : InstrumentLists.InstrumentFolderNameToInstrumentBatchMapping.Keys.ToList();

            var currentAdaptivEnvironment = window.CmbBxAdaptivEnvironments.SelectedValue.ToString();
            var numberOfFailedExtractions = 0;
            var numberOfSuccessfulExtractions = 0;
            foreach (var instrumentBatch in instrumentsToLoopOver)
            {
                for (var errorCount = 0; errorCount < 3; errorCount++)
                {
                    try
                    {
                        window.Logger.NewProcess($"{instrumentBatch} risk view extraction started...");
                        await Task.Run(() =>
                            window.OpenAdaptivAndLogin(username, password,
                                currentAdaptivEnvironment));

                        #region wait for browser

                        while (!window.completedLoading)
                        {
                            await Task.Run(() => Thread.Sleep(100));
                        }

                        await Task.Run(() => Thread.Sleep(2000));
                        window.completedLoading = false;

                        #endregion wait for browser

                        #region wait for browser

                        while (!window.completedLoading)
                        {
                            await Task.Run(() => Thread.Sleep(100));
                        }

                        await Task.Run(() => Thread.Sleep(2000));
                        window.completedLoading = false;

                        #endregion wait for browser

                        window.InjectJavascript(nameof(JsScripts.OpenRiskView),
                            JsScripts.OpenRiskView);

                        await Task.Run(() => Thread.Sleep(1000));
                        window.WebBrowser.Document?.InvokeScript(nameof(JsScripts.OpenRiskView));

                        #region wait for browser

                        for (int i = 0; i < 3; i++)
                        {
                            while (!window.completedLoading)
                            {
                                await Task.Run(() => Thread.Sleep(100));
                            }

                            window.completedLoading = false;
                        }

                        await Task.Run(() => Thread.Sleep(2000));

                        #endregion wait for browser

                        window.Logger.OkayText = $"Filtering for {instrumentBatch}...";
                        window.InjectJavascript(
                            nameof(JsScripts.FilterRiskViewOnInstruments),
                            JsScripts.FilterRiskViewOnInstruments);
                        window.WebBrowser.Document.InvokeScript(
                            nameof(JsScripts.FilterRiskViewOnInstruments),
                            new object[] { InstrumentLists.InstrumentFolderNameToInstrumentBatchMapping[instrumentBatch] });


                        #region wait for browser

                        while (!window.completedLoading)
                        {
                            await Task.Run(() => Thread.Sleep(100));
                        }

                        await Task.Run(() => Thread.Sleep(1000));
                        window.completedLoading = false;

                        #endregion wait for browser


                        window.InjectJavascript(nameof(JsScripts.ExportToCsv),
                            JsScripts.ExportToCsv);
                        window.WebBrowser.Document.InvokeScript(nameof(JsScripts.ExportToCsv));

                        await Task.Run(() => Thread.Sleep(500));

                        #region wait for browser

                        while (!window.completedLoading)
                        {
                            await Task.Run(() => Thread.Sleep(100));
                        }

                        await Task.Run(() => Thread.Sleep(1000));
                        window.completedLoading = false;

                        #endregion wait for browser

                        while (window.WebBrowser.Document.GetElementsByTagName("A").Count == 0)
                        {
                            await Task.Run(() => Thread.Sleep(100));
                        }

                        foreach (HtmlElement link in window.WebBrowser.Document
                            .GetElementsByTagName("A"))
                        {
                            if (link.InnerText.Equals("exported file link"))
                                link.InvokeMember("Click");
                        }

                        var overrideExistingFile = (bool)chkBxOverrideExistingFiles.IsChecked;
                        await Task.Run(() =>
                            SaveFile(instrumentBatch, overrideExistingFile));
                        numberOfSuccessfulExtractions++;
                        break;
                    }
                    catch (Exception)
                    {
                        if (errorCount < 2)
                        {
                            window.Logger.ErrorText = $"Something failed for {instrumentBatch} extraction. Trying again. Attempt number: {++errorCount}";
                        }
                        else
                        {
                            numberOfFailedExtractions++;
                            window.Logger.ErrorText = $"{instrumentBatch} extraction failed 3 times. Moving on to next instrument set.";
                        }
                    }
                }
            }

            window.Logger.ExtractionComplete("Risk View Extraction");
            window.Logger.OkayTextWithoutTime =
                $"Number of successful extractions: {numberOfSuccessfulExtractions}";
            if (numberOfFailedExtractions > 0)
            {
                window.Logger.ErrorTextWithoutTime =
                    $"Number of failed extractions: {numberOfFailedExtractions}";
            }
            else
            {
                window.Logger.OkayTextWithoutTime =
                    $"Number of failed extractions: {numberOfFailedExtractions}";
            }

            GlobalConfigValues.Instance.extractionEndTime = DateTime.Now;
            var timeSpan = GlobalConfigValues.Instance.extractionEndTime -
                           GlobalConfigValues.Instance.extractionStartTime;
            window.Logger.OkayTextWithoutTime
                = $"Extraction took: {timeSpan.Minutes} minutes {timeSpan.Seconds % 60} seconds";
            window.WebBrowser.Url = new Uri("C:\\GitLab\\AdaptivBot\\ExtractionComplete.html");
        }


        public async void SaveFile(string instrumentBatch, bool overrideExistingFile)
        {
            AutoItX.WinWait("File Download", timeout: 20);
            AutoItX.WinActivate("File Download");
            AutoItX.Send("{TAB 3}");
            AutoItX.Send("{ENTER}");
            Dispatcher.Invoke((System.Action)(() =>
            {
                window.Logger.OkayText = $"Saving CSV file for {instrumentBatch}...";
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
                    Dispatcher.Invoke((System.Action)(() =>
                    {
                        window.Logger.WarningText =
                            $"Overriding existing file for {instrumentBatch}...";
                    }));
                }
                else
                {
                    AutoItX.Send("!n");
                    Dispatcher.Invoke((System.Action)(() =>
                    {
                        window.Logger.WarningText =
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

            if (fileSaved)
            {
                var fileSize = (new FileInfo(filePath).Length >= 1048576)
                    ? $"{(new FileInfo(filePath).Length / 1048576):n}" + " MB"
                    : $"{(new FileInfo(filePath).Length / 1024):n}" + " KB";
                Dispatcher.Invoke((System.Action)(() =>
                {
                    window.extractedFiles.Add(new ExtractedFile()
                    {
                        FilePath = filePath,
                        FileName = Path.GetFileName(filePath),
                        FileType = $"Risk View : {instrumentBatch}",
                        FileSize = fileSize
                    });
                }));
            }
            // TODO: Checkbox to close window when complete.

        }


        private void RiskViewSettings_OnLoaded(object sender, RoutedEventArgs e)
        {
            var xdp = (XmlDataProvider) this.Resources["RiskViewSettingsXml"];
            xdp.Source = new Uri(GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath);
        }
    }
}
