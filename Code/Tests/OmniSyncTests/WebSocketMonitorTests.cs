namespace OmniSyncTests
{
    using System;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using OmniSync;
    using WampSharp.Auxiliary.Client;

    [TestFixture]
    public class WebSocketMonitorTests
    {
        private WebSocketMonitor _subject;

        private Mock<IWampClientConnectionMonitor> _mockMonitor;

        private Mock<IWebsocketConnection> _mockWebSocketConnection;

        [SetUp]
        public void Setup()
        {
            _subject = new WebSocketMonitor();
            _mockWebSocketConnection = new Mock<IWebsocketConnection>();
            _mockMonitor = new Mock<IWampClientConnectionMonitor> { DefaultValue = DefaultValue.Empty };
            _mockWebSocketConnection.Setup(x => x.Monitor).Returns(_mockMonitor.Object);
        }

        [Test]
        public void AfterStart_OnWebsocketError_TheConnectionObservableReturnsDisconnected()
        {
            WebSocketConnectionStatusEnum? messageReceived = null;
            _subject.Start(_mockWebSocketConnection.Object);
            _subject.ConnectionObservable.Subscribe(x => messageReceived = x, e => { });

            _mockMonitor.Raise(m => m.ConnectionError += null, new WampConnectionErrorEventArgs(new Exception()));

            messageReceived.Should().Be(WebSocketConnectionStatusEnum.Disconnected);
        }
    }
}