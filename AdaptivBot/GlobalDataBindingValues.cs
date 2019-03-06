using NodaTime;
using System;
using System.ComponentModel;


namespace AdaptivBot
{
    public sealed class GlobalDataBindingValues : INotifyPropertyChanged
    {
        #region properties
        private DateTime _displayDateEnd;

        public DateTime DisplayDateEnd
        {
            get => _displayDateEnd;
            set
            {
                if (_displayDateEnd != value)
                {
                    _displayDateEnd = value;
                    this.OnPropertyChanged(nameof(DisplayDateEnd));
                }
            }
        }


        private string _adaptivBotConfigFilePath;

        public string AdaptivBotConfigFilePath
        {
            get => _adaptivBotConfigFilePath;
            set
            {
                if (_adaptivBotConfigFilePath != value)
                {
                    _adaptivBotConfigFilePath = value;
                    this.OnPropertyChanged(nameof(AdaptivBotConfigFilePath));
                }
            }
        }


        private string _adaptivBotDirectory;

        public string AdaptivBotDirectory
        {
            get => _adaptivBotDirectory;
            set
            {
                if (_adaptivBotDirectory != value)
                {
                    _adaptivBotDirectory = value;
                    this.OnPropertyChanged(nameof(AdaptivBotDirectory));
                }
            }
        }
        #endregion properties


        #region constructors
        private static readonly object padlock = new object();
        private static GlobalDataBindingValues instance = null;

        GlobalDataBindingValues()
        {
            // Used in the DatePickers. Users cannot select date after & including today's date.
            var displayEndDate = LocalDate.FromDateTime(DateTime.Now.AddDays(-1));
            switch (displayEndDate.DayOfWeek)
            {
                case IsoDayOfWeek.Saturday:
                    displayEndDate = displayEndDate.PlusDays(-1);
                    break;
                case IsoDayOfWeek.Sunday:
                    displayEndDate = displayEndDate.PlusDays(-2);
                    break;
            }
            this.DisplayDateEnd = displayEndDate.ToDateTimeUnspecified();
        }


        public static GlobalDataBindingValues Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new GlobalDataBindingValues();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion constructors

        
        #region events
        public event PropertyChangedEventHandler PropertyChanged;


        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }
        #endregion events
    }
}
