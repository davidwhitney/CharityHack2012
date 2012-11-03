using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using CharityHack2012.Code.Http;
using CharityHack2012.Models;
using HtmlAgilityPack;

namespace CharityHack2012.Code.Adapters
{
    public class CharityComissionAdapter
    {
        private readonly IHttpContentGetter _httpGet;

        public string CharityComissionBaseUri { get { return "http://www.charity-commission.gov.uk"; } }
        public string GeneralInfoPart { get { return "/Showcharity/RegisterOfCharities/CharityWithPartB.aspx?SubsidiaryNumber=0&RegisteredCharityNumber=";; } }
        public string TrusteesPart { get { return "/Showcharity/RegisterOfCharities/ContactAndTrustees.aspx?SubsidiaryNumber=0&RegisteredCharityNumber="; ; } }

        public CharityComissionAdapter(IHttpContentGetter httpGet)
        {
            _httpGet = httpGet;
        }

        public string CharityComissionUriForRegistrationNumber(string registrationNumber)
        {
            return CharityComissionBaseUri + GeneralInfoPart + registrationNumber;
        }

        public string CharityComissionUriForTrustees(string registrationNumber)
        {
            return CharityComissionBaseUri + TrusteesPart + registrationNumber;
        }
        
        public CharityProfile LoadByRegNo(string regNo)
        {
            var generalDataDoc = new HtmlDocument();
            generalDataDoc.LoadHtml(_httpGet.Get(CharityComissionUriForRegistrationNumber(regNo)));

            var trusteeDataDoc = new HtmlDocument();
            trusteeDataDoc.LoadHtml(_httpGet.Get(CharityComissionUriForTrustees(regNo)));

            var incomeTable = generalDataDoc.GetElementbyId("TablesIncome").Descendants();
            var spendingTable = generalDataDoc.GetElementbyId("TablesSpending").Descendants();
            var assetsLiabilitiesAndPeople = generalDataDoc.GetElementbyId("TablesAssetsLiabilitiesAndPeople").Descendants();
            var charitableSpending = generalDataDoc.GetElementbyId("TablesCharitableSpending").Descendants();

            var htmlNodes = incomeTable as List<HtmlNode> ?? incomeTable.ToList();
            //var col = trusteeDataDoc.DocumentNode.ChildNodes.Where(x => x.Attributes.Contains("ScrollingSelectionLeftColumn"));

            return new CharityProfile
                {
                    CharityName = GetAndProcessString(() => generalDataDoc.GetElementbyId("ctl00_charityStatus_spnCharityName").InnerText),
                    CharityRegistrationNumber = GetAndProcessString(() => generalDataDoc.GetElementbyId("ctl00_charityStatus_spnCharityNo").InnerText),
                    MissionStatement = GetAndProcessString(() => generalDataDoc.GetElementbyId("ctl00_MainContent_ucDisplay_ucActivities_ucTextAreaInput_txtTextEntry").InnerText),
                    
                    Income = new Income
                        {
                            Total = htmlNodes.First(x => x.InnerText == "Total").NextSibling.NextSibling.InnerText,
                            Voluntary = htmlNodes.First(x => x.InnerText == "Voluntary").NextSibling.NextSibling.InnerText,
                            TradingToRaiseFunds = incomeTable.First(x => x.InnerText == "Trading to raise funds").NextSibling.NextSibling.InnerText,
                            Investment = incomeTable.First(x => x.InnerText == "Investment").NextSibling.NextSibling.InnerText,
                            CharitableActivities = incomeTable.First(x => x.InnerText == "Charitable activities").NextSibling.NextSibling.InnerText,
                            Other = incomeTable.First(x => x.InnerText == "Other").NextSibling.NextSibling.InnerText,
                            InvestmentGains = incomeTable.First(x => x.InnerText == "Investment gains").NextSibling.NextSibling.InnerText,
                        },

                    Expenditure = new Expenditure
                        {
                            GeneratingVoluntaryIncome = spendingTable.First(x => x.InnerText == "Generating voluntary income").NextSibling.NextSibling.InnerText,
                            Governance = spendingTable.First(x => x.InnerText == "Governance").NextSibling.NextSibling.InnerText,
                            TradingToRaiseFunds = spendingTable.First(x => x.InnerText == "Trading to raise funds").NextSibling.NextSibling.InnerText,
                            InvestmentManagement = spendingTable.First(x => x.InnerText == "Investment management").NextSibling.NextSibling.InnerText,
                            CharitableActivities = spendingTable.First(x => x.InnerText == "Charitable activities").NextSibling.NextSibling.InnerText,
                            Other = spendingTable.First(x => x.InnerText == "Other").NextSibling.NextSibling.InnerText,
                            Total = spendingTable.First(x => x.InnerText == "Total").NextSibling.NextSibling.InnerText,
                        },

                    AssetsLiabilitiesAndPeople = new AssetsLiabilitiesAndPeople
                        {
                            OwnUseAssets = assetsLiabilitiesAndPeople.First(x => x.InnerText == "Own use assets").NextSibling.NextSibling.InnerText,
                            LongTermInvestments = assetsLiabilitiesAndPeople.First(x => x.InnerText == "Long term investments").NextSibling.NextSibling.InnerText,
                            OtherAssets = assetsLiabilitiesAndPeople.First(x => x.InnerText == "Other assets").NextSibling.NextSibling.InnerText,
                            TotalLiabilities = assetsLiabilitiesAndPeople.First(x => x.InnerText == "Total liabilities").NextSibling.NextSibling.InnerText,
                            Employees = assetsLiabilitiesAndPeople.First(x => x.InnerText == "Employees").NextSibling.NextSibling.InnerText,
                            Volunteers = assetsLiabilitiesAndPeople.First(x => x.InnerText == "Volunteers").NextSibling.NextSibling.InnerText,
                        },

                    CharitableSpending = new CharitableSpending
                        {
                            IncomeGenerationAndGovernance = charitableSpending.First(x => x.InnerText == "Income generation and governance").NextSibling.NextSibling.InnerText,
                            CharitableSpendingTotal = charitableSpending.First(x => x.InnerText == "Charitable spending").NextSibling.NextSibling.InnerText,
                        }
                };
        }

        public string GetAndProcessString(Func<string> getString)
        {
            try
            {
                var @string = getString();
                @string = @string.Trim();
                @string = @string.Replace("&nbsp;-&nbsp;", "");
                @string = @string.Replace("&nbsp;", "");
                return @string.ToLower();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}