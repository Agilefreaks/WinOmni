using NUnit.Framework;
using Omnipaste.Services;

namespace OmnipasteTests.Services
{
    [TestFixture]
    public class OnlineActivationDataProviderTests
    {
        private OnlineActivationDataProvider _subject;

        [SetUp]
        public void Setup()
        {
            _subject = new OnlineActivationDataProvider();
        }
    }
}
