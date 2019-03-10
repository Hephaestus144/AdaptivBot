using AdaptivBot;
using System.Collections.Generic;
using Xunit;


namespace AdaptivBotTests
{
    public class JsScriptsTests
    {
        [Fact]
        public void FilterCustomerLimitUtilisationReport()
        {
            var expectedScript
                = "function FilterCustomerLimitUtilisationReport()"                                            +
                  "{"                                                                                          +
                  "  frames['cpWork'].ToggleAdvanced();"                                                       +
                  "  var ListFilterFrame = frames['cpWork'].frames['ListFilterFrame'];"                        +
                  "  var advancedDiv = ListFilterFrame.document.body.getElementsByTagName('div')['advanced'];" +
                  "  var advancedInnerDiv = advancedDiv.getElementsByTagName('div')['advancedInner'];"         +
                  "  var filtersTable = advancedInnerDiv.getElementsByTagName('table')['Filters'];"            +
                  "  filtersTable.getElementsByTagName('select')['Ext'].selectedIndex = '1';"                  +
                  "  filtersTable.getElementsByTagName('select')['Ext'].onchange();"                           +
                  "  filtersTable.getElementsByTagName('select')['Ext'] [1].selectedIndex='1';"                +
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
            var conjunctions = new List<string> { "And", "And" };
            var fields = new List<string> { "Product", "Measure", "Portfolio" };
            var conditions = new List<string> { "equal to", "equal to", "equal to" };
            var criteria = new List<string> { "Trading", "Pre-Settlement", "Customer Cube" };
            var actualScript = JsScripts.FilterCustomerLimitUtilisationReport(fields, conditions, criteria, conjunctions);
            Assert.Equal(expectedScript, actualScript);
        }
    }
}
