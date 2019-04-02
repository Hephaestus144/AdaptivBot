using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;


namespace AdaptivBot.SettingForms
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class GeneralSettings : Page
    {
        private readonly MainWindow _window = (MainWindow)Application.Current.MainWindow;

        public GeneralSettings()
        {
            InitializeComponent();
        }


        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            var configDocument =
                XDocument.Load(GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath);
            if (txtBxExcelPath.Text
                != configDocument.Root.Element("GeneralSettings").Element("ExcelExecutablePath").Value)
            {
                configDocument.Root.Element("GeneralSettings").Element("ExcelExecutablePath").Value =
                    txtBxExcelPath.Text;
                configDocument.Save(GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath);
            }
        }


        private void GeneralSettings_OnLoaded(object sender, RoutedEventArgs e)
        {
            var xdp = (XmlDataProvider) this.Resources["GeneralSettingsXml"];
            xdp.Source = new Uri(GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath);
        }

        private void BtnUpdateAdaptivCredentials_OnClick(object sender, RoutedEventArgs e)
        { 
            CredentialStore.Instance.UpdateCredentials();
        }

        private void BtnPurgeAdaptivCredentials_Click(object sender, RoutedEventArgs e)
        {
            CredentialStore.Instance.PurgeCredentials();
        }
    }
}
