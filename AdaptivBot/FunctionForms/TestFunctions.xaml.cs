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
    public partial class TestFunctions : Page
    {
        private readonly MainWindow _window = (MainWindow)Application.Current.MainWindow;

        public TestFunctions()
        {
            InitializeComponent();
        }


        private void BtnTestExtractionCompleteWithoutErrors_OnClick(object sender, RoutedEventArgs e)
        {
            _window.WebBrowser.Url =
                new Uri(GlobalDataBindingValues.Instance.ExtractionCompleteWithoutErrors);
        }

        private void BtnTestExtractionCompleteWithErrors_OnClick(object sender, RoutedEventArgs e)
        {
            _window.WebBrowser.Url =
                new Uri(GlobalDataBindingValues.Instance.ExtractionCompleteWithErrors);
        }

        private void BtnTestExtractionCompleteWithWarnings_OnClick(object sender, RoutedEventArgs e)
        {
            _window.WebBrowser.Url =
                new Uri(GlobalDataBindingValues.Instance.ExtractionCompleteWithWarnings);
        }
    }
}
