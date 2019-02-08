using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using ListBox = System.Windows.Controls.ListBox;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Documents;
using AdaptivBot.Annotations;
using WebBrowser = System.Windows.Controls.WebBrowser;


namespace AdaptivBot.SettingForms
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class RiskViewSettings : Page
    {
        public RiskViewSettings()
        {
            InitializeComponent();
        }
        

        private async void btnRunExtraction_Click(object sender, RoutedEventArgs e)
        {
            GlobalConfigValues.Instance.extractionStartTime = DateTime.Now;

            var window = (MainWindow) App.Current.MainWindow;

            window?.logger.NewExtraction("Risk View Reports Extraction Started");

            if (!window.StoreUserCredentials())
            {
                return;
            }


            // TODO: Use binding here.
            var username = window.txtUserName.Text;
            var password = window.txtPasswordBox.Password;

            var selectedInstruments = new List<string>();

            foreach (var selectedItem in lstBxInstruments.SelectedItems)
            {
                selectedInstruments.Add(InstrumentLists.InstrumentGuiMapping[
                    selectedItem.ToString()
                        .Replace("System.Windows.Controls.ListBoxItem: ", "")]);
            }

            var instrumentsToLoopOver = (selectedInstruments.Count != 0)
                ? selectedInstruments
                : InstrumentLists.instruments.Keys.ToList();

            var currentAdaptivEnvironment = window.cmbBxAdaptivEnvironments.SelectedValue.ToString();
            var numberOfFailedExtractions = 0;
            var numberOfSuccessfulExtractions = 0;
            foreach (var instrumentBatch in instrumentsToLoopOver)
            {
                for (var errorCount = 0; errorCount < 3; errorCount++)
                {
                    try
                    {
                        window.logger.NewProcess($"{instrumentBatch} risk view extraction started...");
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
                        window.webBrowser.Document?.InvokeScript(nameof(JsScripts.OpenRiskView));

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

                        window.logger.OkayText = $"Filtering for {instrumentBatch}...";
                        window.InjectJavascript(
                            nameof(JsScripts.FilterRiskViewOnInstruments),
                            JsScripts.FilterRiskViewOnInstruments);
                        window.webBrowser.Document.InvokeScript(
                            nameof(JsScripts.FilterRiskViewOnInstruments),
                            new object[] { InstrumentLists.instruments[instrumentBatch] });


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
                        window.webBrowser.Document.InvokeScript(nameof(JsScripts.ExportToCsv));

                        await Task.Run(() => Thread.Sleep(500));

                        #region wait for browser

                        while (!window.completedLoading)
                        {
                            await Task.Run(() => Thread.Sleep(100));
                        }

                        await Task.Run(() => Thread.Sleep(1000));
                        window.completedLoading = false;

                        #endregion wait for browser

                        while (window.webBrowser.Document.GetElementsByTagName("A").Count == 0)
                        {
                            await Task.Run(() => Thread.Sleep(100));
                        }

                        foreach (HtmlElement link in window.webBrowser.Document
                            .GetElementsByTagName("A"))
                        {
                            if (link.InnerText.Equals("exported file link"))
                                link.InvokeMember("Click");
                        }

                        var overrideExistingFile = (bool)chkBxOverrideExistingFiles.IsChecked;
                        await Task.Run(() =>
                            window.SaveFile(instrumentBatch, overrideExistingFile));
                        numberOfSuccessfulExtractions++;
                        break;
                    }
                    catch (Exception)
                    {
                        if (errorCount < 2)
                        {
                            window.logger.ErrorText = $"Something failed for {instrumentBatch} extraction. Trying again. Attempt number: {++errorCount}";
                        }
                        else
                        {
                            numberOfFailedExtractions++;
                            window.logger.ErrorText = $"{instrumentBatch} extraction failed 3 times. Moving on to next instrument set.";
                        }
                    }
                }
            }

            window.logger.ExtractionComplete("Risk View Extraction");
            window.logger.OkayTextWithoutTime =
                $"Number of successful extractions: {numberOfSuccessfulExtractions}";
            if (numberOfFailedExtractions > 0)
            {
                window.logger.ErrorTextWithoutTime =
                    $"Number of failed extractions: {numberOfFailedExtractions}";
            }
            else
            {
                window.logger.OkayTextWithoutTime =
                    $"Number of failed extractions: {numberOfFailedExtractions}";
            }

            GlobalConfigValues.Instance.extractionEndTime = DateTime.Now;
            var timeSpan = GlobalConfigValues.Instance.extractionEndTime -
                           GlobalConfigValues.Instance.extractionStartTime;
            window.logger.OkayTextWithoutTime
                = $"Extraction took: {timeSpan.Minutes} minutes {timeSpan.Seconds % 60} seconds";
            window.webBrowser.Url = new Uri("C:\\GitLab\\AdaptivBot\\ExtractionComplete.html");
        }



        private void RiskViewSettings_OnLoaded(object sender, RoutedEventArgs e)
        {
            var xdp = (XmlDataProvider) this.Resources["RiskViewSettingsXml"];
            xdp.Source = new Uri(GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath);
        }
    }
}
