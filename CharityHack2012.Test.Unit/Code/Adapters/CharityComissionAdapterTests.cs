using System.IO;
using CharityHack2012.Code.Adapters;
using CharityHack2012.Code.Http;
using Moq;
using NUnit.Framework;

namespace CharityHack2012.Test.Unit.Code.Adapters
{
    [TestFixture]
    public class CharityComissionAdapterTests
    {
        private CharityComissionAdapter _adapter;
        private Mock<IHttpContentGetter> _httpGetter;
        const string RegNo = "12345";

        [SetUp]
        public void SetUp()
        {
            _httpGetter = new Mock<IHttpContentGetter>();
            _httpGetter.Setup(x => x.Get(It.Is<string>(y => y.Contains("CharityWithPartB")))).Returns(File.ReadAllText("cruk.html"));
            _httpGetter.Setup(x => x.Get(It.Is<string>(y => y.Contains("ContactAndTrustees")))).Returns(File.ReadAllText("cruk2.html"));
            _adapter = new CharityComissionAdapter(_httpGetter.Object);
        }

        [Test]
        public void CharityComissionUriForRegistrationNumber_WhenProvidedWithARegNo_GeneratesAppropriateUrl()
        {
            var uri = _adapter.CharityComissionUriForRegistrationNumber(RegNo);

            Assert.That(uri, Is.EqualTo(_adapter.CharityComissionBaseUri + _adapter.GeneralInfoPart + RegNo));
        }

        [Test]
        public void LoadByRegNo_WhenCalledWithRegNumber_ScapesPageAndReturnsDtoOfData()
        {
            var charityProfile = _adapter.LoadByRegNo(RegNo);

            Assert.That(charityProfile.CharityName, Is.EqualTo("cancer research uk"));
            Assert.That(charityProfile.CharityRegistrationNumber, Is.EqualTo("1089464"));
            Assert.That(charityProfile.MissionStatement, Is.EqualTo("TO PROTECT AND PROMOTE THE HEALTH OF THE PUBLIC IN PARTICULAR BY RESEARCH INTO THE NATURE, CAUSES, DIAGNOSIS, PREVENTION, TREATMENT AND CURE OF ALL FORMS OF CANCER, INCLUDING THE DEVELOPMENT OF RESEARCH INTO PRACTICAL APPLICATIONS FOR THE PREVENTION, TREATMENT AND CURE OF CANCER AND TO PROVIDE INFORMATION AND RAISE PUBLIC UNDERSTANDING OF SUCH MATTERS.".ToLower()));
            Assert.That(charityProfile.Income.Total, Is.EqualTo(492627000m));
            Assert.That(charityProfile.Income.Voluntary, Is.EqualTo(350080000m));
            Assert.That(charityProfile.Income.TradingToRaiseFunds, Is.EqualTo(82071000m));
            Assert.That(charityProfile.Income.Investment, Is.EqualTo(2914000m));
            Assert.That(charityProfile.Income.CharitableActivities, Is.EqualTo(57562000m));
            Assert.That(charityProfile.Income.Other, Is.EqualTo(0m));
            Assert.That(charityProfile.Income.InvestmentGains, Is.EqualTo(1200000m));
            Assert.That(charityProfile.Expenditure.GeneratingVoluntaryIncome, Is.EqualTo(80063000m));
            Assert.That(charityProfile.Expenditure.Governance, Is.EqualTo(1457000m));
            Assert.That(charityProfile.Expenditure.TradingToRaiseFunds, Is.EqualTo(68615000m));
            Assert.That(charityProfile.Expenditure.InvestmentManagement, Is.EqualTo(238000m));
            Assert.That(charityProfile.Expenditure.CharitableActivities, Is.EqualTo(347877000m));
            Assert.That(charityProfile.Expenditure.Other, Is.EqualTo(0m));
            Assert.That(charityProfile.Expenditure.Total, Is.EqualTo(498250000m));
            Assert.That(charityProfile.AssetsLiabilitiesAndPeople.OwnUseAssets, Is.EqualTo(92600000m));
            Assert.That(charityProfile.AssetsLiabilitiesAndPeople.LongTermInvestments, Is.EqualTo(110651000m));
            Assert.That(charityProfile.AssetsLiabilitiesAndPeople.OtherAssets, Is.EqualTo(249757000m));
            Assert.That(charityProfile.AssetsLiabilitiesAndPeople.TotalLiabilities, Is.EqualTo(-271759000m));
            Assert.That(charityProfile.AssetsLiabilitiesAndPeople.Employees, Is.EqualTo(3968m));
            Assert.That(charityProfile.AssetsLiabilitiesAndPeople.Volunteers, Is.EqualTo(40000m));
            Assert.That(charityProfile.CharitableSpending.IncomeGenerationAndGovernance, Is.EqualTo(150373000m));
            Assert.That(charityProfile.CharitableSpending.CharitableSpendingTotal, Is.EqualTo(347877000m));

        }
    }
}
