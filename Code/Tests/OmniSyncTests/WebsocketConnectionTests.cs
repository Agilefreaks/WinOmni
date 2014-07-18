namespace OmniSyncTests
{
    using System.Reactive;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using OmniSync;
    using WampSharp;
    using WampSharp.Auxiliary.Client;

    [TestFixture]
    public class WebsocketConnectionTests
    {
        private WebsocketConnection _subject;

        private Mock<IWampChannel<JToken>> _mockChannel;

        private Mock<IWampClientConnectionMonitor> _mockMonitor;

        [SetUp]
        public void SetUp()
        {
            _mockChannel = new Mock<IWampChannel<JToken>>();
            _mockMonitor = new Mock<IWampClientConnectionMonitor>();

            _mockChannel.Setup(m => m.GetMonitor()).Returns(_mockMonitor.Object);

            _subject = new WebsocketConnection(_mockChannel.Object);
        }

        [Test]
        public void ConnectWhenSessionIdNullShouldCallOnError()
        {
            TestScheduler testScheduler = new TestScheduler();
            var testObserver = testScheduler.CreateObserver<string>();

            _mockMonitor.SetupGet(m => m.SessionId).Returns(() => null);

            _subject.Connect().Subscribe(testObserver);

            testObserver.Messages.Should()
                .Contain(m => m.Value.Kind == NotificationKind.OnError);
        }

        [Test]
        public void ConnectWhenSessioIdIsNotNullCallsNextWithTheRegistration()
        {
            TestScheduler testScheduler = new TestScheduler();
            var testObserver = testScheduler.CreateObserver<string>();

            _mockMonitor.SetupGet(m => m.SessionId).Returns("42");

            _subject.Connect().Subscribe(testObserver);

            testObserver.Messages.Should()
                .Contain(m => m.Value.Kind == NotificationKind.OnNext && m.Value.Value == "42");            
        }
    }
}