namespace OmniSyncTests
{
    using System;
    using System.Reactive;
    using System.Reactive.Subjects;
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

        private TestScheduler _testScheduler;

        private ITestableObserver<string> _testObserver;

        [SetUp]
        public void SetUp()
        {
            _mockChannel = new Mock<IWampChannel<JToken>>();
            _mockMonitor = new Mock<IWampClientConnectionMonitor> { DefaultValue = DefaultValue.Empty };

            _mockChannel.Setup(m => m.GetMonitor()).Returns(_mockMonitor.Object);

            _subject = new WebsocketConnection(_mockChannel.Object);

            _testScheduler = new TestScheduler();
            _testObserver = _testScheduler.CreateObserver<string>();
        }

        [Test]
        public void ConnectWhenSessioIdIsNotNullCallsNextWithTheRegistration()
        {
            _mockMonitor.SetupGet(m => m.SessionId).Returns("42");

            _subject.Connect().Subscribe(_testObserver);

            _testObserver.Messages.Should()
                .Contain(m => m.Value.Kind == NotificationKind.OnNext && m.Value.Value == "42");            
        }

        [Test]
        public void OnWebsocketError_TheConnectionObservableReturnsDisconnected()
        {
            bool errorReceived = false;

            _subject.Subscribe<WebsocketConnectionStatusEnum>(x => errorReceived = true, e => {});
            _mockMonitor.Raise(m => m.ConnectionError += null, new WampConnectionErrorEventArgs(new Exception()));

            errorReceived.Should().BeTrue();
        }
    }
}