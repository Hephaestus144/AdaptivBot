using AutoIt;
using CredentialManagement;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;


namespace AdaptivBot
{
    public sealed class CredentialStore : INotifyPropertyChanged
    {
        private readonly MainWindow _window = (MainWindow)Application.Current.MainWindow;


        private static CredentialStore _instance;

        private static readonly object Padlock = new object();

        public bool CancelRun = false;

        #region fields

        private Credential _credentials;

        public Credential Credentials
        {
            get => _credentials;
            set
            {
                if (_credentials != value)
                {
                    _credentials = value;
                    this.OnPropertyChanged("Credentials");
                }
            }
        }


        private string _target;

        public string Target
        {
            get => _target;
            set
            {
                if (_target != value)
                {
                    _target = "AdaptivBot" + value;
                    Credentials = new Credential { Target = _target };
                    credentialsFound = Credentials.Load();
                    if (credentialsFound)
                    {
                        Username = Credentials.Username;
                        Password = Credentials.Password;
                    }
                    else
                    {
                        Username = "";
                        Password = "";
                    }
                    this.OnPropertyChanged("Target");
                }
            }
        }


        private string _username;

        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    this.OnPropertyChanged("Username");
                }
            }
        }


        private string _password;

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    this.OnPropertyChanged("Password");
                }
            }
        }

        public bool credentialsFound;

        #endregion fields


        #region constructors

        private CredentialStore()
        { }

        public static CredentialStore Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Padlock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CredentialStore();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion constructors


        public bool StoreUserCredentials()
        {
            //this.Credentials.Target = $"AdaptivBot{_window.CmbBxAdaptivEnvironments.SelectedValue}";
            if (!this.credentialsFound && (bool)_window.chkBxRememberMe.IsChecked)
            {
                this.Credentials.Username = _window.TxtUserName.Text;
                this.Credentials.Password = _window.TxtPasswordBox.Password;
                this.Credentials.PersistanceType =
                    PersistanceType.LocalComputer;
                this.Credentials.Save();
                _window.Logger.OkayText = "Saving credentials...";
                return true;
            }

            if (this.credentialsFound
                && (this.Credentials.Username != _window.TxtUserName.Text
                || this.Credentials.Password != _window.TxtPasswordBox.Password))
            {
                var window = new AlertUpdateUserCredentials();
                window.ShowDialog();
                
                if (window.UpdateCredentials)
                {
                    this.Credentials.Username = _window.TxtUserName.Text;
                    this.Credentials.Password = _window.TxtPasswordBox.Password;
                    this.Credentials.PersistanceType = PersistanceType.LocalComputer;
                    this.Credentials.Save();
                    _window.Logger.WarningText = "Updating credentials...";
                    _window.Logger.OkayText = "Continuing with run...";
                }
                else if (!window.CancelRun)
                {
                    _window.Logger.WarningText = "Using new credentials but not updating old credentials...";
                    _window.Logger.OkayText = "Continuing with run...";
                }
                CancelRun = window.CancelRun;
            }

            if (_window.TxtUserName.Text?.Length == 0)
            {
                _window.Logger.ErrorText = "User name blank.";
                return false;
            }

            if (_window.TxtPasswordBox.Password?.Length != 0) return true;

            _window.Logger.ErrorText = "Password blank.";
            return false;
        }


        public bool EnterAdaptivCredentials(string username, string password)
        {
            // There doesn't seem to be a better way other than waiting 4s for the windows security
            for (var i = 0; i < 40; i++)
            {
                if (AutoItX.WinExists("Windows Security") == 0)
                {
                    AutoItX.Sleep(100);
                }
            }

            if (AutoItX.WinExists("Windows Security") != 0)
            {
                _window.Dispatcher.BeginInvoke((Action)(() =>
                {
                    _window.Logger.OkayText = "Entering credentials...";
                }));
                AutoItX.WinActivate("Windows Security");
                AutoItX.Send(username);
                AutoItX.Send("{TAB}");
                AutoItX.Send(password);
                AutoItX.Send("{TAB}");
                AutoItX.Send("{ENTER}");

                // check if credentials failed then ask user to update credentials
                AutoItX.Sleep(1000);
            }
            else
            {
                _window.Dispatcher.BeginInvoke(
                    (Action)(() => _window.Logger.OkayText = "Adaptiv already open."));
            }

            if (AutoItX.WinExists("Windows Security") != 0)
            {
                AutoItX.Send("!c");
                _window.Dispatcher.BeginInvoke((Action)(() =>
                {
                    _window.Logger.ErrorText = "Entering credentials failed. Please ensure they are up to date.";
                }));
                return false;
            }
            _window.Dispatcher.BeginInvoke(
                (Action)(() =>
                   _window.Logger.OkayText = "Acknowledging disclaimer..."));

            while (AutoItX.WinExists("Adaptiv Disclaimer -- Webpage Dialog") == 0)
            {
                Task.Run(() => Thread.Sleep(100));
            }

            AutoItX.WinActivate("Adaptiv Disclaimer -- Webpage Dialog");
            AutoItX.Send("{ENTER}");
            return true;
        }


        #region event handlers

        public event PropertyChangedEventHandler PropertyChanged;


        private void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion event handlers
    }
}