using CharityHack2012.Code.Adapters;
using NUnit.Framework;

namespace CharityHack2012.Test.Unit.Code.Adapters
{
    [TestFixture]
    public class CharityComissionAdapterTests
    {
        private CharityComissionAdapter _adapter;

        [SetUp]
        public void SetUp()
        {
            _adapter = new CharityComissionAdapter();
        }

        [Test]
        public void CharityComissionUriForRegistrationNumber_WhenProvidedWithARegNo_GeneratesAppropriateUrl()
        {
            const string regNo = "12345";

            var uri = _adapter.CharityComissionUriForRegistrationNumber(regNo);

            Assert.That(uri, Is.EqualTo(_adapter.CharityComissionBaseUri + _adapter.UrlPart + regNo));
        }
    }
}
