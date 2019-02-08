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
    }
}
