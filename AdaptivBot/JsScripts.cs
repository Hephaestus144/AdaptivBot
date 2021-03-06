﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;


namespace AdaptivBot
{
    public static class JsScripts
    {
        public static ObservableCollection<string> CustomerLimitUtilisationFilterFields
            = new ObservableCollection<string>
            {
                "-",
                "Customer Code",
                "Customer Name",
                "Group Credit Manager",
                "Primary Credit Manager",
                "Secondary Credit Manager",
                "Relationship Manager",
                "Portfolio",
                "Product",
                "Host",
                "Customer Branch",
                "Commodity",
                "Security Type",
                "Book Definition",
                "Measure",
                "Limit Review Date",
                "Limit Expiry Date",
                "Review Date",
                "Industry Name",
                "Country of Risk",
                "Ultimate Parent Code",
                "Ultimate Parent Name",
                "Internal Risk Grade Local",
                "Internal Risk Grade Foreign",
                "Standard and Poors",
                "Moodys",
                "Fitch",
                "Sub Limit",
                "Approving Delegated Authority Levels (DA)",
                "Approving Delegated Authority Names",
                "Reason for Request"
            };

        public static ObservableCollection<string> RiskViewFilterFields
            = new ObservableCollection<string>
            {
                "-",
                "Reference",
                "Structure",
                "Facility",
                "Product",
                "Details",
                "Lead Account",
                "Booking Branch",
                "Customer",
                "Customer Name",
                "Counterparty",
                "Counterparty Name",
                "Cpty Class",
                "Issuer Code",
                "Issuer Name",
                "Trade Date",
                "End Date",
                "Entered",
                "State",
                "Violations",
                "Entered By",
                "Amount",
                "Currency",
                "Amount2",
                "Currency2",
                "Nominal",
                "Mark To Market",
                "Analytic MTM",
                "Pre Settlement",
                "Primary",
                "Settlement",
                "Issuer",
                "Gross",
                "Gross Liquid",
                "Concentration",
                "Expected",
                "Agreement",
                "Pool",
                "Payment Agreement",
                "Payment Pool",
                "SWWR",
                "GWWR",
                "Liquidity Rating",
                "Underlying Asset",
                "Currency of Underlying Asset",
                "Previous Mark To Market",
                "Risk Carrier",
                "Portfolio Name",
                "Rollup",
                "Source System"
            };

        public static ObservableCollection<string> CustomerLimitUtilisationFilterConditions
            = new ObservableCollection<string>
            {
                "-",
                "starts with",
                "contains",
                "does not contain",
                "equal to",
                "not equal to",
                "greater than",
                "greater than or equal to",
                "less than",
                "less than or equal to",
                "in",
                "not in"
            };

        public static ObservableCollection<string> RiskViewFilterConditions
            = new ObservableCollection<string>
            {
                "-",
                "starts with",
                "contains",
                "does not contain",
                "equal to",
                "not equal to",
                "greater than",
                "greater than or equal to",
                "less than",
                "less than or equal to",
                "in",
                "not in"
            };

        public static string OpenRiskView
            = "function OpenRiskView() " +
              "{" +
              "  window.open('Deal_Home.aspx?List=CRS.Deals.RiskView','cpWork');" +
              "}";

        public static string OpenCustomerLimitUtilisationReport
            = "function OpenCustomerLimitUtilisationReport() " +
              "{"                                                                 +
              "  window.open('List_Main.htm?ListID=Reports.RSReports.CustomerLimitUtilisation','cpWork');" +
              "}";

        public static string ChooseCustomerLimitUtilisationReport
            = "function ChooseCustomerLimitUtilisationReport()" +
              "{"                                                                         +
              "  var element = document.frames['cpWork'].document.getElementById('ReportsCombo');" +
              "  element.selectedIndex = '1';"                                            +
              "  element.onchange();"                                                     +
              "}";

        public static string SelectCustomerLimitUtilisationReportDate
            = "function SelectCustomerLimitUtilisationReportDate(date)" +
              "{"                                                                                      +
              "  document.getElementById('ReportViewerMain_ctl04_ctl03_txtValue').value=(date);"       +
              "  document.getElementById('ReportViewerMain_ctl04_ctl03_txtValue').onchange();"         +
              "}";

        public static string GenerateCustomerLimitUtilisationReport
            = "function GenerateCustomerLimitUtilisationReport()"                                      +
              "{"                                                                                      +
              "  document.getElementById('ReportViewerMain_ctl04_ctl05_ddValue').selectedIndex = '1';" +
              "  document.getElementById('ReportViewerMain_ctl04_ctl07_ddValue').selectedIndex = '1';" +
              "  document.getElementById('ReportViewerMain_ctl04_ctl00').click();"                     +
              "}";

