using Caliburn.Micro;
using Moq;
using NUnit.Framework;
using OmniCommon.EventAggregatorMessages;
using Omnipaste.Framework;
using Omnipaste.Services.Connectivity;

namespace OmnipasteTests.Framework
{
    using Omni;
    using OmniCommon.Interfaces;

    [TestFixture]
    public class OmniServiceHandlerTests
    {
        private OmniServiceHandler _subject;
        private Mock<IOmniService> _mockOmniService;
        private Mock<IEventAggregator> _mockEventAggregator;
        private Mock<IConnectivityNotifyService> _mockConnectivityObserver;

        [SetUp]
        public void SetUp()
        {
            _mockOmniService = new Mock<IOmniService>();
            _mockEventAggregator = new Mock<IEventAggregator>();
            _mockConnectivityObserver = new Mock<IConnectivityNotifyService>();

            _subject = new OmniServiceHandler(_mockOmniService.Object, _mockEventAggregator.Object,
                                              _mockConnectivityObserver.Object);
        }

        [Test]
        public void HandleStartServiceMessage_Always_CallsOmniServiceStart()
        {
            _subject.Handle(new StartOmniServiceMessage());

            _mockOmniService.Verify(m => m.Start(null));
        }

        [Test]
        public void HandlStopServiceMessage_Always_CallsOmniServiceStop()
        {
            _subject.Handle(new StopOmniServiceMessage());

            _mockOmniService.Verify(m => m.Stop());
        }

        [Test]
        public void ConnectivityChanged_WhenServiceShouldBeRunningAndDisconnectedFromInternet_CallsOmniServiceStop()
        {
            _subject.Handle(new StartOmniServiceMessage());

            _mockConnectivityObserver.Raise(x => x.ConnectivityChanged += null, new ConnectivityChangedEventArgs(false));

            _mockOmniService.Verify(m => m.Stop());
        }

        [Test]
        public void ConnectivityCHanged_WhenServiceShouldBeRunningAndReconnectedToInternet_CallsOmniServiceStart()
        {
            _subject.Handle(new StartOmniServiceMessage());
            _mockConnectivityObserver.Raise(x => x.ConnectivityChanged += null, new ConnectivityChangedEventArgs(false));

            _mockConnectivityObserver.Raise(x => x.ConnectivityChanged += null, new ConnectivityChangedEventArgs(true));

            _mockOmniService.Verify(m => m.Start(null), Times.Exactly(2));
        }
    }
}
