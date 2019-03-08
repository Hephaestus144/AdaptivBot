using AutoIt;
using System;
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
                    Dispatcher.Invoke((System.Action)(() =>
                    {
                        _window.Logger.ErrorText = $"JavaScript error caught, restarting extraction...";
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

            const int maxFailureCount = 5;
            for (var failureCount = 0; failureCount < maxFailureCount; failureCount++)
            {
                try
                {
                    _window.Logger.NewExtraction("Customer Limit Utilisation Report Extraction Started");

                    // TODO: Use binding here.
                    var username = _window.TxtUserName.Text;
                    var password = _window.TxtPasswordBox.Password;

                    var currentAdaptivEnvironment
                        = _window.CmbBxAdaptivEnvironments.SelectedValue.ToString();

                    CredentialStore.Instance.StoreUserCredentials();
                    await Task.Run(() => _window.OpenAdaptivAndLogin(username, password, currentAdaptivEnvironment));

                    #region wait for browser
                    _window.completedLoading = false;
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
                    _window.Logger.OkayText = "Filtering customer limit utilisation report...";
                    _window.InjectJavascript(
                        nameof(JsScripts.FilterCustomerLimitUtilisationReport),
                        JsScripts.FilterCustomerLimitUtilisationReport);

                    _window.WebBrowser.Document?.InvokeScript(nameof(JsScripts.FilterCustomerLimitUtilisationReport));

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

                    var saveCustUtilReport =
                        new Action<DateTime, bool>(SaveCustomerLimitUtilisationReport);

                    await Task.Run(() =>
                        SaveCustomerLimitUtilisationReport((DateTime)date,
                            overrideExistingFile));

                    //IAsyncResult saveCustUtilReportResult =
                    //    saveCustUtilReport.BeginInvoke((DateTime) date,
                    //        overrideExistingFile, null, null);
                    



                    //saveCustUtilReport.EndInvoke(saveCustUtilReportResult);

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

                    var fileSize
                        = (new FileInfo(xlsxFile).Length >= 1048576)
                        ? $"{new FileInfo(xlsxFile).Length / 1048576:n}" + " MB"
                        : $"{new FileInfo(xlsxFile).Length / 1024:n}"    + " KB";


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
            if (cmbBxFilterCategory2 == null) return;
            if (((ComboBoxItem) cmbBxFilterCategory1.SelectedItem).Tag.ToString() == "Hide")
            {
                cardFilterCategory2.Visibility = Visibility.Hidden;
                cmbBxFilterCategory2.Visibility = Visibility.Hidden;
                cmbBxFilterCategory2.SelectedIndex = 0;
                cardFilterOperation2.Visibility = Visibility.Hidden;
                cmbBxFilterOperation2.Visibility = Visibility.Hidden;
                cmbBxFilterOperation2.SelectedIndex = 0;
                cardFilterCriteria2.Visibility = Visibility.Hidden;
                txtBxFilterCriteria2.Visibility = Visibility.Hidden;
                txtBxFilterCriteria2.Text = "";
            }
            else
            {
                cardFilterCategory2.Visibility = Visibility.Visible;
                cmbBxFilterCategory2.Visibility = Visibility.Visible;
                cardFilterOperation2.Visibility = Visibility.Visible;
                cmbBxFilterOperation2.Visibility = Visibility.Visible;
                cardFilterCriteria2.Visibility = Visibility.Visible;
                txtBxFilterCriteria2.Visibility = Visibility.Visible;
            }
        }


        private void CmbBxFilterCategory2_OnSelectionChanged(object sender,
            SelectionChangedEventArgs e)
        {
            if (cmbBxFilterCategory3 == null) return;
            if (((ComboBoxItem) cmbBxFilterCategory2.SelectedItem).Tag.ToString() == "Hide")
            {
                cardFilterCategory3.Visibility = Visibility.Hidden;
                cmbBxFilterCategory3.Visibility = Visibility.Hidden;
                cmbBxFilterCategory3.SelectedIndex = 0;
                cardFilterOperation3.Visibility = Visibility.Hidden;
                cmbBxFilterOperation3.Visibility = Visibility.Hidden;
                cmbBxFilterOperation3.SelectedIndex = 0;
                cardFilterCriteria3.Visibility = Visibility.Hidden;
                txtBxFilterCriteria3.Visibility = Visibility.Hidden;
                txtBxFilterCriteria3.Text = "";
            }
            else
            {
                cardFilterCategory3.Visibility = Visibility.Visible;
                cmbBxFilterCategory3.Visibility = Visibility.Visible;
                cardFilterOperation3.Visibility = Visibility.Visible;
                cmbBxFilterOperation3.Visibility = Visibility.Visible;
                cardFilterCriteria3.Visibility = Visibility.Visible;
                txtBxFilterCriteria3.Visibility = Visibility.Visible;
            }

            if (cmbBxFilterCategory1.SelectedValue == cmbBxFilterCategory2.SelectedValue)
            {
                cmbBxFilterCategory1.Foreground = Brushes.Red;
                cmbBxFilterCategory2.Foreground = Brushes.Red;
            }

            if (cmbBxFilterCategory1.SelectedValue == cmbBxFilterCategory3.SelectedValue)
            {
                cmbBxFilterCategory1.Foreground = Brushes.Red;
                cmbBxFilterCategory3.Foreground = Brushes.Red;
            }

            if (cmbBxFilterCategory2.SelectedValue == cmbBxFilterCategory3.SelectedValue)
            {
                cmbBxFilterCategory2.Foreground = Brushes.Red;
                cmbBxFilterCategory3.Foreground = Brushes.Red;
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