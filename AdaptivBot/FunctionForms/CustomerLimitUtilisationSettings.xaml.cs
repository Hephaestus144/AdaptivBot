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
using System.Windows.Media;
using Excel = Microsoft.Office.Interop.Excel;


namespace AdaptivBot.SettingForms
{
    public partial class CustomerLimitUtilisationSettings : Page
    {
        private readonly MainWindow _window = (MainWindow)Application.Current.MainWindow;

        public CustomerLimitUtilisationSettings()
        {
            InitializeComponent();
        }


        private void CustomerLimitUtilisationSettings_OnLoaded(object sender, RoutedEventArgs e)
        {
            var xdp =
                (XmlDataProvider)this.Resources["CustomerLimitUtilisationSettingsXml"];

            xdp.Source = new Uri(GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath);
        }


        private void JavaScriptErrorDialogFound()
        {
            for (var i = 0; i < 15; i++)
            {
                AutoItX.Sleep(100);
                if (AutoItX.WinExists("Script Error") != 0)
                {
                    Dispatcher.Invoke((() =>
                    {
                        _window.Logger.DontPanicErrorText = $"JavaScript error caught, restarting extraction...";
                    }));
                    AutoItX.WinActivate("Script Error");
                    AutoItX.Send("!y");
                    throw new Exception();
                }
            }
        }


