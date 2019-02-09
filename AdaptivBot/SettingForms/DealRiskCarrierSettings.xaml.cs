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


        private async void btnRunExtraction_Click(object sender, RoutedEventArgs e)
        {
            GlobalConfigValues.Instance.extractionStartTime = DateTime.Now;

            _window?.logger.NewExtraction("Deal Risk Carrier Extraction Started");
            if (!_window.StoreUserCredentials())
            {
                return;
            }

            // TODO: Use binding here.
            var username = _window.txtUserName.Text;
            var password = _window.TxtPasswordBox.Password;
            // TODO: Bind this textbox
            var drcFolder =
                $"\\\\pcibtighnas1\\CBSData\\Portfolio Analysis\\Data\\DRC\\{DateTime.Now:MMMMyyyy}";
            var csvDrcFilePath =
                $"{drcFolder}\\DRCs {DateTime.Now:yyyy-MM-dd}.csv";
            var xlsxDrcFilePath =
                $"{drcFolder}\\DRCs {DateTime.Now:yyyy-MM-dd}.xlsx";

            for (var errorCount = 0; errorCount < 3; errorCount++)
            {
                try
                {
                    #region adaptiv extraction
                    var currentAdaptivEnvironment =
                        _window.cmbBxAdaptivEnvironments.SelectedValue.ToString();
                    await Task.Run(() =>
                        _window.OpenAdaptivAndLogin(username, password,
                            currentAdaptivEnvironment));

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

                    await Task.Run(() => Thread.Sleep(1000));
                    _window.webBrowser.Document?.InvokeScript(
                        nameof(JsScripts.OpenRiskView));

                    #region wait for browser

                    for (int i = 0; i < 3; i++)
                    {
                        while (!_window.completedLoading)
                        {
                            await Task.Run(() => Thread.Sleep(100));
                        }

                        _window.completedLoading = false;
                    }

                    await Task.Run(() => Thread.Sleep(2000));

                    #endregion wait for browser

                    _window.logger.OkayText = $"Filtering for Deal Risk Carriers...";
                    _window.InjectJavascript(
                        nameof(JsScripts.FilterRiskViewOnInstruments),
                        JsScripts.FilterRiskViewOnInstruments);

                    _window.webBrowser.Document?.InvokeScript(
                        nameof(JsScripts.FilterRiskViewOnInstruments),
                        new object[] { "Deal Risk Carrier" });

                    #region wait for browser

                    while (!_window.completedLoading)
                    {
                        await Task.Run(() => Thread.Sleep(100));
                    }

                    await Task.Run(() => Thread.Sleep(1000));
                    _window.completedLoading = false;

                    #endregion wait for browser


                    _window.InjectJavascript(nameof(JsScripts.ExportToCsv),
                        JsScripts.ExportToCsv);
                    _window.webBrowser.Document?.InvokeScript(
                        nameof(JsScripts.ExportToCsv));

                    await Task.Run(() => Thread.Sleep(500));

                    #region wait for browser

                    while (!_window.completedLoading)
                    {
                        await Task.Run(() => Thread.Sleep(100));
                    }

                    await Task.Run(() => Thread.Sleep(1000));
                    _window.completedLoading = false;

                    #endregion wait for browser

                    while (_window.webBrowser.Document?.GetElementsByTagName("A").Count ==
                           0)
                    {
                        await Task.Run(() => Thread.Sleep(100));
                    }

                    foreach (HtmlElement link in _window.webBrowser.Document?
                        .GetElementsByTagName("A"))
                    {
                        if (link.InnerText.Equals("exported file link"))
                            link.InvokeMember("Click");
                    }

                    var overrideExistingFile =
                        (bool)chkBxOverrideExistingFiles.IsChecked;
                    await Task.Run(() =>
                        SaveDrcFile(overrideExistingFile));

                    await Task.Run(() =>
                        MainWindow.ConvertWorkbookFormats(csvDrcFilePath, ".csv", ".xlsx"));

                    await Task.Run(() => Thread.Sleep(2000));
                    File.Delete(csvDrcFilePath);
                    csvDrcFilePath = csvDrcFilePath.Replace(".csv", ".xlsx");

                    var fileSize = new FileInfo(csvDrcFilePath).Length >= 1048576
                        ? $"{new FileInfo(csvDrcFilePath).Length / 1048576:n}" + " MB"
                        : $"{new FileInfo(csvDrcFilePath).Length / 1024:n}" + " KB";

                    var path = csvDrcFilePath;
                    Dispatcher.Invoke((Action)(() =>
                    {
                        _window.extractedFiles.Add(new ExtractedFile()
                        {
                            FilePath = path,
                            FileName = Path.GetFileName(path),
                            FileType = $"Risk View : DRCs",
                            FileSize = fileSize
                        });
                    }));

                    #endregion adaptiv extraction


                    #region excel merging

                    Excel.Application xlApp = new Excel.Application();
                    Excel.Workbook wb = xlApp.Workbooks.Open(xlsxDrcFilePath);
                    var wsName = "All RC Files Combined";

                    _window.logger.OkayText = "Reading contents of Risk Carrier Files on shard drive...";
                    var csvContents = new List<string>();
                    var riskCarrierFilePaths =
                        Directory.GetFiles(@"\\pcibtignass1\capr2\RtB\DRC\Upload\");

                    foreach (var rcf in riskCarrierFilePaths)
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

                    // convert the contents of the all the appended CSV files to object[,]
                    _window.logger.OkayText = "Merging contents of Risk Carrier Files...";
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

                    _window.logger.OkayText = "Writing merged contents of Risk Carrier Files to DRC...";
                    ExcelUtils.WriteOutputBlockToExcel(xlApp, wb, wsName, titles, output, 0);
                    #endregion excel merging
                    wb.Save();
                    wb.Close();
                    xlApp.Quit();
                    break;
                }
                catch (Exception)
                {
                    _window.logger.ErrorText = errorCount < 2
                        ? $"Something failed for DRC extraction. Trying again. Attempt number: {++errorCount}"
                        : "DRC extraction failed 3 times. Moving on to next instrument set.";
                }
            }

            _window.logger.ExtractionComplete("DRC Extraction");

            GlobalConfigValues.Instance.extractionEndTime = DateTime.Now;
            var timeSpan = GlobalConfigValues.Instance.extractionEndTime
                            - GlobalConfigValues.Instance.extractionStartTime;
            _window.logger.OkayText
                = $"Extraction took: {timeSpan.Minutes} minutes {timeSpan.Seconds % 60} seconds";
            _window.webBrowser.Url = new Uri("C:\\GitLab\\AdaptivBot\\ExtractionComplete.html");

        }


        public async void SaveDrcFile(bool overrideExistingFile)
        {
            AutoItX.WinWait("File Download", timeout: 20);
            AutoItX.WinActivate("File Download");
            AutoItX.Send("{TAB 3}");
            AutoItX.Send("{ENTER}");
            Dispatcher.Invoke((Action)(() =>
            {
                _window.logger.OkayText = $"Saving CSV file for DRCs...";
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
                        _window.logger.WarningText =
                            $"Overriding existing file for DRCs...";
                    }));
                }
                else
                {
                    AutoItX.Send("!n");
                    Dispatcher.Invoke((Action)(() =>
                    {
                        _window.logger.WarningText =
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
