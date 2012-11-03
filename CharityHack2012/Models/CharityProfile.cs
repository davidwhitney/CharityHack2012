using System.Collections.Generic;

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
        
        public string CharityImage { get { return "/content/img/charity-logo-default.png"; }  }
        public string CharitySummary { get { return "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse et erat nunc. Nam quam felis, rutrum sed vehicula vitae, eleifend dictum arcu. Nam iaculis dignissim aliquam. Cras tristique, risus ac facilisis congue, leo metus adipiscing metus, sed viverra nulla augue a metus. Nulla varius sollicitudin velit, in rhoncus nisi semper eget. Mauris mi nunc, tristique non laoreet nec, volutpat eget lorem. Donec justo massa, ultricies eu pretium eget, vehicula vel ante. Praesent at nisi ipsum. Ut et arcu nisl, a sagittis velit. Nam ac sollicitudin ante. Mauris eget nunc eget urna tincidunt eleifend. Cras placerat hendrerit magna eu lacinia. Etiam id urna risus, nec eleifend velit. Aenean commodo vulputate leo."; }  }

        public List<string> TrusteeNames { get; set; }

        public CharityProfile()
        {
            Income = new Income();
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
}