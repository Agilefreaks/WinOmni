using NUnit.Framework;
using Omnipaste.Services;

namespace OmnipasteTests.Services
{
    [TestFixture]
    public class ClickOnceActivationDataProviderTests
    {
        private ClickOnceActivationDataProvider _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new ClickOnceActivationDataProvider();
        }
    }
}
