using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string adaptivBotDirectory;
        public string adaptivBotConfigFile;
        public static string possibleExcelPath1
            = @"C:\Program Files (x86)\Microsoft Office\root\Office16\Excel.exe";

        public static string possibleExcelPath2
            = @"C:\Program Files\Microsoft Office\root\Office16\Excel.exe";

        public static string excelPath;

        public static YesNoMaybe excelPathConfigured = YesNoMaybe.Maybe;

        public static YesNoMaybe createdConfigFile = YesNoMaybe.Maybe;

        public static string defaultConfigFileContent
            = "<AdaptivBot>\n"                                                          +
              "\t<ExcelExecutablePath></ExcelExecutablePath>"                           +
              "\n"                                                                      +
              "\t<RiskViewReports>\n"                                                   +
              "\t\t<BaseExtractionFolderPath></BaseExtractionFolderPath>\n"             +
              "\t\t<UATExtractionServerPath></UATExtractionServerPath>\n"               +
              "\t\t<ProductionExtractionServerPath></ProductionExtractionServerPath>\n" +
              "\t</RiskViewReports>\n"                                                  +
              "\n"                                                                      +
              "\t<CustomerLimitUtilisationReports>\n"                                   +
              "\t\t<BaseExtractionFolderPath></BaseExtractionFolderPath>\n"             +
              "\t\t<UATExtractionServerPath></UATExtractionServerPath>\n"               +
              "\t\t<ProductionExtractionServerPath></ProductionExtractionServerPath>\n" +
              "\t</CustomerLimitUtilisationReports>\n"                                  +
              "\n"                                                                      +
              "\t<DealRiskCarrierReport>\n"                                             +
              "\t\t<BaseExtractionFolderPath></BaseExtractionFolderPath>\n"             +
              "\t\t<UATExtractionServerPath></UATExtractionServerPath>\n"               +
              "\t\t<ProductionExtractionServerPath></ProductionExtractionServerPath>\n" +
              "\t</DealRiskCarrierReport>\n"                                            +
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
