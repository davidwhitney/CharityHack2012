using CharityHack2012.Code.Adapters;
using NUnit.Framework;

namespace CharityHack2012.Test.Unit.Code.Adapters
{
    [TestFixture]
    public class CharityComissionAdapterTests
    {
        private CharityComissionAdapter _adapter;
        const string RegNo = "12345";

        [SetUp]
        public void SetUp()
        {
            _adapter = new CharityComissionAdapter();
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
            _adapter.LoadByRegNo(RegNo);
        }
    }
}
