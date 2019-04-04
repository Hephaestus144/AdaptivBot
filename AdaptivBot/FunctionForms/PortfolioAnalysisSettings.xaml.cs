using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            txtBxProductionFolder.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtBxUATFolder.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            xdp.Document.Save(GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath);
            xdp.Refresh();
            _window.Logger.OkayText = "Portfolio Analysis Settings saved!";
        }
    }
}
