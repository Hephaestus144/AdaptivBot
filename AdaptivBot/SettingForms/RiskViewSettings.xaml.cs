using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AdaptivBot;

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

            window.logger.NewExtraction("Risk View Reports Extraction Started");

            if (!window.StoreUserCredentials())
            {
                return;
            }

            // Wrap this in a function called login to Adaptiv or which checks if Adaptiv
            // has already been  logged into.

            // TODO: Use binding here.
            var username = window.txtUserName.Text;
            var password = window.txtPasswordBox.Password;


            var currentAdaptivEnvironment = window.cmbBxAdaptivEnvironments.SelectedValue.ToString();
            int numberOfFailedExtractions = 0;
            int numberOfSuccessfulExtractions = 0;
            foreach (var instrumentBatch in InstrumentLists.instruments.Keys)
            {
                for (var errorCount = 0; errorCount < 3; errorCount++)
                {
                    try
                    {
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
                        window.webBrowser.Document.InvokeScript(nameof(JsScripts.OpenRiskView));

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

            window.logger.OkayText = "Completed";
            window.logger.OkayText =
                $"Number of successful extractions: {numberOfSuccessfulExtractions}";
            window.logger.ErrorText =
                $"Number of failed extractions: {numberOfFailedExtractions}";
            GlobalConfigValues.Instance.extractionEndTime = DateTime.Now;
            window.logger.OkayText =
                $"Extraction took: {(GlobalConfigValues.Instance.extractionEndTime - GlobalConfigValues.Instance.extractionStartTime).Minutes} minutes";
        }

    }
}
