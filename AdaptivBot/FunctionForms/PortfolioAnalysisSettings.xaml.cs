using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace AdaptivBot.SettingForms
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class PortfolioAnalysisSettings : Page
    {
        private readonly MainWindow _window = (MainWindow)Application.Current.MainWindow;

        public PortfolioAnalysisSettings()
        {
            InitializeComponent();
        }

        
        private void PortfolioAnalysisSettings_OnLoaded(object sender, RoutedEventArgs e)
        {
            var xdp =
                (XmlDataProvider)this.Resources["PortfolioAnalysisSettingsXml"];

            xdp.Source = new Uri(GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath);
        }

        
        private void BtnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            var xdp = (XmlDataProvider)this.Resources["PortfolioAnalysisSettingsXml"];
            xdp.Refresh();
            txtBxProductionFolder.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            txtBxUATFolder.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            xdp.Document.Save(GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath);
            _window.Logger.OkayText = "Portfolio Analysis Settings saved.";
            xdp.Refresh();
            OnSettingsSaved(EventArgs.Empty);
        }


        //public event EventHandler SettingsSaved;

        public void UpdateTargets(object sender, EventArgs e)
        {
            var xdp = (XmlDataProvider)this.Resources["PortfolioAnalysisSettingsXml"];
            xdp.Refresh();
            txtBxProductionFolder.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
            txtBxUATFolder.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
        }

        public void OnSettingsSaved(EventArgs e)
        {
            EventHandler handler = _window.ConfigFileChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