        public async void btnRunExtraction_Click(object sender, RoutedEventArgs e)
        {
            GlobalDataBindingValues.Instance.extractionStartTime = DateTime.Now;
            var date = datePicker.SelectedDate;

            if (date is null)
            {
                _window.Logger.ErrorText = "Please select a date for extraction.";
                return;
            }

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
            var username = _window.TxtUserName.Text;
            var password = _window.TxtPasswordBox.Password;

            const int maxFailureCount = 5;
            for (var failureCount = 0; failureCount < maxFailureCount; failureCount++)
            {
                try
                {
                    _window.Logger.NewExtraction("Customer Limit Utilisation Report Extraction Started");

                    var currentAdaptivEnvironment
                        = _window.CmbBxAdaptivEnvironments.SelectedValue.ToString();

                    //CredentialStore.Instance.StoreUserCredentials();
                    var successfulLogin = await Task.Run(() =>
                        _window.OpenAdaptivAndLogin(username, password,
                            currentAdaptivEnvironment));

                    if (!successfulLogin)
                    {
                        _window.Logger.ErrorText = "Failed to run customer limit utilisation extraction!";
                        return;
                    }

                    #region wait for browser
                    _window.completedLoading = false;
                    while (!_window.completedLoading)
                    {
                        await Task.Run(() => Thread.Sleep(100));
                    }

                    await Task.Run(() => Thread.Sleep(1000));
                    _window.completedLoading = false;
                    #endregion wait for browser
                    
                    Action methodName = JavaScriptUtils.JavaScriptErrorDialogFound;
                    IAsyncResult result = methodName.BeginInvoke(null, null);
                    _window.InjectJavascript(
                        nameof(JsScripts.OpenCustomerLimitUtilisationReport),
                        JsScripts.OpenCustomerLimitUtilisationReport);

                    _window.InjectJavascript(
                        nameof(JsScripts.OpenCustomerLimitUtilisationReport),
                        JsScripts.OpenCustomerLimitUtilisationReport);

                    _window.WebBrowser.Document?.InvokeScript(nameof(JsScripts.OpenCustomerLimitUtilisationReport));

                    #region wait for browser

                    while (!_window.completedLoading)
                    {
                        await Task.Run(() => Thread.Sleep(100));
                    }

                    await Task.Run(() => Thread.Sleep(1000));
                    _window.completedLoading = false;
                    #endregion wait for browser

                    #region wait for browser
                    while (!_window.completedLoading)
                    {
                        await Task.Run(() => Thread.Sleep(100));
                    }

                    await Task.Run(() => Thread.Sleep(1000));
                    _window.completedLoading = false;

                    #endregion wait for browser

                    methodName = JavaScriptErrorDialogFound;
                    result = methodName.BeginInvoke(null, null);

                    // TODO: Get filter details for customer limit utilisation report.
                    var fields = new List<string>();
                    var conditions = new List<string>();
                    var criteria = new List<string>();
                    var conjunctions = new List<string>();

                    if (cmbBxFilterField1.SelectedIndex != 0)
                    {
                        fields.Add(cmbBxFilterField1.SelectedValue.ToString());
                    }

                    if (cmbBxFilterField2.SelectedIndex != 0)
                    {
                        fields.Add(cmbBxFilterField2.SelectedValue.ToString());
                    }

                    if (cmbBxFilterField3.SelectedIndex != 0)
                    {
                        fields.Add(cmbBxFilterField3.SelectedValue.ToString());
                    }

                    if (cmbBxFilterCondition1.SelectedIndex != 0)
                    {
                        conditions.Add(cmbBxFilterCondition1.SelectedValue.ToString());
                    }

                    if (cmbBxFilterCondition2.SelectedIndex != 0)
                    {
                        conditions.Add(cmbBxFilterCondition2.SelectedValue.ToString());
                    }

                    if (cmbBxFilterCondition3.SelectedIndex != 0)
                    {
                        conditions.Add(cmbBxFilterCondition3.SelectedValue.ToString());
                    }

                    
                    _window.Logger.OkayText = "Filtering customer limit utilisation report...";
                    _window.InjectJavascript(
                        nameof(JsScripts.FilterCustomerLimitUtilisationReportForPortfolioAnalysis),
                        JsScripts.FilterCustomerLimitUtilisationReportForPortfolioAnalysis);

                    //_window.InjectJavascript(
                    //    nameof(JsScripts.FilterCustomerLimitUtilisationReport),
                    //    JsScripts.FilterCustomerLimitUtilisationReport(fields, conditions, criteria, conjunctions));

                    _window.WebBrowser.Document?.InvokeScript(nameof(JsScripts.FilterCustomerLimitUtilisationReportForPortfolioAnalysis));

                    #region wait for browser

                    //_window.completedLoading = false;

                    //while (!_window.completedLoading)
                    //{
                    //    await Task.Run(() => Thread.Sleep(100));
                    //}

                    //await Task.Run(() => Thread.Sleep(1000));
                    //_window.completedLoading = false;

                    #endregion wait for browser

                    methodName.EndInvoke(result);

                    methodName = JavaScriptErrorDialogFound;
                    result = methodName.BeginInvoke(null, null);
                    _window.Logger.OkayText =
                        $"Opening customer limit utilisation report for {(DateTime)date:dd-MMM-yyyy}...";

                    _window.InjectJavascript(
                        nameof(JsScripts.ChooseCustomerLimitUtilisationReport),
                        JsScripts.ChooseCustomerLimitUtilisationReport);

                    await Task.Run(() => Thread.Sleep(1000));

                    _window.WebBrowser.Document?.InvokeScript(
                        nameof(JsScripts.ChooseCustomerLimitUtilisationReport));

                    _window.InjectedScripts.Clear();

                    #region wait for browser

                    _window.completedLoading = false;

                    while (!_window.completedLoading)
                    {
                        await Task.Run(() => Thread.Sleep(100));
                    }

                    await Task.Run(() => Thread.Sleep(1000));
                    _window.completedLoading = false;

                    #endregion wait for browser

                    methodName.EndInvoke(result);

                    await Task.Run(() => Thread.Sleep(1000));
                    methodName = JavaScriptErrorDialogFound;
                    result = methodName.BeginInvoke(null, null);

                    _window.InjectJavascript(
                        nameof(JsScripts.SelectCustomerLimitUtilisationReportDate),
                        JsScripts.SelectCustomerLimitUtilisationReportDate);

                    _window.WebBrowser.Document?.InvokeScript(
                        nameof(JsScripts.SelectCustomerLimitUtilisationReportDate),
                        new object[] { ((DateTime)date).ToString("dd/MM/yyyy") });

                    #region wait for browser

                    _window.completedLoading = false;
                    while (!_window.completedLoading)
                    {
                        await Task.Run(() => Thread.Sleep(100));
                    }

                    await Task.Run(() => Thread.Sleep(1000));
                    _window.completedLoading = false;

                    #endregion wait for browser

                    methodName.EndInvoke(result);

                    methodName = JavaScriptErrorDialogFound;
                    result = methodName.BeginInvoke(null, null);

                    _window.InjectJavascript(
                        nameof(JsScripts.GenerateCustomerLimitUtilisationReport),
                        JsScripts.GenerateCustomerLimitUtilisationReport);

                    _window.WebBrowser.Document?.InvokeScript(
                        nameof(JsScripts.GenerateCustomerLimitUtilisationReport));

                    methodName.EndInvoke(result);

                    while (_window.WebBrowser.Document?.GetElementsByTagName("img").Count < 5)
                    {
                        await Task.Run(() => Thread.Sleep(1000));
                    }

                    await Task.Run(() => Thread.Sleep(3000));

                    methodName = JavaScriptErrorDialogFound;
                    result = methodName.BeginInvoke(null, null);
                    _window.InjectJavascript(
                        nameof(JsScripts.ExportCustomerLimitUtilisationReportToCsv),
                        JsScripts.ExportCustomerLimitUtilisationReportToCsv);

                    _window.WebBrowser.Document?.InvokeScript(
                        nameof(JsScripts.ExportCustomerLimitUtilisationReportToCsv));

                    var overrideExistingFile = (bool)chkBxOverrideExistingFiles.IsChecked;

                    await Task.Run(() => Thread.Sleep(1000));
                    methodName.EndInvoke(result);

                    await Task.Run(() =>
                        SaveCustomerLimitUtilisationReport((DateTime)date,
                            overrideExistingFile));

                    var csvFile = $"\\\\pcibtighnas1\\CBSData\\Portfolio Analysis\\Data\\Cust Util\\SBG\\CustomerLimitUtil {date:dd.MM.yyyy}.csv";
                    Dispatcher.Invoke((System.Action)(() =>
                    {
                        _window.Logger.OkayText =
                            "Converting csv extraction to xlsx...";
                    }));

                    var xlsxFile = csvFile.Replace(".csv", ".xlsx");

                    if (overrideExistingFile && File.Exists(xlsxFile))
                    {
                        File.Delete(xlsxFile);
                        Thread.Sleep(1000);
                    }

                    MainWindow.ConvertWorkbookFormats(csvFile, ".xlsx");

                    while (!File.Exists(xlsxFile))
                    {
                        await Task.Run(() => Thread.Sleep(100));
                    }

                    var fileSize = FileUtils.FileSize(xlsxFile);

                    Dispatcher.Invoke((System.Action)(() =>
                    {
                        _window.Logger.OkayText =
                            "Performing minor formatting on xlsx file...";
                    }));

                    var xlApp = new Excel.Application();
                    Excel.Workbook wb = xlApp.Workbooks.Open(xlsxFile);
                    Excel.Worksheet ws = wb.Worksheets[1];
                    xlApp.DisplayAlerts = false;
                    ws.Name = "Customer Limit Utilisation";
                    Excel.Range topLeftCell = ws.Cells[1, 1];
                    Excel.Range bottomRightCell = ws.Cells[3, 1000];
                    Excel.Range rangeToDelete = ws.Range[topLeftCell, bottomRightCell];
                    rangeToDelete.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                    wb.Save();
                    xlApp.DisplayAlerts = true;
                    wb.Close();
                    xlApp.Quit();

                    Dispatcher.Invoke((System.Action)(() =>
                    {
                        _window.Logger.OkayText = "Deleting csv file...";
                    }));
                    File.Delete(csvFile);


                    Dispatcher.Invoke((System.Action)(() =>
                    {
                        if (_window.extractedFiles.Any(x => x.FilePath == xlsxFile))
                        {
                            _window.extractedFiles.Remove(_window.extractedFiles.First(x => x.FilePath == xlsxFile));
                        }

                        _window.extractedFiles.Add(new ExtractedFile
                        {
                            FilePath = xlsxFile,
                            FileName = Path.GetFileName(xlsxFile),
                            FileType = "Customer Limit Utilisation",
                            FileSize = fileSize
                        });
                    }));

                    _window.Logger.ExtractionComplete("Customer Limit Utilisation");
                    GlobalDataBindingValues.Instance.extractionEndTime = DateTime.Now;
                    var timeSpan = GlobalDataBindingValues.Instance.extractionEndTime
                                   - GlobalDataBindingValues.Instance.extractionStartTime;

                    _window.Logger.OkayTextWithoutTime =
                        $"Extraction took: {timeSpan.Minutes} minutes {timeSpan.Seconds % 60} seconds";

                    _window.WebBrowser.Url = new Uri("C:\\GitLab\\AdaptivBot\\ExtractionComplete.html");
                    break;
                }
                catch (Exception exception)
                {
                    if (failureCount < maxFailureCount)
                    {
                        _window.Logger.ErrorText =
                            $"Something failed for Customer Limit Utilisation extraction. {exception.Message}";
                        _window.Logger.ErrorText = $"Trying again. Attempt number: {failureCount + 2}";
                    }
                    else
                    {
                        _window.Logger.ErrorText = $"Customer Limit Utilisation " +
                                                   $"extraction extraction failed {maxFailureCount} times. " +
                                                   $"This may be due to an Adaptiv error. Please try again later.";
                    }
                }
            }
        }


