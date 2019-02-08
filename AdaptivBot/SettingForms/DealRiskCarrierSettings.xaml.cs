using System;
using System.Collections.Generic;
using System.IO;
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
        public DealRiskCarrierSettings()
        {
            InitializeComponent();
        }


        private async void btnRunExtraction_Click(object sender, RoutedEventArgs e)
        {
            GlobalConfigValues.Instance.extractionStartTime = DateTime.Now;
            var window = (MainWindow)Application.Current.MainWindow;

            window?.logger.NewExtraction("Deal Risk Carrier Extraction Started");
            if (!window.StoreUserCredentials())
            {
                return;
            }

            // TODO: Use binding here.
            var username = window.txtUserName.Text;
            var password = window.txtPasswordBox.Password;
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
                    //#region adaptiv extraction
                    //var currentAdaptivEnvironment =
                    //    window.cmbBxAdaptivEnvironments.SelectedValue.ToString();
                    //await Task.Run(() =>
                    //    window.OpenAdaptivAndLogin(username, password,
                    //        currentAdaptivEnvironment));

                    //#region wait for browser

                    //while (!window.completedLoading)
                    //{
                    //    await Task.Run(() => Thread.Sleep(100));
                    //}

                    //await Task.Run(() => Thread.Sleep(2000));
                    //window.completedLoading = false;

                    //#endregion wait for browser

                    //#region wait for browser

                    //while (!window.completedLoading)
                    //{
                    //    await Task.Run(() => Thread.Sleep(100));
                    //}

                    //await Task.Run(() => Thread.Sleep(2000));
                    //window.completedLoading = false;

                    //#endregion wait for browser

                    //window.InjectJavascript(nameof(JsScripts.OpenRiskView),
                    //    JsScripts.OpenRiskView);

                    //await Task.Run(() => Thread.Sleep(1000));
                    //window.webBrowser.Document?.InvokeScript(
                    //    nameof(JsScripts.OpenRiskView));

                    //#region wait for browser

                    //for (int i = 0; i < 3; i++)
                    //{
                    //    while (!window.completedLoading)
                    //    {
                    //        await Task.Run(() => Thread.Sleep(100));
                    //    }

                    //    window.completedLoading = false;
                    //}

                    //await Task.Run(() => Thread.Sleep(2000));

                    //#endregion wait for browser

                    //window.logger.OkayText = $"Filtering for Deal Risk Carriers...";
                    //window.InjectJavascript(
                    //    nameof(JsScripts.FilterRiskViewOnInstruments),
                    //    JsScripts.FilterRiskViewOnInstruments);

                    //window.webBrowser.Document?.InvokeScript(
                    //    nameof(JsScripts.FilterRiskViewOnInstruments),
                    //    new object[] {"Deal Risk Carrier"});

                    //#region wait for browser

                    //while (!window.completedLoading)
                    //{
                    //    await Task.Run(() => Thread.Sleep(100));
                    //}

                    //await Task.Run(() => Thread.Sleep(1000));
                    //window.completedLoading = false;

                    //#endregion wait for browser


                    //window.InjectJavascript(nameof(JsScripts.ExportToCsv),
                    //    JsScripts.ExportToCsv);
                    //window.webBrowser.Document?.InvokeScript(
                    //    nameof(JsScripts.ExportToCsv));

                    //await Task.Run(() => Thread.Sleep(500));

                    //#region wait for browser

                    //while (!window.completedLoading)
                    //{
                    //    await Task.Run(() => Thread.Sleep(100));
                    //}

                    //await Task.Run(() => Thread.Sleep(1000));
                    //window.completedLoading = false;

                    //#endregion wait for browser

                    //while (window.webBrowser.Document?.GetElementsByTagName("A").Count ==
                    //       0)
                    //{
                    //    await Task.Run(() => Thread.Sleep(100));
                    //}

                    //foreach (HtmlElement link in window.webBrowser.Document?
                    //    .GetElementsByTagName("A"))
                    //{
                    //    if (link.InnerText.Equals("exported file link"))
                    //        link.InvokeMember("Click");
                    //}

                    //var overrideExistingFile =
                    //    (bool) chkBxOverrideExistingFiles.IsChecked;
                    //await Task.Run(() =>
                    //    window.SaveDrcFile(overrideExistingFile));

                    //await Task.Run(() =>
                    //    MainWindow.ConvertWorkbookFormats(csvDrcFilePath, ".csv", ".xlsx"));

                    //await Task.Run(() => Thread.Sleep(2000));
                    //File.Delete(csvDrcFilePath);
                    //csvDrcFilePath = csvDrcFilePath.Replace(".csv", ".xlsx");

                    //var fileSize = new FileInfo(csvDrcFilePath).Length >= 1048576
                    //    ? $"{new FileInfo(csvDrcFilePath).Length / 1048576:n}" + " MB"
                    //    : $"{new FileInfo(csvDrcFilePath).Length / 1024:n}"    + " KB";

                    //var path = csvDrcFilePath;
                    //Dispatcher.Invoke((Action)(() =>
                    //{
                    //    window.extractedFiles.Add(new ExtractedFile()
                    //    {
                    //        FilePath = path,
                    //        FileName = Path.GetFileName(path),
                    //        FileType = $"Risk View : DRCs",
                    //        FileSize = fileSize
                    //    });
                    //}));
                    
                    //#endregion adaptiv extraction


                    #region excel merging

                    Excel.Application xlApp = new Excel.Application();
                    Excel.Workbook wb = xlApp.Workbooks.Open(xlsxDrcFilePath);
                    var wsName = "All RC Files Combined";

                    var csvContents = new List<object[,]>();
                    var riskCarrierFilePaths =
                        Directory.GetFiles(@"\\pcibtignass1\capr2\RtB\DRC\Upload\");

                    foreach (var rcf in riskCarrierFilePaths)
                    {
                        csvContents.Add(FileUtils.Read(rcf, "Reference"));
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

                    var objectSizeDimension0 = 0;
                    var objectSizeDimension1 = 0;
                    for (var i = 0; i < csvContents.Count; i++)
                    {
                        if (!(csvContents[i] is null))
                        {
                            objectSizeDimension0 += csvContents[i].GetLength(0);
                            objectSizeDimension1 = Math.Max(objectSizeDimension1,
                                csvContents[i].GetLength(1));
                        }
                    }

                    var mergedDrcOutput =
                        new object[objectSizeDimension0, objectSizeDimension1];
                    var currentRow = 0;

                    for (var i = 0; i < csvContents.Count; i++)
                    {
                        if (!(csvContents[i] is null))
                        {
                            for (var j = 0; j < csvContents[i].GetLength(0); j++)
                            {
                                for (var k = 0; k < csvContents[i].GetLength(1); k++)
                                {
                                    mergedDrcOutput[currentRow, k] =
                                        csvContents[i][j, k] ?? "";
                                }

                                currentRow++;
                            }
                        }
                    }

                    ExcelUtils.WriteOutputBlockToExcel(xlApp, wb, wsName, titles, mergedDrcOutput, 0);
                    #endregion excel merging
                    wb.Save();
                    wb.Close();
                    xlApp.Quit();
                    break;
                }
                catch (Exception)
                {
                    window.logger.ErrorText = errorCount < 2
                        ? $"Something failed for DRC extraction. Trying again. Attempt number: {++errorCount}"
                        : "DRC extraction failed 3 times. Moving on to next instrument set.";
                }
            }

            window.logger.ExtractionComplete("DRC Extraction");

            GlobalConfigValues.Instance.extractionEndTime = DateTime.Now;
            var timeSpan = GlobalConfigValues.Instance.extractionEndTime
                            - GlobalConfigValues.Instance.extractionStartTime;

            window.logger.OkayText
                = $"Extraction took: {timeSpan.Minutes} minutes {timeSpan.Seconds % 60} seconds";
            window.webBrowser.Url = new Uri("C:\\GitLab\\AdaptivBot\\ExtractionComplete.html");

            // merge RCFs
        }
    }
}
