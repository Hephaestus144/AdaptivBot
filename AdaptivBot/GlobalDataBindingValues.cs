﻿using NodaTime;
using System;
using System.Collections.Generic;
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


        private string _extractionCompleteWithoutErrors;

        public string ExtractionCompleteWithoutErrors
        {
            get => _extractionCompleteWithoutErrors;
            set
            {
                if (_extractionCompleteWithoutErrors != value)
                {
                    _extractionCompleteWithoutErrors = value;
                    this.OnPropertyChanged(nameof(ExtractionCompleteWithoutErrors));
                }
            }
        }


        private string _extractionCompleteWithErrors;

        public string ExtractionCompleteWithErrors
        {
            get => _extractionCompleteWithErrors;
            set
            {
                if (_extractionCompleteWithErrors != value)
                {
                    _extractionCompleteWithErrors = value;
                    this.OnPropertyChanged(nameof(ExtractionCompleteWithErrors));
                }
            }
        }

        private string _extractionCompleteWithWarnings;

        public string ExtractionCompleteWithWarnings
        {
            get => _extractionCompleteWithWarnings;
            set
            {
                if (_extractionCompleteWithWarnings != value)
                {
                    _extractionCompleteWithWarnings = value;
                    this.OnPropertyChanged(nameof(ExtractionCompleteWithWarnings));
                }
            }
        }

        // It's difficult to generically determine the location of Excel
        public static List<string> PossibleExcelPaths
            = new List<string>
            { 
                 @"C:\Program Files (x86)\Microsoft Office\root\Office16\Excel.exe",
                 @"C:\Program Files\Microsoft Office\root\Office16\Excel.exe"
            };

        public static string actualExcelPath;

        public DateTime extractionStartTime;
        public DateTime extractionEndTime;

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
