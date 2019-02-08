using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptivBot
{
    public static class InstrumentLists
    {
        public static string bondInstruments
        = "Bond Forward, Bond Future, "
        + "Bond Total Return Swap, Bond Future Option, Bond Option, Bond Spot, "
        + "Bond Derivative Projected Cash Flow";

        public static string commodityAgricultureInstruments
            = "Agricultural Variation Margin, Agricultural Total Margin For Parenting, "
            + "Agricultural Future, Agricultural Future Option, Agricultural Forward, "
            + "Agricultural Option";

        public static string commodityBaseMetalInstruments
            = "Base Metal Forward, Base Metal Average Forward, Base Metal Option, "
            + "Base Metal Average Option, Base Metal Future, Base Metal Future Option,"
            + "Base Metal Future Average Option, Base Metal Swap Leg, "
            + "Base Metal Average Swap, Base Metal Projected Cash Flow";

        public static string commodityEnergyInstruments
            = "Energy Future, Energy Future Option, Energy Option, Energy Swap, "
            + "Energy Spread Swap, Emission Forward, Emission Future, Emission Option, "
            + "Emission Future Option, Energy Projected Cash Flow";

        public static string commodityPreciousMetalInstruments
            = "Future Metal Flow, Precious Metal Forward, Precious Metal Average Forward, "
            + "Precious Metal Forward Rate Agreement, Precious Metal Future, "
            + "Precious Metal Future Option, Precious Metal Option, "
            + "Precious Metal Average Option, Lease Rate Swap, Metal Lease Margined, "
            + "Precious Metal Projected Cash Flow, Future Metal Flow";

        public static string creditInstruments
            = "Credit Default Swap, " +
            "Credit Basket Default Swap, Credit Index Default Swap, Nth to Default Swap, " +
            "Credit Default Swap Option, Credit Linked Note, Nth to Default CLN, " +
            "Bond Total Return Swap, Credit Derivative Projected Cash Flow";

        public static string equityInstruments = "Equity Forward, Equity Option, " +
            "Equity Asian Option,Equity Barrier Option, Equity Cliquet Option, " +
            "Equity Future, Equity Future Option, Equity Swap, Equity Dividend Swap, " +
            "Equity Variance Swap, Equity Warrant, Contract For Difference, " +
            "Equity Projected Cash Flow";

        public static string fxInstruments = "Foreign Exchange Forward, " +
            "Foreign Exchange Average Forward, Foreign Exchange Future, " +
            "Foreign Exchange Option, Foreign Exchange Average Option, " +
            "Foreign Exchange Barrier Option, Foreign Exchange Swap Leg, " +
            "Foreign Exchange Swap, Foreign Exchange Range Accrual, " +
            "Cross Currency Swap, Foreign Exchange Projected Cash Flow";

        public static string interestRateInstruments = "Cap Or Floor, " +
            "Forward Rate Agreement, Interest Rate Future, Interest Rate Future Option, " +
            "Interest Rate Swap, Basis Swap, Average Rate Swap, Constant Maturity Swap, " +
            "Inflation Linked Swap, Overnight Index Swap, Range Accrual Swap, " +
            "Swaption, Inflation Zero Coupon Swap, Inflation Accrual Swap, " +
            "Interest Rate Projected CashFlow";

        public static string secfinBondInstruments = "Bond Repo or Reverse Repo, " +
            "Bond Tripartite Repo or Reverse Repo, Bond Buy or Sell Back Leg, " +
            "Bond Forward Purchase or Sale, Bond Loan or Borrow, Debt Collateral";

        public static string secfinEquityInstruments = "Equity Repo or Reverse Repo, " +
            "Equity Tripartite Repo or Reverse Repo , Equity Forward Purchase or Sale, " +
            "Equity Buy or Sell Back Leg, Equity Loan or Borrow, " +
            "Equity Financing Loan or Borrow, Equity Collateral";

        public static Dictionary<string, string> InstrumentGuiMapping =
            new Dictionary<string, string>()
            {
                ["Commodities : Agri"] = "Agri",
                ["Commodities : Base Metals"] = "BM",
                ["Commodities : Energy"] = "Energy",
                ["Commodities : Precious Metals"] = "PM",
                ["Credit"] = "Credit",
                ["Equities"] = "Equity",
                ["Foreign Exchange"] = "FX",
                ["Interest Rates"] = "IR",
                ["Securities Financing : Bonds"] = "SecFinBond",
                ["Securities Financing : Equities"] = "SecFinEquity"
            };


        public static Dictionary<string, string> instruments
            = new Dictionary<string, string>()
            {
                ["Agri"] = commodityAgricultureInstruments,
                ["BM"] = commodityBaseMetalInstruments,
                ["Bond"] = bondInstruments,
                ["Credit"] = creditInstruments,
                ["Energy"] = commodityEnergyInstruments,
                ["Equity"] = equityInstruments,
                ["FX"] = fxInstruments,
                ["IR"] = interestRateInstruments,
                ["PM"] = commodityPreciousMetalInstruments,
                ["SecFinBond"] = secfinBondInstruments,
                ["SecFinEquity"] = secfinEquityInstruments
            };
    }
}
