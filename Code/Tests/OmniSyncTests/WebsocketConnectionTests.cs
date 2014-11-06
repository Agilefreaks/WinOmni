namespace OmniSyncTests
{
    using System;
    using System.Reactive;
    using System.Reactive.Subjects;
    using System.Threading;
    using FluentAssertions;
    using Microsoft.Reactive.Testing;
    using Moq;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using OmniCommon.Models;
    using OmniSync;
    using WampSharp.Core.Listener;
    using WampSharp.V1;
    using WampSharp.V1.Auxiliary.Client;

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
        public void ConnectWhenCanOpenChannelAndSessionIdIsNotNullCallsNextWithTheRegistration()
        {
            _mockChannel.Setup(x => x.Open()).Callback(() => { });
            _mockMonitor.SetupGet(m => m.SessionId).Returns("42");

            _subject.Connect().Subscribe(_testObserver);

            _testObserver.Messages.Should()
                .Contain(m => m.Value.Kind == NotificationKind.OnNext && m.Value.Value == "42");            
        }

        [Test]
        public void SubscribeOmniMessageObserver_ConnectWasNeverCalled_DoesNotThrowException()
        {	
            var connection = new WebsocketConnection(_mockChannel.Object);

            connection.Subscribe(x => {}, e => { });
        }        
        
        [Test]
        public void Connect_Always_SetsUpTheConnectionToRelayChannelMessages()
        {
            OmniMessage receivedMessage = null;
            var messageToSimulate = new OmniMessage();
            var replaySubject = new ReplaySubject<OmniMessage>();
            var autoResetEvent = new AutoResetEvent(false);
            _subject.Subscribe(
                x =>
                    {
                        receivedMessage = x;
                        autoResetEvent.Set();
                    });
            _mockMonitor.SetupGet(m => m.SessionId).Returns("42");
            _mockChannel.Setup(x => x.GetSubject<OmniMessage>("42")).Returns(replaySubject);
            _subject.Connect();
            replaySubject.OnNext(messageToSimulate);

            autoResetEvent.WaitOne();
            receivedMessage.Should().Be(messageToSimulate);
        }
    }
}