        public static string FilterCustomerLimitUtilisationReportForPortfolioAnalysis
            = "function FilterCustomerLimitUtilisationReport()"                                            +
              "{"                                                                                          +
              "  frames['cpWork'].ToggleAdvanced();"                                                       +
              "  var ListFilterFrame = frames['cpWork'].frames['ListFilterFrame'];"                        +                                                   
              "  var advancedDiv = ListFilterFrame.document.body.getElementsByTagName('div')['advanced'];" +
              "  var advancedInnerDiv = advancedDiv.getElementsByTagName('div')['advancedInner'];"         +
              "  var filtersTable = advancedInnerDiv.getElementsByTagName('table')['Filters'];"            +
              "  filtersTable.getElementsByTagName('select')['Ext'].selectedIndex = '1';"                  +
              "  filtersTable.getElementsByTagName('select')['Ext'].onchange();"                           +
              "  filtersTable.getElementsByTagName('select')['Ext'] [1].selectedIndex='1';"                + // "AND"
              "  filtersTable.getElementsByTagName('select')['Ext'] [1].onchange();"                       +
              "  filtersTable.getElementsByTagName('select')['Fields'] [0].selectedIndex = '7';"           +
              "  filtersTable.getElementsByTagName('select')['Fields'] [1].selectedIndex = '13';"          +
              "  filtersTable.getElementsByTagName('select')['Fields'] [2].selectedIndex = '6';"           +
              "  filtersTable.getElementsByTagName('select')['Condition'] [0].selectedIndex = '3';"        +
              "  filtersTable.getElementsByTagName('select')['Condition'] [1].selectedIndex = '3';"        +
              "  filtersTable.getElementsByTagName('select')['Condition'] [2].selectedIndex = '3';"        +
              "  filtersTable.getElementsByTagName('input')['Criteria'] [0].value = 'Trading';"            +
              "  filtersTable.getElementsByTagName('input')['Criteria'] [1].value = 'Pre-Settlement';"     +
              "  filtersTable.getElementsByTagName('input')['Criteria'] [2].value = 'Customer Cube';"      +
              "  frames['cpWork'].ApplyFilter();"                                                          +
              "}";

        public static List<string> FilterConjunctions = new List<string> { "", "And", "Or" };

        public static string FilterCustomerLimitUtilisationReport(
            List<string> fields,
            List<string> conditions,
            List<string> criteria,
            List<string> conjunctions)
        {
            const string scriptPreamble
                = "function FilterCustomerLimitUtilisationReport()" +
                  "{" +
                  "  frames['cpWork'].ToggleAdvanced();" +
                  "  var ListFilterFrame = frames['cpWork'].frames['ListFilterFrame'];" +
                  "  var advancedDiv = ListFilterFrame.document.body.getElementsByTagName('div')['advanced'];" +
                  "  var advancedInnerDiv = advancedDiv.getElementsByTagName('div')['advancedInner'];" +
                  "  var filtersTable = advancedInnerDiv.getElementsByTagName('table')['Filters'];";

            var stringBuilder = new StringBuilder(scriptPreamble);
            
            for (var i = 0; i < conjunctions.Count; i++)
            {
                if (i == 0)
                {
                    stringBuilder
                        .Append(
                            "  filtersTable.getElementsByTagName('select')['Ext'].selectedIndex = '")
                        .Append(FilterConjunctions.IndexOf(conjunctions[i])).Append("';");

                    stringBuilder.Append("  filtersTable.getElementsByTagName('select')['Ext'].onchange();");
                }
                else
                {
                    stringBuilder
                        .Append("  filtersTable.getElementsByTagName('select')['Ext'] [")
                        .Append(i).Append("].selectedIndex='")
                        .Append(FilterConjunctions.IndexOf(conjunctions[i])).Append("';");

                    stringBuilder
                        .Append("  filtersTable.getElementsByTagName('select')['Ext'] [")
                        .Append(i).Append("].onchange();");
                }
            }

            for (var i = 0; i < fields.Count; i++)
            {
                stringBuilder
                    .Append("  filtersTable.getElementsByTagName('select')['Fields'] [")
                    .Append(i).Append("].selectedIndex = '")
                    .Append(CustomerLimitUtilisationFilterFields.IndexOf(fields[i]) - 1).Append("';");
            }

            for (var i = 0; i < conditions.Count; i++)
            {
                stringBuilder
                    .Append(
                        "  filtersTable.getElementsByTagName('select')['Condition'] [")
                    .Append(i).Append("].selectedIndex = '")
                    .Append(CustomerLimitUtilisationFilterConditions.IndexOf(conditions[i]) - 1).Append("';");
            }

            for (var i = 0; i < criteria.Count; i++)
            {
                stringBuilder
                    .Append("  filtersTable.getElementsByTagName('input')['Criteria'] [")
                    .Append(i).Append("].value = '").Append(criteria[i]).Append("';");
            }

            stringBuilder.Append("  frames['cpWork'].ApplyFilter();");
            stringBuilder.Append("}");

            return stringBuilder.ToString();
        }


        public static string ExportCustomerLimitUtilisationReportToCsv
            = "function ExportCustomerLimitUtilisationReportToCsv()" +
              "{"                                                    +
              "  $find('ReportViewerMain').exportReport('CSV');"     +
              "}";


        // Update this to add filter criteria
        public static string FilterRiskViewOnInstruments
            = "function FilterRiskViewOnInstruments(instruments)" +
              "{" +
              "  var frameInsideCpWork = frames['cpWork'].frames[0];" +
              "  var ListFilterFrame = frames['cpWork'].frames[0].frames['ListFilterFrame'];" +
              "  frameInsideCpWork.ToggleAdvanced();" +
              "  var advancedDiv = ListFilterFrame.document.body.getElementsByTagName('div')['advanced'];" +
              "  var advancedInnerDiv = advancedDiv.getElementsByTagName('div')['advancedInner'];" +
              "  var filtersTable = advancedInnerDiv.getElementsByTagName('table')['Filters'];" +
              "  var fieldsDropDown = filtersTable.getElementsByTagName('select')['Fields'];" +
              "  var criteria = filtersTable.getElementsByTagName('input')['Criteria'];" +
              "  criteria.value = instruments;" +
              "  fieldsDropDown.selectedIndex = '3';" +
              "  var conditionDropDown = filtersTable.getElementsByTagName('select')['Condition'];" +
              "  conditionDropDown.selectedIndex = '9';" +
              "  frameInsideCpWork.ApplyFilter();" +
              "}";


        public static string ExportToCsv
            = "function ExportToCsv()" +
              "{" +
              "  var frameInsideCpWork = frames['cpWork'].frames[0];" +
              "  frameInsideCpWork.RunExport('list2csv');" +
              "}";
    }
}