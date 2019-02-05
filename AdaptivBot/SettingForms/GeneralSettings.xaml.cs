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

        private void GeneralSettings_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            var configDocument =
                XDocument.Load(GlobalConfigValues.Instance.adaptivBotConfigFile);
            if (txtBxExcelPath.Text !=
                configDocument.Root.Element("ExcelExecutablePath").Value)
            {
                configDocument.Root.Element("ExcelExecutablePath").Value =
                    txtBxExcelPath.Text;
                configDocument.Save(GlobalConfigValues.Instance.adaptivBotConfigFile);
            }
        }

        private void GeneralSettings_OnLoaded(object sender, RoutedEventArgs e)
        {
            var configDocument =
                XDocument.Load(GlobalConfigValues.Instance.adaptivBotConfigFile);
            txtBxExcelPath.Text =
                configDocument.Root.Element("ExcelExecutablePath").Value;
        }
    }
}
