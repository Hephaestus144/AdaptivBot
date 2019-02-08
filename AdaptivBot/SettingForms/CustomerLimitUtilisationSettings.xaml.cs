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

        private void CmbBxFilterCategory1_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbBxFilterCategory2 == null) return;
            if (((ComboBoxItem)cmbBxFilterCategory1.SelectedItem).Tag.ToString() == "Hide")
            {
                cardFilterCategory2.Visibility = Visibility.Hidden;
                cmbBxFilterCategory2.Visibility = Visibility.Hidden;
                cmbBxFilterCategory2.SelectedIndex = 0;

                cardFilterOperation2.Visibility = Visibility.Hidden;
                cmbBxFilterOperation2.Visibility = Visibility.Hidden;
                cmbBxFilterOperation2.SelectedIndex = 0;

                cardFilterCriteria2.Visibility = Visibility.Hidden;
                txtBxFilterCriteria2.Visibility = Visibility.Hidden;
                txtBxFilterCriteria2.Text = "";
            }
            else
            {
                cardFilterCategory2.Visibility = Visibility.Visible;
                cmbBxFilterCategory2.Visibility = Visibility.Visible;

                cardFilterOperation2.Visibility = Visibility.Visible;
                cmbBxFilterOperation2.Visibility = Visibility.Visible;

                cardFilterCriteria2.Visibility = Visibility.Visible;
                txtBxFilterCriteria2.Visibility = Visibility.Visible;
            }
        }

        private void CmbBxFilterCategory2_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbBxFilterCategory3 == null) return;
            if (((ComboBoxItem)cmbBxFilterCategory2.SelectedItem).Tag.ToString() == "Hide")
            {
                cardFilterCategory3.Visibility = Visibility.Hidden;
                cmbBxFilterCategory3.Visibility = Visibility.Hidden;
                cmbBxFilterCategory3.SelectedIndex = 0;

                cardFilterOperation3.Visibility = Visibility.Hidden;
                cmbBxFilterOperation3.Visibility = Visibility.Hidden;
                cmbBxFilterOperation3.SelectedIndex = 0;

                cardFilterCriteria3.Visibility = Visibility.Hidden;
                txtBxFilterCriteria3.Visibility = Visibility.Hidden;
                txtBxFilterCriteria3.Text = "";
            }
            else
            {
                cardFilterCategory3.Visibility = Visibility.Visible;
                cmbBxFilterCategory3.Visibility = Visibility.Visible;

                cardFilterOperation3.Visibility = Visibility.Visible;
                cmbBxFilterOperation3.Visibility = Visibility.Visible;

                cardFilterCriteria3.Visibility = Visibility.Visible;
                txtBxFilterCriteria2.Visibility = Visibility.Visible;
            }
        }
    }
}
