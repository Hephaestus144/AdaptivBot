using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace AdaptivBot.SettingForms
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class CustomerLimitUtilisationSettings : Page
    {
        public CustomerLimitUtilisationSettings()
        {
            InitializeComponent();
        }


        private void CustomerLimitUtilisationSettings_OnLoaded(object sender, RoutedEventArgs e)
        {
            var xdp =
                (XmlDataProvider) this.Resources["CustomerLimitUtilisationSettingsXml"];
            xdp.Source = new Uri(GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath);
        }


        public async void btnRunExtraction_Click(object sender, RoutedEventArgs e)
        {
            var window = (MainWindow)Application.Current.MainWindow;
            var date = datePicker.SelectedDate;
            
            if (date is null)
            {
                window.logger.ErrorText = "Please select a date for extraction.";
                return;
            }

            if (!window.StoreUserCredentials())
            {
                return;
            }

            window.logger.NewExtraction("Customer Limit Utilisation Report Extraction Started");

            // TODO: Use binding here.
            var username = window.txtUserName.Text;
            var password = window.txtPasswordBox.Password;

            var currentAdaptivEnvironment =
                window.cmbBxAdaptivEnvironments.SelectedValue.ToString();

            window.StoreUserCredentials();

            await Task.Run(() => window.OpenAdaptivAndLogin(username, password, currentAdaptivEnvironment));

            #region wait for browser
            while (!window.completedLoading)
            {
                await Task.Run(() => Thread.Sleep(100));
            }
            await Task.Run(() => Thread.Sleep(1000));
            window.completedLoading = false;
            while (!window.completedLoading)
            {
                await Task.Run(() => Thread.Sleep(100));
            }
            await Task.Run(() => Thread.Sleep(1000));
            window.completedLoading = false;
            #endregion wait for browser

            window.InjectJavascript(
                nameof(JsScripts.OpenCustomerLimitUtilisationReport),
                JsScripts.OpenCustomerLimitUtilisationReport);

            window.webBrowser.Document?.InvokeScript(nameof(JsScripts.OpenCustomerLimitUtilisationReport));

            #region wait for browser
            while (!window.completedLoading)
            {
                await Task.Run(() => Thread.Sleep(100));
            }
            await Task.Run(() => Thread.Sleep(1000));
            window.completedLoading = false;
            while (!window.completedLoading)
            {
                await Task.Run(() => Thread.Sleep(100));
            }
            await Task.Run(() => Thread.Sleep(1000));
            window.completedLoading = false;
            #endregion wait for browser


            window.InjectJavascript(
                nameof(JsScripts.FilterCustomerLimitUtilisationReport),
                JsScripts.FilterCustomerLimitUtilisationReport);
            window.webBrowser.Document?.InvokeScript(nameof(JsScripts.FilterCustomerLimitUtilisationReport));

            #region wait for browser
            window.completedLoading = false;
            while (!window.completedLoading)
            {
                await Task.Run(() => Thread.Sleep(100));
            }
            await Task.Run(() => Thread.Sleep(1000));
            window.completedLoading = false;
            #endregion wait for browser

            window.InjectJavascript(
                nameof(JsScripts.ChooseCustomerLimitUtilisationReport),
                JsScripts.ChooseCustomerLimitUtilisationReport);
            await Task.Run(() => Thread.Sleep(1000));
            window.webBrowser.Document?.InvokeScript(nameof(JsScripts.ChooseCustomerLimitUtilisationReport));

            window.injectedScripts.Clear();


            #region wait for browser
            window.completedLoading = false;
            while (!window.completedLoading)
            {
                await Task.Run(() => Thread.Sleep(100));
            }
            await Task.Run(() => Thread.Sleep(1000));
            window.completedLoading = false;
            #endregion wait for browser
            await Task.Run(() => Thread.Sleep(1000));


            window.InjectJavascript(
                nameof(JsScripts.SelectCustomerLimitUtilisationReportDate),
                JsScripts.SelectCustomerLimitUtilisationReportDate);

            window.webBrowser.Document?.InvokeScript(
                nameof(JsScripts.SelectCustomerLimitUtilisationReportDate),
                new object[] { ((DateTime)date).ToString("dd/MM/yyyy") });

            #region wait for browser
            window.completedLoading = false;
            while (!window.completedLoading)
            {
                await Task.Run(() => Thread.Sleep(100));
            }
            await Task.Run(() => Thread.Sleep(1000));
            window.completedLoading = false;
            #endregion wait for browser

            window.InjectJavascript(
                nameof(JsScripts.GenerateCustomerLimitUtilisationReport),
                JsScripts.GenerateCustomerLimitUtilisationReport);

            window.webBrowser.Document?.InvokeScript(
                nameof(JsScripts.GenerateCustomerLimitUtilisationReport));

            while (window.webBrowser.Document?.GetElementsByTagName("img").Count < 5)
            {
                await Task.Run(() => Thread.Sleep(1000));
            }
            await Task.Run(() => Thread.Sleep(3000));

            window.InjectJavascript(
                nameof(JsScripts.ExportCustomerLimitUtilisationReportToCsv),
                JsScripts.ExportCustomerLimitUtilisationReportToCsv);

            window.webBrowser.Document?.InvokeScript(nameof(JsScripts.ExportCustomerLimitUtilisationReportToCsv));

            var overrideExistingFile = true;
            //(bool)chkBxOverrideExistingFiles.IsChecked;

            await Task.Run(() => Thread.Sleep(1000));
            await Task.Run(() => window.SaveCustomerLimitUtilisationReport((DateTime)date, overrideExistingFile));

            var csvFile = $"\\\\pcibtighnas1\\CBSData\\Portfolio Analysis\\Data\\Cust Util\\SBG\\CustomerLimitUtil {date:dd.MM.yyyy}.csv";

            MainWindow.ConvertWorkbookFormats(csvFile, ".csv", ".xlsx");
            window.logger.OkayText = "Complete";
        }


        #region filtering
        private void CmbBxFilterCategory1_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbBxFilterCategory2 == null) return;
            if (((ComboBoxItem)cmbBxFilterCategory1.SelectedItem).Tag.ToString() == "Hide")
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


        private void CmbBxFilterCategory2_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbBxFilterCategory3 == null) return;
            if (((ComboBoxItem)cmbBxFilterCategory2.SelectedItem).Tag.ToString() == "Hide")
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
                txtBxFilterCriteria2.Visibility = Visibility.Visible;
            }
        }
        #endregion filtering
    }
}
