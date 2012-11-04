using System.Collections.Generic;
using CharityHack2012.Code;
using CharityHack2012.Code.Adapters;
using JustGiving.Api.Sdk.Model.Charity;
using JustGiving.Api.Sdk.Model.Search;

namespace CharityHack2012.Models
{
    public class CharityProfile
    {
        public string CharityName { get; set; }
        public string CharityRegistrationNumber { get; set; }
        public string MissionStatement { get; set; }

        public Income Income { get; set; }
        public Expenditure Expenditure { get; set; }
        public AssetsLiabilitiesAndPeople AssetsLiabilitiesAndPeople { get; set; }
        public CharitableSpending CharitableSpending { get; set; }

        public string CharityImage { get; set; }

        public List<string> TrusteeNames { get; set; }
        public List<Item> NewsItems { get; set; }

        public CharityEntity JgCharityData { get; set; }

        public bool HasJgData
        {
            get { return JgCharityData != null; }
        }

        public CharitySearchResults RelatedCharities { get; set; }

        public CharityProfile()
        {
            Income = new Income();
            CharityImage = "/content/img/charity-logo-default.png";
        }
    }

    public class Income
    {
        public string Total { get; set; }
        public string Voluntary { get; set; }
        public string TradingToRaiseFunds { get; set; }
        public string Investment { get; set; }
        public string CharitableActivities { get; set; }
        public string Other { get; set; }
        public string InvestmentGains { get; set; }
    }
    
    public class Expenditure
    {
        public string GeneratingVoluntaryIncome { get; set; }
        public string Governance { get; set; }
        public string TradingToRaiseFunds { get; set; }
        public string InvestmentManagement { get; set; }
        public string CharitableActivities { get; set; }
        public string Other { get; set; }
        public string Total { get; set; }
    }

    public class AssetsLiabilitiesAndPeople
    {
        public string OwnUseAssets { get; set; }
        public string LongTermInvestments { get; set; }
        public string OtherAssets { get; set; }
        public string TotalLiabilities { get; set; }
        public string Employees { get; set; }
        public string Volunteers { get; set; }
    }

    public class CharitableSpending
    {
        public string IncomeGenerationAndGovernance { get; set; }
        public string CharitableSpendingTotal { get; set; }
    }

    public class CharitySearchResult
    {
        public int JgCharityId { get; set; }
        public string CharityDisplayName { get; set; }
        public string CharityRegistrationNumber { get; set; }
    }
}