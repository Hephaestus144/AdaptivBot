using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptivBot
{
    public static class JsScripts
    {
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


        public static string SelectCustomerLimitUtilisationReport
            = "function SelectCustomerLimitUtilisationReport()" +
              "{"                                                                         +
              "  var element = document.frames['cpWork'].document.getElementById('ReportsCombo');" +
              "  element.selectedIndex = '1';"                                            +
              "  element.onchange();"                                                     +
              "}";

        public static string GenerateCustomerLimitUtilisationReport
            = "function GenerateCustomerLimitUtilisationReport(date)"                                  +
              "{"                                                                                      +
              "  document.getElementById('ReportViewerMain_ctl04_ctl03_txtValue').value=(date);"       +
              "  document.getElementById('ReportViewerMain_ctl04_ctl05_ddValue').selectedIndex = '1';" +
              "  document.getElementById('ReportViewerMain_ctl04_ctl07_ddValue').selectedIndex = '1';" +
              "  document.getElementById('ReportViewerMain_ctl04_ctl00').click();"                     +
              "}";


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