        public async void SaveCustomerLimitUtilisationReport(
            DateTime date,
            bool overrideExistingFile)
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
            AutoItX.Send(
                $"\\\\pcibtighnas1\\CBSData\\Portfolio Analysis\\Data\\Cust Util\\SBG");
            AutoItX.Send("!s");
            Dispatcher.Invoke((Action) (() =>
            {
                _window.Logger.OkayText = $"Saving csv file...";
            }));

            await Task.Run(() => Thread.Sleep(1000));
            if (AutoItX.WinExists("Confirm Save As") != 0)
            {
                AutoItX.WinActivate("Confirm Save As");
                if (overrideExistingFile)
                {
                    AutoItX.Send("!y");
                    Dispatcher.Invoke((Action) (() =>
                    {
                        _window.Logger.WarningText = $"Overriding existing file...";
                    }));
                }
                else
                {
                    AutoItX.Send("!n");
                    Dispatcher.Invoke((Action) (() =>
                    {
                        _window.Logger.WarningText = $"File already exists.";
                    }));
                    AutoItX.WinWait("Save As", timeout: 20);
                    AutoItX.WinActivate("Save As");
                    AutoItX.Send("{ENTER}");
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
                    await Task.Run(() => Thread.Sleep(100));
                }
            }
        }


        #region filtering

