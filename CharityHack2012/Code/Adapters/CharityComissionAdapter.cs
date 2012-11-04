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
        public string GeneralInfoPart { get { return "/Showcharity/RegisterOfCharities/CharityWithPartB.aspx?SubsidiaryNumber=0&RegisteredCharityNumber="; } }
        public string TrusteesPart { get { return "/Showcharity/RegisterOfCharities/ContactAndTrustees.aspx?SubsidiaryNumber=0&RegisteredCharityNumber="; } }

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

            var trusteeNodes =
                trusteeDataDoc.DocumentNode.Descendants().Where(
                    x => x.Name == "a" 
                            && x.Id.Contains("ctl00_MainContent_") 
                            && !x.InnerText.Contains("#99") 
                            && !x.InnerText.Contains("www."))
                    .ToList();
            var trusteeNames = trusteeNodes.Select(trustee => GetAndProcessString(()=>trustee.InnerText)).Where(x=>!string.IsNullOrWhiteSpace(x)).ToList();

            var prof = new CharityProfile
                {
                    CharityName = GetAndProcessString(() => generalDataDoc.GetElementbyId("ctl00_charityStatus_spnCharityName").InnerText),
                    CharityRegistrationNumber = GetAndProcessString(() => generalDataDoc.GetElementbyId("ctl00_charityStatus_spnCharityNo").InnerText),
                    MissionStatement = GetAndProcessString(() => generalDataDoc.GetElementbyId("ctl00_MainContent_ucDisplay_ucActivities_ucTextAreaInput_txtTextEntry").InnerText),
                    TrusteeNames = trusteeNames,

                    Income = new Income
                        {
                            Total = GetAndProcessDecimal(() => incomeTable.TableValue("Total")),
                            Voluntary = GetAndProcessDecimal(() => incomeTable.TableValue("Voluntary")),
                            TradingToRaiseFunds = GetAndProcessDecimal(() => incomeTable.TableValue("Trading to raise funds")),
                            Investment = GetAndProcessDecimal(() => incomeTable.TableValue("Investment")),
                            CharitableActivities = GetAndProcessDecimal(() => incomeTable.TableValue("Charitable activities")),
                            Other = GetAndProcessDecimal(() => incomeTable.TableValue("Other")),
                            InvestmentGains = GetAndProcessDecimal(() => incomeTable.TableValue("Investment gains")),
                        },

                    Expenditure = new Expenditure
                        {
                            GeneratingVoluntaryIncome = GetAndProcessDecimal(() => spendingTable.TableValue("Generating voluntary income")),
                            Governance = GetAndProcessDecimal(() => spendingTable.TableValue("Governance")),
                            TradingToRaiseFunds = GetAndProcessDecimal(() => spendingTable.TableValue("Trading to raise funds")),
                            InvestmentManagement = GetAndProcessDecimal(() => spendingTable.TableValue("Investment management")),
                            CharitableActivities = GetAndProcessDecimal(() => spendingTable.TableValue("Charitable activities")),
                            Other = GetAndProcessDecimal(() => spendingTable.TableValue("Other")),
                            Total = GetAndProcessDecimal(() => spendingTable.TableValue("Total")),
                        },

                    AssetsLiabilitiesAndPeople = new AssetsLiabilitiesAndPeople
                        {
                            OwnUseAssets = GetAndProcessDecimal(() => assetsLiabilitiesAndPeople.TableValue("Own use assets")),
                            LongTermInvestments = GetAndProcessDecimal(() => assetsLiabilitiesAndPeople.TableValue("Long term investments")),
                            OtherAssets = GetAndProcessDecimal(() => assetsLiabilitiesAndPeople.TableValue("Other assets")),
                            TotalLiabilities = GetAndProcessDecimal(() => assetsLiabilitiesAndPeople.TableValue("Total liabilities")),
                            Employees = GetAndProcessDecimal(() => assetsLiabilitiesAndPeople.TableValue("Employees")),
                            Volunteers = GetAndProcessDecimal(() => assetsLiabilitiesAndPeople.TableValue("Volunteers")),
                        },

                    CharitableSpending = new CharitableSpending
                        {
                            IncomeGenerationAndGovernance = GetAndProcessDecimal(() => charitableSpending.TableValue("Income generation and governance")),
                            CharitableSpendingTotal = GetAndProcessDecimal(() => charitableSpending.TableValue("Charitable spending")),
                        }
                };

            return prof;
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
                @string = @string.Replace("\n", "");
                @string = @string.Replace("\r", "");
                @string = @string.Replace("&nbsp;-&nbsp;", "");
                @string = @string.Replace("&nbsp;", "");
                @string = @string.Replace("&amp;", "&");
                @string = @string.Trim();
                return @string.ToLower();
            }
            catch
            {
                return string.Empty;
            }
        }

        public decimal? GetAndProcessDecimal(Func<string> getString)
        {
            try
            {
                var @string = getString();
                @string = @string.Replace("\n", "");
                @string = @string.Replace("\r", "");
                @string = @string.Replace("&nbsp;-&nbsp;", "");
                @string = @string.Replace("&nbsp;", "");
                @string = @string.Replace("&amp;", "&");
                @string = @string.Trim();

                decimal dec;
                return decimal.TryParse(@string, out dec) ? (decimal?) dec : null;
            }
            catch
            {
                return null;
            }
        }
    }

    public static class Extensions
    {
        public static string TableValue(this IEnumerable<HtmlNode> nodes, string key)
        {
            return nodes.First(x => x.InnerText == key).NextSibling.NextSibling.InnerText;
        }
    }
}