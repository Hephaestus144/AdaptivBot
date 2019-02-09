using System.ComponentModel;
using System.Runtime.CompilerServices;
using CredentialManagement;
using System.Windows;
using AdaptivBot.Annotations;


namespace AdaptivBot
{
    public sealed class CredentialStore : INotifyPropertyChanged
    {
        private readonly MainWindow _window = (MainWindow)Application.Current.MainWindow;

        private Credential _credential;

        public Credential Credentials
        {
            get => _credential;
            set
            {
                if (_credential != value)
                {
                    _credential = value;
                    this.OnPropertyChanged("Credentials");
                }
            }
        }

        public bool credentialsFound;


        public CredentialStore(string target)
        {
            Credentials = new Credential { Target = target };
            credentialsFound = Credentials.Load();
        }

        public event PropertyChangedEventHandler PropertyChanged;


        private void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
