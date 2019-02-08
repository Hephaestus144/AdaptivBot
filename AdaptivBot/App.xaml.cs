using System;
using System.IO;
using System.Windows;
using System.Xml.Linq;


namespace AdaptivBot
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            GlobalConfigValues.Instance.AdaptivBotDirectory
                = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder
                        .LocalApplicationData), "AdaptivBot");

            GlobalConfigValues.Instance.AdaptivBotConfigFilePath
                = Path.Combine(GlobalConfigValues.Instance.AdaptivBotDirectory,
                    "AdaptivBot.config");

            
            // TODO: Replace GlobalConfigValues with GlobalDataBindingValues
            GlobalDataBindingValues.Instance.AdaptivBotConfigFilePath =
                GlobalConfigValues.Instance.AdaptivBotConfigFilePath;

            if (!Directory.Exists(GlobalConfigValues.Instance.AdaptivBotDirectory))
            {
                Directory.CreateDirectory(GlobalConfigValues.Instance.AdaptivBotDirectory);
            }

            if (!File.Exists(GlobalConfigValues.Instance.AdaptivBotConfigFilePath))
            {
                File.WriteAllText(
                    GlobalConfigValues.Instance.AdaptivBotConfigFilePath,
                    GlobalConfigValues.defaultConfigFileContent);
                GlobalConfigValues.CreatedConfigFile = YesNoMaybe.Yes;
            }

            var document =
                XDocument.Load(GlobalConfigValues.Instance.AdaptivBotConfigFilePath, LoadOptions.PreserveWhitespace);

            if (File.Exists(GlobalConfigValues.possibleExcelPath1)
                && document.Root.Element("GeneralSettings").Element("ExcelExecutablePath").Value == "")
            {
                document.Root.Element("GeneralSettings").Element("ExcelExecutablePath").Value = GlobalConfigValues.possibleExcelPath1;
                document.Save(GlobalConfigValues.Instance.AdaptivBotConfigFilePath);
                GlobalConfigValues.ExcelPathConfigured = YesNoMaybe.Yes;
            }
            else if (document.Root.Element("GeneralSettings").Element("ExcelExecutablePath").Value == "")
            {
                GlobalConfigValues.ExcelPathConfigured = YesNoMaybe.No;
            }

            var configDocument =
                XDocument.Load(GlobalConfigValues.Instance.AdaptivBotConfigFilePath);
            GlobalConfigValues.excelPath =
                configDocument.Root.Element("GeneralSettings").Element("ExcelExecutablePath").Value;

        }
    }
}
