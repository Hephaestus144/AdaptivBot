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

        public static string defaultConfigFileContent
            = "<AdaptivBot>\n"                                                                                                    +
              "\t<ExcelExecutablePath></ExcelExecutablePath>"                                                                     +
              "\n"                                                                                                                +
              "\t<GeneralSettings>\n"                                                                                             +
              "\t\t<QlikviewUATFolder>\\\\10952APPJNB0001\\PS.Portfolio_Sensitivities\\8.Import\\New</QlikviewUATFolder>\n"       +
              "\t\t<QlikviewProductionFolder>\\\\10952appprdsdc4\\Portfolio Analysis\\8.Import\\New</QlikviewProductionFolder>\n" +
              "\t</GeneralSettings>"                                                                                              +
              "\n"                                                                                                                +
              "\t<RiskViewSettings>\n"                                                                                            +
              "\t\t<BaseFolder>\\\\pcibtighnas1\\cbsdata\\Portfolio Analysis\\Data</BaseFolder>\n"                                +
              "\t</RiskViewSettings>\n"                                                                                           +
              "\n"                                                                                                                +
              "\t<CustomerLimitUtilisationSettings>\n"                                                                            +
              "\t\t<BaseFolder>\\\\pcibtighnas1\\cbsdata\\Portfolio Analysis\\Data\\Cust Util</BaseFolder>\n"                     +
              "\t</CustomerLimitUtilisationSettings>\n"                                                                           +
              "\n"                                                                                                                +
              "\t<DealRiskCarrierSettings>\n"                                                                                     +
              "\t\t<BaseFolder>\\\\pcibtighnas1\\cbsdata\\Portfolio Analysis\\Data\\DRC</BaseFolder>\n"                           +
              "\t</DealRiskCarrierSettings>\n"                                                                                    +
              "\t</AdaptivBot>\n";

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
