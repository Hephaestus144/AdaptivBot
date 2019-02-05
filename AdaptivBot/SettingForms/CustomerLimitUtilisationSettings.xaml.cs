﻿using System;
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
    public partial class CustomerLimitUtilisationSettings : Page
    {
        public CustomerLimitUtilisationSettings()
        {
            InitializeComponent();
        }

        private void CustomerLimitUtilisationSettings_OnLoaded(object sender, RoutedEventArgs e)
        {
            var configDocument =
                XDocument.Load(GlobalConfigValues.Instance.adaptivBotConfigFile);
            txtBoxBaseFolder.Text =
                configDocument.Root.Element("ExcelExecutablePath").Value;
        }
    }
}
