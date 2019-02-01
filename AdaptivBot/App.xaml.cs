using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using CredentialManagement;

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

            GlobalConfigValues.Instance.adaptivBotDirectory
                = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder
                        .LocalApplicationData), "AdaptivBot");

            GlobalConfigValues.Instance.adaptivBotConfigFile
                = Path.Combine(GlobalConfigValues.Instance.adaptivBotDirectory,
                    "AdaptivBot.config");

            if (!Directory.Exists(GlobalConfigValues.Instance.adaptivBotDirectory))
            {
                Directory.CreateDirectory(GlobalConfigValues.Instance.adaptivBotDirectory);
            }

            if (!File.Exists(GlobalConfigValues.Instance.adaptivBotConfigFile))
            {
                File.WriteAllText(
                    GlobalConfigValues.Instance.adaptivBotConfigFile,
                    GlobalConfigValues.defaultConfigFileContent);
                GlobalConfigValues.createdConfigFile = YesNoMaybe.Yes;
            }

            var document =
                XDocument.Load(GlobalConfigValues.Instance.adaptivBotConfigFile, LoadOptions.PreserveWhitespace);

            if (File.Exists(GlobalConfigValues.possibleExcelPath1)
                && document.Root.Element("ExcelExecutablePath").Value == "")
            {
                document.Root.Element("ExcelExecutablePath").Value = GlobalConfigValues.possibleExcelPath1;
                document.Save(GlobalConfigValues.Instance.adaptivBotConfigFile);
                GlobalConfigValues.excelPathConfigured = YesNoMaybe.Yes;
            }
            else if (document.Root.Element("ExcelExecutablePath").Value == "")
            {
                GlobalConfigValues.excelPathConfigured = YesNoMaybe.No;
            }

            var configDocument =
                XDocument.Load(GlobalConfigValues.Instance.adaptivBotConfigFile);
            GlobalConfigValues.excelPath =
                configDocument.Root.Element("ExcelExecutablePath").Value;

        }
    }
}
