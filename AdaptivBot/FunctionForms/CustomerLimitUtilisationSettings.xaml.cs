using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using AutoIt;


namespace AdaptivBot.SettingForms
{
    public partial class EmailBugSuggestion : Page
    {
        private readonly MainWindow window = (MainWindow)Application.Current.MainWindow;

        public EmailBugSuggestion()
        {
            InitializeComponent();
        }

    }
}
