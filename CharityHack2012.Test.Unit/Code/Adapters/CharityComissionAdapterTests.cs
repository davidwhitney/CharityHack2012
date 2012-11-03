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
            _httpGetter.Setup(x => x.Get(It.IsAny<string>())).Returns(File.ReadAllText("cruk.html"));
            _adapter = new CharityComissionAdapter(_httpGetter.Object);
        }

        [Test]
        public void CharityComissionUriForRegistrationNumber_WhenProvidedWithARegNo_GeneratesAppropriateUrl()
        {

            var uri = _adapter.CharityComissionUriForRegistrationNumber(RegNo);

            Assert.That(uri, Is.EqualTo(_adapter.CharityComissionBaseUri + _adapter.UrlPart + RegNo));
        }

        [Test]
        public void LoadByRegNo_WhenCalledWithRegNumber_ScapesPageAndReturnsDtoOfData()
        {
            var charityProfile = _adapter.LoadByRegNo(RegNo);

            Assert.That(charityProfile.CharityName, Is.EqualTo("cancer research uk"));
            Assert.That(charityProfile.CharityRegistrationNumber, Is.EqualTo("1089464"));
            Assert.That(charityProfile.Income.Total, Is.EqualTo("492,627,000"));
            Assert.That(charityProfile.Income.Voluntary, Is.EqualTo("350,080,000"));
            Assert.That(charityProfile.Income.TradingToRaiseFunds, Is.EqualTo("82,071,000"));
            Assert.That(charityProfile.Income.Investment, Is.EqualTo("2,914,000"));
            Assert.That(charityProfile.Income.CharitableActivities, Is.EqualTo("57,562,000"));
            Assert.That(charityProfile.Income.Other, Is.EqualTo("0"));
        }
    }
}
