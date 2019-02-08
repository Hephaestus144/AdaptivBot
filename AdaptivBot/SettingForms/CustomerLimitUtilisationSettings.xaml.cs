using System;
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
    }
}