        private void CmbBxFilterCategory1_OnSelectionChanged(object sender,
            SelectionChangedEventArgs e)
        {
            if (cmbBxFilterField2 == null) return;
            if (((ComboBoxItem) cmbBxFilterField1.SelectedItem).Tag.ToString() == "Hide")
            {
                cardFilterField2.Visibility = Visibility.Hidden;
                cmbBxFilterField2.Visibility = Visibility.Hidden;
                cmbBxFilterField2.SelectedIndex = 0;
                cardFilterCondition2.Visibility = Visibility.Hidden;
                cmbBxFilterCondition2.Visibility = Visibility.Hidden;
                cmbBxFilterCondition2.SelectedIndex = 0;
                cardFilterCriteria2.Visibility = Visibility.Hidden;
                txtBxFilterCriteria2.Visibility = Visibility.Hidden;
                txtBxFilterCriteria2.Text = "";
            }
            else
            {
                cardFilterField2.Visibility = Visibility.Visible;
                cmbBxFilterField2.Visibility = Visibility.Visible;
                cardFilterCondition2.Visibility = Visibility.Visible;
                cmbBxFilterCondition2.Visibility = Visibility.Visible;
                cardFilterCriteria2.Visibility = Visibility.Visible;
                txtBxFilterCriteria2.Visibility = Visibility.Visible;
            }
        }


        private void CmbBxFilterCategory2_OnSelectionChanged(object sender,
            SelectionChangedEventArgs e)
        {
            if (cmbBxFilterField3 == null) return;
            if (((ComboBoxItem) cmbBxFilterField2.SelectedItem).Tag.ToString() == "Hide")
            {
                cardFilterField3.Visibility = Visibility.Hidden;
                cmbBxFilterField3.Visibility = Visibility.Hidden;
                cmbBxFilterField3.SelectedIndex = 0;
                cardFilterCondition3.Visibility = Visibility.Hidden;
                cmbBxFilterCondition3.Visibility = Visibility.Hidden;
                cmbBxFilterCondition3.SelectedIndex = 0;
                cardFilterCriteria3.Visibility = Visibility.Hidden;
                txtBxFilterCriteria3.Visibility = Visibility.Hidden;
                txtBxFilterCriteria3.Text = "";
            }
            else
            {
                cardFilterField3.Visibility = Visibility.Visible;
                cmbBxFilterField3.Visibility = Visibility.Visible;
                cardFilterCondition3.Visibility = Visibility.Visible;
                cmbBxFilterCondition3.Visibility = Visibility.Visible;
                cardFilterCriteria3.Visibility = Visibility.Visible;
                txtBxFilterCriteria3.Visibility = Visibility.Visible;
            }

            if (cmbBxFilterField1.SelectedValue == cmbBxFilterField2.SelectedValue)
            {
                cmbBxFilterField1.Foreground = Brushes.Red;
                cmbBxFilterField2.Foreground = Brushes.Red;
            }

            if (cmbBxFilterField1.SelectedValue == cmbBxFilterField3.SelectedValue)
            {
                cmbBxFilterField1.Foreground = Brushes.Red;
                cmbBxFilterField3.Foreground = Brushes.Red;
            }

            if (cmbBxFilterField2.SelectedValue == cmbBxFilterField3.SelectedValue)
            {
                cmbBxFilterField2.Foreground = Brushes.Red;
                cmbBxFilterField3.Foreground = Brushes.Red;
            }

            _window.Logger.ErrorText =
                "At least two of your filtering categories are the same.";
        }

        #endregion filtering


        private void BtnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}