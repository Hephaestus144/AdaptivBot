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
using WindowStartupLocation = System.Windows.WindowStartupLocation;

namespace AdaptivBot
{
    /// <summary>
    /// Interaction logic for AlertUpdateUserCredentials.xaml
    /// </summary>
    public partial class AlertUpdateUserCredentials : Window
    {
        public bool UpdateCredentials;
        public bool CancelRun = false;

        public AlertUpdateUserCredentials()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }


        public void btnYes_Click(object sender, RoutedEventArgs e)
        {
            UpdateCredentials = true;
            this.Close();
        }


        public void btnNo_Click(object sender, RoutedEventArgs e)
        {
            UpdateCredentials = false;
            this.Close();
        }

        private void btnCancelRun_Click(object sender, RoutedEventArgs e)
        {
            UpdateCredentials = false;
            CancelRun = true;
            this.Close();
        }
    }
}
