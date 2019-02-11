using System;


namespace AdaptivBot
{
    public enum YesNoMaybe
    {
        Yes,
        No,
        Maybe
    }


    public class GlobalConfigValues
    {
        private static GlobalConfigValues instance = null;

        private static readonly object padlock = new object();

        public string AdaptivBotDirectory;
        public string AdaptivBotConfigFilePath;
        public static string possibleExcelPath1
            = @"C:\Program Files (x86)\Microsoft Office\root\Office16\Excel.exe";

        public DateTime extractionStartTime;
        public DateTime extractionEndTime;

        public static string possibleExcelPath2
            = @"C:\Program Files\Microsoft Office\root\Office16\Excel.exe";

        public static string excelPath;

        public static YesNoMaybe ExcelPathConfigured = YesNoMaybe.Maybe;

        public static YesNoMaybe CreatedConfigFile = YesNoMaybe.Maybe;

        
        private GlobalConfigValues()
        { }

        public static GlobalConfigValues Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new GlobalConfigValues();
                        }
                    }
                }
                return instance;
            }
        }
    }
}
