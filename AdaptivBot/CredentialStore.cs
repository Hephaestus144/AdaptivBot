using AutoIt;
using CredentialManagement;
using System;
using System.ComponentModel;
using System.Security;
using System.Windows;
using System.Windows.Controls;


namespace AdaptivBot
{
    public sealed class CredentialStore : INotifyPropertyChanged
    {
        private readonly MainWindow _window = (MainWindow)Application.Current.MainWindow;


        private static CredentialStore _instance;

        private static readonly object Padlock = new object();


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
                this.Credentials.Username = _window.txtUserName.Text;
                this.Credentials.Password = _window.TxtPasswordBox.Password;
                this.Credentials.PersistanceType =
                    PersistanceType.LocalComputer;
                this.Credentials.Save();
                _window.Logger.OkayText = "Saving credentials...";
                return true;
            }

            if (this.credentialsFound
                && (this.Credentials.Username != _window.txtUserName.Text
                || this.Credentials.Password != _window.TxtPasswordBox.Password))
            {
                var window = new AlertUpdateUserCredentials();
                window.Show();
                return false;
                // TODO: Add message box to warn user that the credentials that have been entered are different 
                // to the saved credentials & would they like to save them?
            }

            if (_window.txtUserName.Text == "")
            {
                _window.Logger.ErrorText = "User name blank.";
                return false;
            }

            if (_window.TxtPasswordBox.Password == "")
            {
                _window.Logger.ErrorText = "Password blank.";
                return false;
            }

            return true;
        }

        public void EnterAdaptivCredentials(string username, string password)
        {
            AutoItX.Sleep(3000);
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
            }
            else
            {
                _window.Dispatcher.BeginInvoke(
                    (Action)(() => { _window.Logger.OkayText = "Adaptiv already open."; }));
            }

            _window.Dispatcher.BeginInvoke(
                (Action)(() => { _window.Logger.OkayText = "Acknowledging disclaimer..."; }));

            AutoItX.WinWait("Adaptiv Disclaimer -- Webpage Dialog");
            AutoItX.WinActivate("Adaptiv Disclaimer -- Webpage Dialog");
            AutoItX.Send("{ENTER}");
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
