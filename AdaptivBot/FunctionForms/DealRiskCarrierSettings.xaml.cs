using AutoIt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Application = System.Windows.Application;
using Excel = Microsoft.Office.Interop.Excel;


namespace AdaptivBot.SettingForms
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class DealRiskCarrierSettings : Page
    {
        private readonly MainWindow _window = (MainWindow)Application.Current.MainWindow;


        public DealRiskCarrierSettings()
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
                    Dispatcher.Invoke((System.Action)(() =>
                    {
                        _window.Logger.DontPanicErrorText = $"JavaScript error caught, restarting extraction...";
                    }));
                    AutoItX.WinActivate("Script Error");
                    AutoItX.Send("!y");
                    throw new Exception();
                }
            }
        }

        private async void btnRunExtraction_Click(object sender, RoutedEventArgs e)
        {
            GlobalDataBindingValues.Instance.extractionStartTime = DateTime.Now;

            _window?.Logger.NewExtraction("Deal Risk Carrier Extraction Started");
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
            // TODO: Bind this textbox
            var drcFolder =
                $"\\\\pcibtighnas1\\CBSData\\Portfolio Analysis\\Data\\DRC\\{DateTime.Now:MMMMyyyy}";
            var csvDrcFilePath =
                $"{drcFolder}\\DRCs {DateTime.Now:yyyy-MM-dd}.csv";
            var xlsxDrcFilePath =
                $"{drcFolder}\\DRCs {DateTime.Now:yyyy-MM-dd}.xlsx";

            const int maxFailureCount = 3;
            for (var failureCount = 0; failureCount < maxFailureCount; failureCount++)
            {
                try
                {
                    var currentAdaptivEnvironment =
                        _window.CmbBxAdaptivEnvironments.SelectedValue.ToString();
                    var successfulLogin = await Task.Run(() =>
                        _window.OpenAdaptivAndLogin(username, password,
                            currentAdaptivEnvironment));

                    if (!successfulLogin)
                    {
                        _window.Logger.ErrorText = "Failed to run deal risk carrier extraction!";
                        return;
                    }

                    #region wait for browser
                    _window.completedLoading = false;
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

                    await Task.Run(() => Thread.Sleep(1000));
                    _window.WebBrowser.Document?.InvokeScript(
                        nameof(JsScripts.OpenRiskView));

                    #region wait for browser
                    _window.completedLoading = false;
                    for (var i = 0; i < 3; i++)
                    {
                        while (!_window.completedLoading)
                        {
                            await Task.Run(() => Thread.Sleep(100));
                        }
                        _window.completedLoading = false;
                    }

                    await Task.Run(() => Thread.Sleep(2000));

                    #endregion wait for browser

                    _window.Logger.OkayText = $"Filtering for Deal Risk Carriers...";

                    _window.InjectJavascript(
                        nameof(JsScripts.FilterRiskViewOnInstruments),
                        JsScripts.FilterRiskViewOnInstruments);

                    _window.WebBrowser.Document?.InvokeScript(
                        nameof(JsScripts.FilterRiskViewOnInstruments),
                        new object[] { "Deal Risk Carrier" });

                    #region wait for browser
                    _window.completedLoading = false;
                    while (!_window.completedLoading)
                    {
                        await Task.Run(() => Thread.Sleep(100));
                    }

                    await Task.Run(() => Thread.Sleep(1000));
                    _window.completedLoading = false;
                    #endregion wait for browser

                    Action methodName = JavaScriptErrorDialogFound;
                    IAsyncResult result = methodName.BeginInvoke(null, null);

                    _window.InjectJavascript(nameof(JsScripts.ExportToCsv),
                        JsScripts.ExportToCsv);
                    _window.WebBrowser.Document?.InvokeScript(
                        nameof(JsScripts.ExportToCsv));

                    #region wait for browser

                    while (!_window.completedLoading)
                    {
                        await Task.Run(() => Thread.Sleep(100));
                    }

                    await Task.Run(() => Thread.Sleep(1000));
                    _window.completedLoading = false;

                    #endregion wait for browser

                    methodName.EndInvoke(result);

                    while (_window.WebBrowser.Document?.GetElementsByTagName("A").Count == 0)
                    {
                        await Task.Run(() => Thread.Sleep(100));
                    }

                    foreach (HtmlElement link in _window.WebBrowser.Document?
                        .GetElementsByTagName("A"))
                    {
                        if (link.InnerText.Equals("exported file link"))
                            link.InvokeMember("Click");
                    }

                    var overrideExistingFile = (bool)chkBxOverrideExistingFiles.IsChecked;
                    await Task.Run(() => SaveDrcFile(overrideExistingFile));

                    if (overrideExistingFile && File.Exists(xlsxDrcFilePath))
                    {
                        File.Delete(xlsxDrcFilePath);
                        Thread.Sleep(1000);
                    }

                    MainWindow.ConvertWorkbookFormats(csvDrcFilePath, ".xlsx");

                    while (!File.Exists(xlsxDrcFilePath))
                    {
                        await Task.Run(() => Thread.Sleep(100));
                    }

                    await Task.Run(() => Thread.Sleep(2000));
                    File.Delete(csvDrcFilePath);

                    // TODO: Extract this to a function in FileUtils

                    //var fileSize = new FileInfo(xlsxDrcFilePath).Length >= 1048576
                    //    ? $"{new FileInfo(xlsxDrcFilePath).Length / 1048576:n}" + " MB"
                    //    : $"{new FileInfo(xlsxDrcFilePath).Length / 1024:n}" + " KB";

                    #region excel merging

                    Excel.Application xlApp = new Excel.Application();
                    Excel.Workbook wb = xlApp.Workbooks.Open(xlsxDrcFilePath);
                    const string wsName = "All RC Files Combined";

                    Dispatcher.Invoke((System.Action)(() =>
                    {
                        _window.Logger.OkayText =
                            "Reading contents of Risk Carrier Files on shared drive..."; 
                    }));

                    var csvContents = new List<string>();


                    foreach (var rcf in Directory.GetFiles(@"\\pcibtignass1\capr2\RtB\DRC\Upload\"))
                    {
                        csvContents.AddRange(FileUtils.Read(rcf, "Reference"));
                    }
                    
                    var titles = new[]
                    {
                        "Reference", "Counterparty", "Maturity Date", "Risk Profile",
                        "Exposure Currency", "Risk Measure", "Active", "Include Deals",
                        "Exclude Deals", "Effective Product", "Type",
                        "Exclude From Country Risk", "Override Country of Risk",
                        "Trade Date", "Source System", "Booking Branch",
                        "Expected Risk Profile", "Book Definition",
                        "Netting Agreement Reference", "Portfolio Type", "Cut Off Date"
                    };

                    // convert the contents of all the appended CSV files to object[,]

                    Dispatcher.Invoke((System.Action)(() =>
                    {
                        _window.Logger.OkayText =
                            "Merging contents of Risk Carrier Files...";
                    }));
                    
                    var outputLists = csvContents.Select(x => x.Split(',')).ToList();
                    var maxListLength = outputLists.Select(x => x.Length).Max();
                    var output = new object[outputLists.Count, maxListLength];

                    for (var i = 0; i < outputLists.Count; i++)
                    {
                        for (var j = 0; j < maxListLength; j++)
                        {
                            output[i, j] = j < outputLists[i].Length
                                ? outputLists[i][j]
                                : "";
                        }
                    }

                    Dispatcher.Invoke((System.Action)(() =>
                    {
                        _window.Logger.OkayText =
                            "Writing merged contents of Risk Carrier Files to DRC...";
                    }));
                    
                    ExcelUtils.WriteOutputBlockToExcel(xlApp, wb, wsName, titles, output, 0);
                    #endregion excel merging
                    wb.Save();
                    wb.Close();
                    xlApp.Quit();

                    Dispatcher.Invoke((Action)(() =>
                    {
                        if (_window.extractedFiles.Any(x => x.FilePath == xlsxDrcFilePath))
                        {
                            _window.extractedFiles.Remove(_window.extractedFiles.First(x => x.FilePath == xlsxDrcFilePath));
                        }

                        _window.extractedFiles.Add(new ExtractedFile()
                        {
                            FilePath = xlsxDrcFilePath,
                            FileName = Path.GetFileName(xlsxDrcFilePath),
                            FileType = $"Risk View : DRCs",
                            FileSize = FileUtils.FileSize(xlsxDrcFilePath)
                        });
                    }));
                    break;
                }
                catch (Exception exception)
                {
                    if (failureCount < 2)
                    {
                        _window.Logger.ErrorText =
                            $"Something failed for DRC extraction. {exception.Message}\nTrying again. Attempt number: {failureCount + 2}";
                    }
                    else
                    {
                        _window.Logger.ErrorText = "DRC extraction failed 3 times. Moving on to next instrument set.";
                    }

                }
            }

            _window.Logger.ExtractionComplete("DRC Extraction");

            GlobalDataBindingValues.Instance.extractionEndTime = DateTime.Now;
            var timeSpan = GlobalDataBindingValues.Instance.extractionEndTime
                            - GlobalDataBindingValues.Instance.extractionStartTime;
            _window.Logger.OkayText
                = $"Extraction took: {timeSpan.Minutes} minutes {timeSpan.Seconds % 60} seconds";
            _window.WebBrowser.Url = new Uri("C:\\GitLab\\AdaptivBot\\ExtractionComplete.html");
        }


        public async void SaveDrcFile(bool overrideExistingFile)
        {
            AutoItX.WinWait("File Download", timeout: 20);
            AutoItX.WinActivate("File Download");
            AutoItX.Send("{TAB 3}");
            AutoItX.Send("{ENTER}");
            Dispatcher.Invoke((Action)(() =>
            {
                _window.Logger.OkayText = $"Saving CSV file for DRCs...";
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
                        _window.Logger.WarningText =
                            $"Overriding existing file for DRCs...";
                    }));
                }
                else
                {
                    AutoItX.Send("!n");
                    Dispatcher.Invoke((Action)(() =>
                    {
                        _window.Logger.WarningText =
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
    }
}