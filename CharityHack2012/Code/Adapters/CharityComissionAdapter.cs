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
        public string UrlPart { get { return "/Showcharity/RegisterOfCharities/CharityWithPartB.aspx?SubsidiaryNumber=0&RegisteredCharityNumber=";; } }

        public CharityComissionAdapter(IHttpContentGetter httpGet)
        {
            _httpGet = httpGet;
        }

        public string CharityComissionUriForRegistrationNumber(string registrationNumber)
        {
            return CharityComissionBaseUri + UrlPart + registrationNumber;
        }
        
        public CharityProfile LoadByRegNo(string regNo)
        {
            var uri = CharityComissionUriForRegistrationNumber(regNo);
            var body = _httpGet.Get(uri);

            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var incomeTable = doc.GetElementbyId("TablesIncome").Descendants();
            var spendingTable = doc.GetElementbyId("TablesSpending").Descendants();
            var assetsLiabilitiesAndPeople = doc.GetElementbyId("TablesAssetsLiabilitiesAndPeople").Descendants();

            return new CharityProfile
                {
                    CharityName = GetAndProcessString(() => doc.GetElementbyId("ctl00_charityStatus_spnCharityName").InnerText),
                    CharityRegistrationNumber = GetAndProcessString(() => doc.GetElementbyId("ctl00_charityStatus_spnCharityNo").InnerText),
                    Income = new Income
                        {
                            Total = incomeTable.First(x => x.InnerText == "Total").NextSibling.NextSibling.InnerText,
                            Voluntary = incomeTable.First(x => x.InnerText == "Voluntary").NextSibling.NextSibling.InnerText,
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