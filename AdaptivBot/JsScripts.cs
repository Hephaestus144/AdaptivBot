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
