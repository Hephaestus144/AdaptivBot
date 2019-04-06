using System;
using System.IO;
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

        private void BtnDeleteAdaptivBotConfigFile_OnClick(object sender, RoutedEventArgs e)
        {
            if (File.Exists(GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath))
            {
                File.Delete(GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath);
                _window.Logger.WarningText = "Config file deleted!";
                _window.Logger.WarningText = "Beware that this significantly impacts functionality.";
            }
            else
            {
                _window.Logger.WarningText = "Config file not found!";
            }
        }

        private void BtnResetAdaptivBotConfigFile_OnClick(object sender, RoutedEventArgs e)
        {
            if(File.Exists(GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath))
            {
                File.Delete(GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath);
                _window.Logger.WarningText = "Config file deleted!";
                try
                {
                    File.WriteAllText(
                        GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath,
                        Properties.Resources.AdaptivBot);
                    EventHandler handler = _window.ConfigFileChanged;
                    if(handler != null)
                    {
                        handler(this, e);
                    }
                    _window.Logger.OkayText = $"Config file reset : {GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath}";
                }
                catch(Exception exception)
                {
                    _window.Logger.ErrorText = $"Exception caught: {exception.Message}";
                    _window.Logger.ErrorText = "Config file not created! Limited functionality.";
                }
            }
            else
            {
                _window.Logger.WarningText = "Config file not found!";
                _window.Logger.WarningText = "Config file not reset!";
            }
        }


        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            var xdp = (XmlDataProvider)this.Resources["GeneralSettingsXml"];
            //xdp.Refresh();
            txtBxExcelPath.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            txtBxAdaptivBotConfigFilePath.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            xdp.Document.Save(GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath);
            _window.Logger.OkayText = "General settings saved.";
            xdp.Refresh();
            OnSettingsSaved(EventArgs.Empty);
        }

        public void UpdateTargets(object sender, EventArgs e)
        {
            var xdp = (XmlDataProvider)this.Resources["GeneralSettingsXml"];
            xdp.Refresh();
            txtBxAdaptivBotConfigFilePath.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
            txtBxExcelPath.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
        }


        public void OnSettingsSaved(EventArgs e)
        {
            EventHandler handler = _window.ConfigFileChanged;
            if(handler != null)
            {
                handler(this, e);
            }
        }
    }
}
