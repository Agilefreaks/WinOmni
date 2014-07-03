namespace OmniSyncTests
{
    using Moq;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using OmniSync;
    using WampSharp;

    [TestFixture]
    public class WebsocketConnectionTests
    {
        private WebsocketConnection _subject;

        [SetUp]
        public void SetUp()
        {
            _subject = new WebsocketConnection(new Mock<IWampChannel<JToken>>().Object);
        }
    }
}