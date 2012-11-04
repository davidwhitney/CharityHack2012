using System;
using System.Collections.Generic;
using CharityHack2012.Code;
using CharityHack2012.Code.Adapters;
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

        public decimal? VolenteerAverageRaised
        {
            get
            {
                if(Income.Voluntary.GetValueOrDefault(Int32.MinValue) == Int32.MinValue
                    || AssetsLiabilitiesAndPeople.Volunteers.GetValueOrDefault(Int32.MinValue) == Int32.MinValue
                    || Income.Voluntary == 0
                    || AssetsLiabilitiesAndPeople.Volunteers == 0)
                {
                    return null;
                }

                return Income.Voluntary/AssetsLiabilitiesAndPeople.Volunteers;
            }
        }
    }

    public class Income
    {
        public decimal? Total { get; set; }
        public decimal? Voluntary { get; set; }
        public decimal? TradingToRaiseFunds { get; set; }
        public decimal? Investment { get; set; }
        public decimal? CharitableActivities { get; set; }
        public decimal? Other { get; set; }
        public decimal? InvestmentGains { get; set; }
    }
    
    public class Expenditure
    {
        public decimal? GeneratingVoluntaryIncome { get; set; }
        public decimal? Governance { get; set; }
        public decimal? TradingToRaiseFunds { get; set; }
        public decimal? InvestmentManagement { get; set; }
        public decimal? CharitableActivities { get; set; }
        public decimal? Other { get; set; }
        public decimal? Total { get; set; }
    }

    public class AssetsLiabilitiesAndPeople
    {
        public decimal? OwnUseAssets { get; set; }
        public decimal? LongTermInvestments { get; set; }
        public decimal? OtherAssets { get; set; }
        public decimal? TotalLiabilities { get; set; }
        public decimal? Employees { get; set; }
        public decimal? Volunteers { get; set; }
    }

    public class CharitableSpending
    {
        public decimal? IncomeGenerationAndGovernance { get; set; }
        public decimal? CharitableSpendingTotal { get; set; }
    }

    public class CharitySearchResult
    {
        public int JgCharityId { get; set; }
        public string CharityDisplayName { get; set; }
        public string CharityRegistrationNumber { get; set; }
    }
}