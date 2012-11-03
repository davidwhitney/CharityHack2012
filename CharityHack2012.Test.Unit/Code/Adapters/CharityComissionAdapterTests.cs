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

            Assert.That(uri, Is.EqualTo(_adapter.CharityComissionBaseUri + _adapter.TrusteesPart + RegNo));
        }

        [Test]
        public void LoadByRegNo_WhenCalledWithRegNumber_ScapesPageAndReturnsDtoOfData()
        {
            var charityProfile = _adapter.LoadByRegNo(RegNo);

            Assert.That(charityProfile.CharityName, Is.EqualTo("cancer research uk"));
            Assert.That(charityProfile.CharityRegistrationNumber, Is.EqualTo("1089464"));
            Assert.That(charityProfile.MissionStatement, Is.EqualTo("TO PROTECT AND PROMOTE THE HEALTH OF THE PUBLIC IN PARTICULAR BY RESEARCH INTO THE NATURE, CAUSES, DIAGNOSIS, PREVENTION, TREATMENT AND CURE OF ALL FORMS OF CANCER, INCLUDING THE DEVELOPMENT OF RESEARCH INTO PRACTICAL APPLICATIONS FOR THE PREVENTION, TREATMENT AND CURE OF CANCER AND TO PROVIDE INFORMATION AND RAISE PUBLIC UNDERSTANDING OF SUCH MATTERS.".ToLower()));
            Assert.That(charityProfile.Income.Total, Is.EqualTo("492,627,000"));
            Assert.That(charityProfile.Income.Voluntary, Is.EqualTo("350,080,000"));
            Assert.That(charityProfile.Income.TradingToRaiseFunds, Is.EqualTo("82,071,000"));
            Assert.That(charityProfile.Income.Investment, Is.EqualTo("2,914,000"));
            Assert.That(charityProfile.Income.CharitableActivities, Is.EqualTo("57,562,000"));
            Assert.That(charityProfile.Income.Other, Is.EqualTo("0"));
            Assert.That(charityProfile.Income.InvestmentGains, Is.EqualTo("1,200,000"));
            Assert.That(charityProfile.Expenditure.GeneratingVoluntaryIncome, Is.EqualTo("80,063,000"));
            Assert.That(charityProfile.Expenditure.Governance, Is.EqualTo("1,457,000"));
            Assert.That(charityProfile.Expenditure.TradingToRaiseFunds, Is.EqualTo("68,615,000"));
            Assert.That(charityProfile.Expenditure.InvestmentManagement, Is.EqualTo("238,000"));
            Assert.That(charityProfile.Expenditure.CharitableActivities, Is.EqualTo("347,877,000"));
            Assert.That(charityProfile.Expenditure.Other, Is.EqualTo("0"));
            Assert.That(charityProfile.Expenditure.Total, Is.EqualTo("498,250,000"));
            Assert.That(charityProfile.AssetsLiabilitiesAndPeople.OwnUseAssets, Is.EqualTo("92,600,000"));
            Assert.That(charityProfile.AssetsLiabilitiesAndPeople.LongTermInvestments, Is.EqualTo("110,651,000"));
            Assert.That(charityProfile.AssetsLiabilitiesAndPeople.OtherAssets, Is.EqualTo("249,757,000"));
            Assert.That(charityProfile.AssetsLiabilitiesAndPeople.TotalLiabilities, Is.EqualTo("-271,759,000"));
            Assert.That(charityProfile.AssetsLiabilitiesAndPeople.Employees, Is.EqualTo("3,968"));
            Assert.That(charityProfile.AssetsLiabilitiesAndPeople.Volunteers, Is.EqualTo("40,000"));
            Assert.That(charityProfile.CharitableSpending.IncomeGenerationAndGovernance, Is.EqualTo("150,373,000"));
            Assert.That(charityProfile.CharitableSpending.CharitableSpendingTotal, Is.EqualTo("347,877,000"));

        }
    }
}
