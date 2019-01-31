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
using System.Windows.Shapes;
using System.Xml.Linq;

namespace AdaptivBot.SettingForms
{
    /// <summary>
    /// Interaction logic for GeneralSettings.xaml
    /// </summary>
    public partial class GeneralSettings : Window
    {
        public GeneralSettings()
        {
            InitializeComponent();
        }

        private void btnCancelSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            var configDocument =
                XDocument.Load(GlobalConfigValues.Instance.adaptivBotConfigFile);
            if (txtBxExcelExePath.Text !=
                configDocument.Root.Element("ExcelExecutablePath").Value)
            {
                configDocument.Root.Element("ExcelExecutablePath").Value =
                    txtBxExcelExePath.Text;
                configDocument.Save(GlobalConfigValues.Instance.adaptivBotConfigFile);
            }
        }

        private void GeneralSettings_OnLoaded(object sender, RoutedEventArgs e)
        {
            var configDocument =
                XDocument.Load(GlobalConfigValues.Instance.adaptivBotConfigFile);
            txtBxExcelExePath.Text =
                configDocument.Root.Element("ExcelExecutablePath").Value;
        }
    }
}
