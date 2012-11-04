using System;
using System.Collections.Generic;
using System.Linq;
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
            var generalDataUri = CharityComissionUriForRegistrationNumber(regNo);
            var generalDataDoc = new HtmlDocument();
            var generalDataRaw = _httpGet.Get(generalDataUri);

            if (generalDataRaw.Contains("The page you have requested does not exist"))
            {
                generalDataRaw = _httpGet.Get(generalDataUri.Replace("CharityWithPartB", "CharityWithoutPartB"));
            }

            generalDataDoc.LoadHtml(generalDataRaw);

            var trusteeUri = CharityComissionUriForTrustees(regNo);
            var trusteeDataDoc = new HtmlDocument();
            var trusteeRaw = _httpGet.Get(trusteeUri);
            trusteeRaw = trusteeRaw.Replace("class=\"ScrollingSelectionLeftColumn\"", "id=\"trusteeDataDoc\" class=\"ScrollingSelectionLeftColumn\"");
            trusteeDataDoc.LoadHtml(trusteeRaw);


            var incomeTable = GetById(generalDataDoc, "TablesIncome");
            var spendingTable = GetById(generalDataDoc, "TablesSpending"); 
            var assetsLiabilitiesAndPeople = GetById(generalDataDoc, "TablesAssetsLiabilitiesAndPeople"); 
            var charitableSpending = GetById(generalDataDoc, "TablesCharitableSpending");

            var trusteeNodes = trusteeDataDoc.DocumentNode.Descendants().Where(x => x.Name == "a" && x.Id.Contains("ctl00_MainContent_")).ToList();
            var trusteeNames = trusteeNodes.Select(trustee => trustee.InnerText).ToList();

            return new CharityProfile
                {
                    CharityName = GetAndProcessString(() => generalDataDoc.GetElementbyId("ctl00_charityStatus_spnCharityName").InnerText),
                    CharityRegistrationNumber = GetAndProcessString(() => generalDataDoc.GetElementbyId("ctl00_charityStatus_spnCharityNo").InnerText),
                    MissionStatement = GetAndProcessString(() => generalDataDoc.GetElementbyId("ctl00_MainContent_ucDisplay_ucActivities_ucTextAreaInput_txtTextEntry").InnerText),
                    TrusteeNames = trusteeNames,

                    Income = new Income
                        {
                            Total = GetAndProcessString(() => incomeTable.First(x => x.InnerText == "Total").NextSibling.NextSibling.InnerText),
                            Voluntary = GetAndProcessString(() => incomeTable.First(x => x.InnerText == "Voluntary").NextSibling.NextSibling.InnerText),
                            TradingToRaiseFunds = GetAndProcessString(() =>incomeTable.First(x => x.InnerText == "Trading to raise funds").NextSibling.NextSibling.InnerText),
                            Investment = GetAndProcessString(() =>incomeTable.First(x => x.InnerText == "Investment").NextSibling.NextSibling.InnerText),
                            CharitableActivities = GetAndProcessString(() =>incomeTable.First(x => x.InnerText == "Charitable activities").NextSibling.NextSibling.InnerText),
                            Other = GetAndProcessString(() =>incomeTable.First(x => x.InnerText == "Other").NextSibling.NextSibling.InnerText),
                            InvestmentGains = GetAndProcessString(() =>incomeTable.First(x => x.InnerText == "Investment gains").NextSibling.NextSibling.InnerText),
                        },

                    Expenditure = new Expenditure
                        {
                            GeneratingVoluntaryIncome = GetAndProcessString(() =>spendingTable.First(x => x.InnerText == "Generating voluntary income").NextSibling.NextSibling.InnerText),
                            Governance = GetAndProcessString(() =>spendingTable.First(x => x.InnerText == "Governance").NextSibling.NextSibling.InnerText),
                            TradingToRaiseFunds = GetAndProcessString(() =>spendingTable.First(x => x.InnerText == "Trading to raise funds").NextSibling.NextSibling.InnerText),
                            InvestmentManagement = GetAndProcessString(() =>spendingTable.First(x => x.InnerText == "Investment management").NextSibling.NextSibling.InnerText),
                            CharitableActivities = GetAndProcessString(() =>spendingTable.First(x => x.InnerText == "Charitable activities").NextSibling.NextSibling.InnerText),
                            Other = GetAndProcessString(() =>spendingTable.First(x => x.InnerText == "Other").NextSibling.NextSibling.InnerText),
                            Total = GetAndProcessString(() =>spendingTable.First(x => x.InnerText == "Total").NextSibling.NextSibling.InnerText),
                        },

                    AssetsLiabilitiesAndPeople = new AssetsLiabilitiesAndPeople
                        {
                            OwnUseAssets = GetAndProcessString(() =>assetsLiabilitiesAndPeople.First(x => x.InnerText == "Own use assets").NextSibling.NextSibling.InnerText),
                            LongTermInvestments = GetAndProcessString(() =>assetsLiabilitiesAndPeople.First(x => x.InnerText == "Long term investments").NextSibling.NextSibling.InnerText),
                            OtherAssets =GetAndProcessString(() => assetsLiabilitiesAndPeople.First(x => x.InnerText == "Other assets").NextSibling.NextSibling.InnerText),
                            TotalLiabilities = GetAndProcessString(() =>assetsLiabilitiesAndPeople.First(x => x.InnerText == "Total liabilities").NextSibling.NextSibling.InnerText),
                            Employees =GetAndProcessString(() => assetsLiabilitiesAndPeople.First(x => x.InnerText == "Employees").NextSibling.NextSibling.InnerText),
                            Volunteers =GetAndProcessString(() => assetsLiabilitiesAndPeople.First(x => x.InnerText == "Volunteers").NextSibling.NextSibling.InnerText),
                        },

                    CharitableSpending = new CharitableSpending
                        {
                            IncomeGenerationAndGovernance = GetAndProcessString(() =>charitableSpending.First(x => x.InnerText == "Income generation and governance").NextSibling.NextSibling.InnerText),
                            CharitableSpendingTotal = GetAndProcessString(() => charitableSpending.First(x => x.InnerText == "Charitable spending").NextSibling.NextSibling.InnerText),
                        }
                };
        }

        private static IEnumerable<HtmlNode> GetById(HtmlDocument generalDataDoc, string id)
        {
            try
            {
                return generalDataDoc.GetElementbyId(id).Descendants();
            }
            catch
            {
                return new List<HtmlNode>();
            }
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